using UnityEngine;
using TMPro;
using UnityEngine.UI; // <--- INI KUNCI JAWABANNYA!
using System.Collections; // Wajib dipanggil buat Coroutine animasi

public class PhoneManager : MonoBehaviour
{
    public static PhoneManager instance;

    [Header("UI References")]
    public GameObject phoneUI; 
    public TextMeshProUGUI senderText;
    public TextMeshProUGUI messageText;
    public GameObject instructionUI; 

    [Header("Animation Settings")]
    public float slideDuration = 0.3f; // Kecepatan HP naik/turun
    public Vector2 hiddenPosition = new Vector2(0, -600f); // Posisi Y saat HP ngumpet di bawah
    public Vector2 visiblePosition = new Vector2(0, 0f);   // Posisi Y saat HP nampil di layar

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip notifSound;
    [HideInInspector]
    public bool canUsePhone = true;

    private bool isPhoneActive = false;
    private RectTransform phoneRect;
    private Coroutine activeAnim;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        canUsePhone = true; 
        activeAnim = null;
        // Ambil komponen RectTransform dari HP abang buat digerak-gerakin
        if (phoneUI != null)
        {
            phoneRect = phoneUI.GetComponent<RectTransform>();
            phoneRect.anchoredPosition = hiddenPosition; // Posisikan di bawah layar saat mulai
            phoneUI.SetActive(false); 
        }

        if (instructionUI != null) instructionUI.SetActive(false);
    }

    void Update()
    {
        // Buka tutup HP pakai tombol TAB (sesuai request abang)
        if (canUsePhone && Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePhone();
        }
    }

    public void ReceiveSMS(string sender, string message)
    {
        if (senderText != null) senderText.text = sender;
        if (messageText != null) messageText.text = message;
        
        // Paksa Layout-nya update sekarang juga!
        Canvas.ForceUpdateCanvases(); 

        if (audioSource != null && notifSound != null)
        {
            audioSource.PlayOneShot(notifSound);
        }

        if (instructionUI != null && !isPhoneActive)
        {
            instructionUI.SetActive(true);
        }
    }

    void TogglePhone()
    {
        if (activeAnim != null) return; 

        isPhoneActive = !isPhoneActive;

        if (isPhoneActive)
        {
            // --- BARIS INI YANG BERTUGAS MEMATIKAN TEKSNYA ---
            if (instructionUI != null) instructionUI.SetActive(false);
            
            phoneUI.SetActive(true);
            activeAnim = StartCoroutine(SlidePhoneAnim(visiblePosition, false));
        }
        else
        {
            activeAnim = StartCoroutine(SlidePhoneAnim(hiddenPosition, true));
        }
    }

    IEnumerator SlidePhoneAnim(Vector2 targetPos, bool disableAfter)
    {
        Vector2 startPos = phoneRect.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            // Pake SmoothStep biar gerakannya ga kaku (ada ngeremnya)
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / slideDuration);
            
            phoneRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Pastikan posisi pas di titik target saat animasi selesai
        phoneRect.anchoredPosition = targetPos;

        if (disableAfter && phoneUI != null)
        {
            phoneUI.SetActive(false);
        }

        activeAnim = null; // Kosongkan coroutine biar bisa dipencet lagi
    }

    // Fungsi untuk menyembunyikan dan mengunci HP paksa
    public void ForceHidePhone()
    {
        canUsePhone = false; // Kunci tombol Tab

        if (isPhoneActive)
        {
            isPhoneActive = false;
            if (activeAnim != null) StopCoroutine(activeAnim); // Stop animasi kalau lagi jalan
            
            if (phoneUI != null) phoneUI.SetActive(false);
            if (phoneRect != null) phoneRect.anchoredPosition = hiddenPosition; // Kembalikan ke bawah
        }

        if (instructionUI != null) instructionUI.SetActive(false); // Sembunyikan teks "Tekan E"
    }

    // Fungsi untuk membuka kuncian HP lagi
    public void AllowPhoneUsage()
    {
        canUsePhone = true; 
    }
}