using UnityEngine;
using TMPro; // Buat ngontrol teks UI
using UnityEngine.UI; // Buat ngontrol Slider
using System.Collections.Generic; // Buat bikin List kata

public class LaptopManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI targetWordText;
    public Slider progressBar;

    [Header("UI Canvas")]
    public GameObject winCanvas; 
    public GameObject gameOverCanvas;

    [Header("Gameplay Settings")]
    public string fileName = "kamus_sempro"; // Nama file txt lu tanpa .txt
    [Range(0, 100)]
    public float progressPerWord = 5f; // Tiap ngetik 1 kata bener, nambah berapa %?

    // Variabel internal (ga kelihatan di Unity Inspector)
    private List<string> wordList = new List<string>();
    private string currentWord = "";
    private string typedWord = "";
    public float currentProgress = 0f;

    void Start()
    {
        LoadWordsFromFile();
        progressBar.value = 0f;
        SetNewRandomWord();
    }

    void Update()
    {
        // Fitur ini otomatis nangkep apapun huruf yang ditekan player di keyboard
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Kalo nekan Backspace (buat hapus/opsional)
            {
                if (typedWord.Length > 0)
                {
                    typedWord = typedWord.Substring(0, typedWord.Length - 1);
                }
            }
            else if (c == '\n' || c == '\r') // Kalo nekan Enter (abaikan)
            {
                continue;
            }
            else // Kalo ngetik huruf biasa
            {
                // Kita ubah semua inputan jadi Huruf Besar (Kapital) biar cocok sama notepad
                typedWord += char.ToUpper(c); 
            }

            CheckInput();
        }
    }

    // FUNGSI 1: BACA FILE NOTEPAD
    void LoadWordsFromFile()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        
        if (textAsset != null)
        {
            // Pecah isi file berdasarkan baris baru (Enter), masukin ke List
            string[] words = textAsset.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            wordList = new List<string>(words);
            Debug.Log($"[LAPTOP] Berhasil memuat {wordList.Count} kata dari file kamus_sempro.txt!");
        }
        else
        {
            Debug.LogError($"[ERROR] File {fileName}.txt GAK KETEMU! Pastiin ada di folder Resources.");
        }
    }

    // FUNGSI 2: NGELUARIN KATA BARU
    void SetNewRandomWord()
    {
        if (wordList.Count > 0)
        {
            // Ambil kata random dari List
            int randomIndex = Random.Range(0, wordList.Count);
            currentWord = wordList[randomIndex];
            typedWord = ""; // Reset ketikan player buat kata baru

            UpdateWordDisplay();
        }
    }

    // FUNGSI 3: NGECEK KETIKAN & NAMBAH PROGRESS
    void CheckInput()
    {
        // 1. Cek apakah huruf yang diketik sejauh ini BENAR?
        if (currentWord.StartsWith(typedWord))
        {
            // Update warna di layar
            UpdateWordDisplay();

            // 2. Cek apakah KATA SUDAH SELESAI SEMUA?
            if (typedWord == currentWord)
            {
                Debug.Log($"[LAPTOP] Kata '{currentWord}' SELESAI!");
                
                // Tambah progress skripsi
                currentProgress += progressPerWord;
                progressBar.value = currentProgress / 100f; // Slider butuh angka 0 sampai 1

                if (currentProgress >= 100f)
                {
                    Debug.Log("[LAPTOP] PROGRESS 100%! SKRIPSI KELAR!");
                    // TODO: Panggil fungsi menang/tidur di sini nanti
                }
                else
                {
                    // Lanjut kasih kata baru
                    SetNewRandomWord();
                }
            }
        }
        else
        {
            // Kalau SALAH KETIK, hukum player dengan nge-reset ketikannya dari awal kata!
            typedWord = "";
            UpdateWordDisplay();
        }
    }

    // FUNGSI 4: NGEWARNAIN HURUF (Hijau = Bener, Putih = Belum diketik)
    void UpdateWordDisplay()
    {
        string coloredText = "";

        // Bagian huruf yang udah diketik (Kasih warna HIJAU)
        coloredText += "<color=#00FF00>" + currentWord.Substring(0, typedWord.Length) + "</color>";
        
        // Bagian sisa huruf yang belum diketik (Tetap warna bawaan/PUTIH)
        coloredText += currentWord.Substring(typedWord.Length);

        // Tampilkan ke layar UI
        targetWordText.text = coloredText;
    }
}