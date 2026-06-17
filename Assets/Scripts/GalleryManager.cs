using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Class ini buat nyimpan data masing-masing item di Inspector
[System.Serializable]
public class GalleryItem
{
    public string itemID;          // ID unik, misal: "Item_Pedang", "Item_Batu"
    public Sprite unlockedSprite;  // Gambar kalau item udah didapat
    public Sprite lockedSprite;    // Gambar siluet/gembok kalau belum didapat
}

public class GalleryManager : MonoBehaviour
{
    [Header("Gallery Settings")]
    public GalleryItem[] allItems;       // Daftar semua item collectible
    
    [Header("UI References")]
    public GameObject itemSlotPrefab;    // Masukkan prefab ItemSlot ke sini
    public Transform gridContainer;      // Masukkan panel GridContainer ke sini

    void Start()
    {
        RefreshGallery();
    }

    void RefreshGallery()
    {
        // Looping untuk memunculkan setiap item yang ada di daftar
        foreach (GalleryItem item in allItems)
        {
            // 1. Munculkan kotak UI baru di dalam GridContainer
            GameObject newSlot = Instantiate(itemSlotPrefab, gridContainer);
            
            // 2. Ambil komponen Image dari kotak UI tersebut
            Image slotImage = newSlot.GetComponent<Image>();

            // 3. Cek di PlayerPrefs, apakah item ini sudah diambil? (1 = udah, 0 = belum)
            // Defaultnya 0 (belum kebuka)
            int isUnlocked = PlayerPrefs.GetInt(item.itemID, 0);

            // 4. Ganti gambar sesuai statusnya
            if (isUnlocked == 1)
            {
                slotImage.sprite = item.unlockedSprite;
            }
            else
            {
                slotImage.sprite = item.lockedSprite;
            }
        }
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Pastikan namanya sama persis dengan scene utama
    }
}