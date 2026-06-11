using UnityEngine;
using TMPro;
using System.Collections; // <--- WAJIB DITAMBAHIN BUAT ANIMASI (COROUTINE)

public class PaperInteract : MonoBehaviour
{
    [Header("Referensi Objek 3D (MIRIP LAPTOP)")]
    public Transform playerTransform; // Colok FPS Controller lu ke sini bang!
    public float maxInteractDistance = 3f; 

    [Header("Hubungan ke UI Kertas")]
    public GameObject paperUIPanel;
    public TextMeshProUGUI paperText;

    [Header("Pengaturan Animasi Kertas")]
    public float slideDuration = 0.3f; // Kecepatan animasi (detik)
    public Vector2 hiddenPosition = new Vector2(0, -1000f); // Posisi ngumpet (Jauh di bawah layar)
    public Vector2 visiblePosition = new Vector2(0, 0f);   // Posisi nampil (Tepat di tengah)

    [HideInInspector] 
    public bool isReading = false; 

    private GameManager gameManager;
    private RectTransform paperRect; // Buat ngambil komponen posisi UI
    private Coroutine activeAnim;    // Buat nge-track animasi biar gak bentrok

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // Ambil komponen RectTransform dari UI Kertas
        if (paperUIPanel != null)
        {
            paperRect = paperUIPanel.GetComponent<RectTransform>();
            paperRect.anchoredPosition = hiddenPosition; // Posisikan di bawah layar pas game mulai
            paperUIPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Tutup pakai ESC / Klik Kanan
        if (isReading && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            TutupKertas();
        }
    }

    void OnMouseDown()
    {
        // Kalau player kosong ATAU kertas lagi proses animasi, jangan bisa diklik dulu
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

        // --- MULAI ANIMASI MUNCUL ---
        if (paperUIPanel != null)
        {
            paperUIPanel.SetActive(true); // Nyalain dulu objeknya
            if (activeAnim != null) StopCoroutine(activeAnim); // Stop animasi sebelumnya kalau ada
            activeAnim = StartCoroutine(SlidePaperAnim(visiblePosition, false)); // Jalanin ke tengah
        }

        if (PhoneManager.instance != null) PhoneManager.instance.ForceHidePhone();
    }

    public void TutupKertas()
    {
        isReading = false; 
        
        // --- MULAI ANIMASI NGUMPET ---
        if (paperUIPanel != null)
        {
            if (activeAnim != null) StopCoroutine(activeAnim); // Stop animasi sebelumnya kalau ada
            activeAnim = StartCoroutine(SlidePaperAnim(hiddenPosition, true)); // Jalanin ke bawah
        }
        
        if (PhoneManager.instance != null) PhoneManager.instance.AllowPhoneUsage();
    }

    // --- JURUS ANIMASI DARI PHONE MANAGER ---
    IEnumerator SlidePaperAnim(Vector2 targetPos, bool disableAfter)
    {
        Vector2 startPos = paperRect.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // SmoothStep biar gerakannya halus (ngegas di awal, ngerem di akhir)
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / slideDuration);
            
            paperRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null; // Tunggu ke frame berikutnya
        }

        // Pastiin posisinya pas di tujuan pas animasi beres
        paperRect.anchoredPosition = targetPos;

        // Kalau perintahnya buat nutup, matikan objeknya setelah nyampe bawah
        if (disableAfter && paperUIPanel != null)
        {
            paperUIPanel.SetActive(false);
        }

        activeAnim = null; // Kosongin track animasi biar bisa diklik lagi
    }
}