using UnityEngine;
using UnityEngine.UI; // <--- WAJIB ADA BIAR BISA NGE-HACK GAMBARNYA

public class CrosshairController : MonoBehaviour
{
    void Start()
    {
        // Baca buku catatan PlayerPrefs
        int isCrosshairOn = PlayerPrefs.GetInt("CrosshairEnabled", 1);
        
        Debug.Log("CCTV CROSSHAIR BACA DATA: " + isCrosshairOn);

        // Cari komponen gambar (Image) di objek ini
        Image crosshairImg = GetComponent<Image>();

        if (crosshairImg != null)
        {
            if (isCrosshairOn == 1)
            {
                crosshairImg.enabled = true;  // Munculkan gambarnya
                Debug.Log("Crosshair Tampil!");
            }
            else
            {
                crosshairImg.enabled = false; // Bikin gambarnya tembus pandang
                Debug.Log("Crosshair Disembunyikan!");
            }
        }
        else
        {
            Debug.LogWarning("GAGAL: Objek ini gak punya komponen Image! Pastikan script ditaruh di objek UI Image Crosshair.");
        }
    }
}