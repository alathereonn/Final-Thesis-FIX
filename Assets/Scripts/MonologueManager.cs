using UnityEngine;
using TMPro;
using System.Collections;

public class MonologueManager : MonoBehaviour
{
    // Bikin sistem Singleton biar gampang dipanggil dari script lain
    public static MonologueManager instance; 

    [Header("UI References")]
    public TextMeshProUGUI subtitleText;

    [Header("Settings")]
    public float defaultDisplayTime = 3f; // Waktu standar teks muncul (detik)

    private Coroutine activeMonologue;

    void Awake()
    {
        // Setup Singleton
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (subtitleText != null) subtitleText.text = ""; // Bersihkan layar di awal
    }

    // Fungsi utama yang bakal dipanggil buat memunculkan teks
    public void ShowMonologue(string message, float duration = 0f)
    {
        // Kalau masih ada monolog lama yang tayang, stop dulu biar ga numpuk
        if (activeMonologue != null)
        {
            StopCoroutine(activeMonologue);
        }

        // Kalau durasi ga diisi, pakai durasi standar
        float timeToDisplay = (duration > 0f) ? duration : defaultDisplayTime;
        
        activeMonologue = StartCoroutine(DisplayRoutine(message, timeToDisplay));
    }

    IEnumerator DisplayRoutine(string message, float duration)
    {
        subtitleText.text = message;

        // BISA TAMBAHIN AUDIO DI SINI NANTI
        // Contoh: audioSource.PlayOneShot(mumbleSound);

        yield return new WaitForSeconds(duration);

        // Hapus teksnya setelah waktunya habis
        subtitleText.text = "";
    }
}