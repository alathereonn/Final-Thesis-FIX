using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    [Header("Pengaturan Scroll")]
    public float scrollSpeed = 50f; 
    
    [Tooltip("Batas maksimal posisi Y agar teks berhenti menggeser")]
    public float stopPositionY = 1000f; 

    private RectTransform rectTransform;
    private bool isScrolling = true; 

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isScrolling)
        {
            // === FITUR BARU: SKIP KE AKHIR KALAU DIKLIK ===
            // GetMouseButtonDown(0) mendeteksi klik kiri mouse di mana saja
            if (Input.GetMouseButtonDown(0))
            {
                // Langsung teleportasi posisi Y ke titik berhenti
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, stopPositionY);
                isScrolling = false; // Matikan mesin scroll
                return; // Setop baca kode di bawahnya untuk frame ini
            }
            // ===============================================

            // Gerakan merayap normal kalau tidak diklik
            rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

            // Cek apakah posisi Y saat ini sudah melewati batas otomatis
            if (rectTransform.anchoredPosition.y >= stopPositionY)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, stopPositionY);
                isScrolling = false; 
            }
        }
    }
}