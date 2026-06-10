using UnityEngine;
using UnityEngine.EventSystems; // Wajib ditambahin biar Unity tahu urusan mouse

// Tambahin antarmuka IPointerEnterHandler (buat Hover) dan IPointerClickHandler (buat Klik)
public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Audio Setup")]
    public AudioSource sfxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Fungsi ini otomatis kepanggil 1x pas mouse masuk/nyentuh area tombol
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sfxSource != null && hoverSound != null)
        {
            sfxSource.PlayOneShot(hoverSound); // PlayOneShot biar suaranya bisa numpuk & ga motong lagu
        }
    }

    // Fungsi ini otomatis kepanggil pas lu klik kiri di tombolnya
    public void OnPointerClick(PointerEventData eventData)
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }
}