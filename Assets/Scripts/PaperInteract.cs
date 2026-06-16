using UnityEngine;
using TMPro;
using System.Collections; 

public class PaperInteract : MonoBehaviour
{
    [Header("Referensi Objek 3D (MIRIP LAPTOP)")]
    public Transform playerTransform; 
    public float maxInteractDistance = 3f; 

    [Header("Hubungan ke UI Kertas")]
    public GameObject paperUIPanel;
    public TextMeshProUGUI paperText;

    // ---> INI COLOKAN BARU BUAT EFEK BLUR KAMERA <---
    [Header("Post Processing Effects")]
    public GameObject blurVolume;

    [Header("Pengaturan Animasi Kertas")]
    public float slideDuration = 0.3f; 
    public Vector2 hiddenPosition = new Vector2(0, -1000f); 
    public Vector2 visiblePosition = new Vector2(0, 0f);   

    [HideInInspector] 
    public bool isReading = false; 

    private GameManager gameManager;
    private RectTransform paperRect; 
    private Coroutine activeAnim;    

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        if (paperUIPanel != null)
        {
            paperRect = paperUIPanel.GetComponent<RectTransform>();
            paperRect.anchoredPosition = hiddenPosition; 
            paperUIPanel.SetActive(false);
        }

        // Pastikan blur mati di awal
        if (blurVolume != null) blurVolume.SetActive(false);
    }

    void Update()
    {
        if (isReading && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            TutupKertas();
        }
    }

    void OnMouseDown()
    {
        if (playerTransform == null || activeAnim != null) return; 
        
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distance <= maxInteractDistance && !isReading)
        {
            BukaKertas();
        }
    }

    public void BukaKertas()
    {
        if (gameManager != null) paperText.text = gameManager.GetPaperText();
        else paperText.text = "GameManager tidak ditemukan...";

        isReading = true; 

        if (paperUIPanel != null)
        {
            paperUIPanel.SetActive(true); 
            if (activeAnim != null) StopCoroutine(activeAnim); 
            activeAnim = StartCoroutine(SlidePaperAnim(visiblePosition, false)); 
        }

        // Tutup HP paksa kalau lagi buka HP
        if (PhoneManager.instance != null) PhoneManager.instance.ForceHidePhone();

        // ---> NYALAKAN BLUR SETELAH HP DITUTUP <---
        if (blurVolume != null) blurVolume.SetActive(true);
    }

    public void TutupKertas()
    {
        isReading = false; 
        
        // ---> MATIKAN BLUR PAS KERTAS DITUTUP <---
        if (blurVolume != null) blurVolume.SetActive(false);

        if (paperUIPanel != null)
        {
            if (activeAnim != null) StopCoroutine(activeAnim); 
            activeAnim = StartCoroutine(SlidePaperAnim(hiddenPosition, true)); 
        }
        
        if (PhoneManager.instance != null) PhoneManager.instance.AllowPhoneUsage();
    }

    IEnumerator SlidePaperAnim(Vector2 targetPos, bool disableAfter)
    {
        Vector2 startPos = paperRect.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / slideDuration);
            
            paperRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null; 
        }

        paperRect.anchoredPosition = targetPos;

        if (disableAfter && paperUIPanel != null)
        {
            paperUIPanel.SetActive(false);
        }

        activeAnim = null; 
    }
}