using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

// Class ini buat nyimpan data masing-masing item di Inspector
[System.Serializable]
public class GalleryItem
{
    public string itemID;          // ID unik, misal: "Item_Pedang", "Item_Batu"
    public Sprite unlockedSprite;  // Gambar kalau item udah didapat
    public Sprite lockedSprite;    // Gambar siluet/gembok kalau belum didapat
    [TextArea(3, 5)]
    public string description;
}

public class GalleryManager : MonoBehaviour
{
    [Header("Gallery Settings")]
    public GalleryItem[] allItems;       // Daftar semua item collectible
    
    [Header("UI References")]
    public GameObject itemSlotPrefab;    // Masukkan prefab ItemSlot ke sini
    public Transform gridContainer;      // Masukkan panel GridContainer ke sini

    [Header("Popup References")]
    public GameObject popupPanel;
    public Image popupImage;
    public TextMeshProUGUI popupDescription;
    public TextMeshProUGUI popupDate;

    void Start()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
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
            Button slotButton = newSlot.GetComponent<Button>();

            // 3. Cek di PlayerPrefs, apakah item ini sudah diambil? (1 = udah, 0 = belum)
            // Defaultnya 0 (belum kebuka)
            int isUnlocked = PlayerPrefs.GetInt(item.itemID, 0);

            // 4. Ganti gambar sesuai statusnya
            if (isUnlocked == 1)
            {
                slotImage.sprite = item.unlockedSprite;
                if (slotButton != null)
                {
                    slotButton.interactable = true; // Make it clickable
                    
                    // Fetch the date from PlayerPrefs
                    string dateCollected = PlayerPrefs.GetString(item.itemID + "_Date", "Unknown Date");
                    
                    // Automatically wire the button to open the popup with correct data
                    slotButton.onClick.AddListener(() => OpenPopup(item.unlockedSprite, item.description, dateCollected));
                }
            }
            else
            {
                slotImage.sprite = item.lockedSprite;
                if (slotButton != null) slotButton.interactable = false; // Disable click if locked
            }
        }
    }

    public void OpenPopup(Sprite image, string desc, string date)
    {
        if (popupImage != null) popupImage.sprite = image;
        if (popupDescription != null) popupDescription.text = desc;
        if (popupDate != null) popupDate.text = "Discovered on:\n" + date;
        
        if (popupPanel != null) popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Pastikan namanya sama persis dengan scene utama
    }
}