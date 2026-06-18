using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [Header("Gallery Setup")]
    [Tooltip("ID ini HARUS sama persis dengan ID yang kamu daftarkan di GalleryManager!")]
    public string itemID; 

    [Header("Pengaturan Ganti Gambar")]
    [Tooltip("Masukkan material kertas kosong ke sini")]
    public Material blankMaterial; 
    private MeshRenderer meshRenderer;

    [Header("Jarak Interaksi (Mirip Kertas)")]
    public Transform playerTransform; 
    public float maxInteractDistance = 3f; 

    [Header("Efek Tambahan (Opsional)")]
    public AudioClip pickupSound; // Efek suara pas barang diambil

    void Start()
    {
        // Ambil komponen MeshRenderer dari objek Quad ini
        meshRenderer = GetComponent<MeshRenderer>();

        // Cek apakah item ini sudah pernah diambil sebelumnya (di hari kemarin)
        if (PlayerPrefs.GetInt(itemID, 0) == 1)
        {
            // Karena sudah masuk Gallery, next day tidak perlu muncul lagi
            //gameObject.SetActive(false);
            if (blankMaterial != null && meshRenderer != null)
            {
                meshRenderer.material = blankMaterial;
            }

            // Collider langsung dimatikan agar tidak bisa diklik lagi
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            /* * CATATAN: Kalau abang ingin besoknya quad ini tetap ada tapi gambarnya kosong, 
             * hapus baris 'gameObject.SetActive(false);' di atas, lalu pakai 2 baris ini:
             * * meshRenderer.material = blankMaterial;
             * GetComponent<Collider>().enabled = false;
             */
        }
    }

    void OnMouseDown()
    {
        if (playerTransform == null) return; 

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= maxInteractDistance)
        {
            AmbilKoleksi();
        }
    }

    void AmbilKoleksi()
    {
        // 1. Simpan data ke Gallery
        PlayerPrefs.SetInt(itemID, 1);

        string kunciBarang = "Koleksi_01"; // Sesuaikan dengan nama ID barang abang
        PlayerPrefs.SetInt(kunciBarang, 1);

        string waktuSekarang = System.DateTime.Now.ToString("dd MMM yyyy - HH:mm");
        PlayerPrefs.SetString(kunciBarang + "_Date", waktuSekarang);

        PlayerPrefs.Save();
        Debug.Log("Sukses! " + itemID + " sekarang sudah masuk Gallery.");

        // 2. Play efek suara kalau ada
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // 3. Ganti material gambar asli jadi material kosongan
        if (blankMaterial != null && meshRenderer != null)
        {
            meshRenderer.material = blankMaterial;
        }

        // 4. Matikan collider supaya tidak bisa diklik berkali-kali di hari yang sama
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
}