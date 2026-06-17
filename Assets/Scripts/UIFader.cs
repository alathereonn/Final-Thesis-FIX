using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFader : MonoBehaviour
{
    [Header("Pengaturan Transisi")]
    [Tooltip("Berapa lama layar diam warna hitam pekat sebelum mulai memudar")]
    public float waktuTunggu = 1.5f; // <--- FITUR JEDA BARU

    [Tooltip("Berapa detik waktu yang dibutuhkan dari hitam ke transparan")]
    public float durasiFade = 2f; 

    private Image layarHitam;

    void Awake()
    {
        // Ambil komponen Image (tirai hitamnya)
        layarHitam = GetComponent<Image>();
    }

    void OnEnable()
    {
        // 1. Pastikan saat baru muncul, layarnya 100% hitam pekat
        Color warnaAwal = layarHitam.color;
        warnaAwal.a = 1f; // Alpha 1 = Solid
        layarHitam.color = warnaAwal;

        // 2. Mulai proses Coroutine
        StartCoroutine(MulaiFadeOut());
    }

    IEnumerator MulaiFadeOut()
    {
        // --- INI KUNCI JEDANYA ---
        // Layar akan ditahan dalam kondisi hitam pekat selama 'waktuTunggu'
        // Kita pakai Realtime agar jedanya tidak rusak kalau game sedang di-pause
        yield return new WaitForSecondsRealtime(waktuTunggu);
        // -------------------------

        float waktuBerjalan = 0f;
        Color warnaTirai = layarHitam.color;

        // 3. Mulai proses perlahan memudarkan layar
        while (waktuBerjalan < durasiFade)
        {
            // Pakai unscaledDeltaTime biar aman dari jeda waktu
            waktuBerjalan += Time.unscaledDeltaTime;
            
            // Hitung mundur nilai Alpha dari 1 (Hitam) menuju 0 (Tembus Pandang)
            warnaTirai.a = 1f - Mathf.Clamp01(waktuBerjalan / durasiFade);
            layarHitam.color = warnaTirai;

            yield return null; // Tunggu frame selanjutnya
        }

        // Pastikan di akhir transisi, layarnya benar-benar 100% tembus pandang (0)
        warnaTirai.a = 0f;
        layarHitam.color = warnaTirai;
    }
}