using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

[System.Serializable]
public class NightSettings
{
    public int doorAiLevel;
    public int windowAiLevel;
    public int toiletAiLevel;
}

public class GameManager : MonoBehaviour
{
    [Header("Intro Transition")]
    public GameObject introCanvas;
    public TextMeshProUGUI introText;

    [Header("Win Condition (UAS/Sempro)")]
    public bool requireFullProgress = true;  
    public LaptopManager laptopManager; 

    [Header("UI Canvas")]
    public GameObject winCanvas; 
    public GameObject gameOverCanvas;
    public GameObject goodEndingCanvas; // <--- KABEL BARU BUAT NIGHT 6
    public TextMeshProUGUI babTitleText; 

    [Header("Player")]
    public SC_FPSController fpsController;
    
    [Header("All Monsters (Employees)")]
    public MonsterAI doorMonster;
    public MonsterAI windowMonster;   
    public MonsterAI toiletMonster;   

    [Header("Night Configuration")]
    public int currentNight = 1;
    public NightSettings[] nightSettings = new NightSettings[6]; 

    [Header("Time Settings")]
    public TextMeshProUGUI timeText;
    public float realSecondsPerHour = 10f; 
    
    private int currentHour = 12; 
    private float hourTimer = 0f;
    
    private bool is2AMTriggered = false; 
    private bool is3AMTriggered = false;

    // --- FUNGSI BARU: BACA DATA SEBELUM START ---
    void Awake()
    {
        // Narik data dari Main Menu buat nentuin sekarang main malam ke berapa
        currentNight = PlayerPrefs.GetInt("CurrentPlayNight", 1);
    }

    void Start()
    {
        StartCoroutine(PlayIntroRoutine());
        PlayIntroDialogue();
        UpdateBabTitle();
        ApplyNightSettings(); 
        UpdateTimeUI();
        Debug.Log($"GAME STARTED: Night {currentNight} - 12 AM");
    }

    void PlayIntroDialogue()
    {
        string dialogMessage = "";
        float displayTime = 4f;

        switch (currentNight)
        {
            case 1: dialogMessage = "Hadeh... revisian TA ini nggak kelar-kelar, mana deadline besok."; break;
            case 2: dialogMessage = "Semalam perasaan ada yang aneh... Ah udahlah, mending fokus ngetik Bab 2."; break;
            case 3: dialogMessage = "Udah malam ketiga... Kenapa hawanya di kamar ini makin ga enak ya?"; break;
            case 4: dialogMessage = "Bodo amat sama suara-suara itu, kalau TA ga kelar gue bisa DO!"; break;
            case 5: dialogMessage = "Tinggal dikit lagi... Gue ga boleh mati konyol sebelum sidang!"; break;
            case 6: 
                dialogMessage = "Malam penentuan... Selesaiin program ini sekarang, atau terjebak selamanya!"; 
                displayTime = 5f; 
                break;
            default: dialogMessage = "Lanjut nugas..."; displayTime = 3f; break;
        }

        if (MonologueManager.instance != null)
        {
            MonologueManager.instance.ShowMonologue(dialogMessage, displayTime);
        }
    }

    void UpdateBabTitle()
    {
        if (babTitleText == null) return; 

        switch (currentNight)
        {
            case 1: babTitleText.text = "BAB I\nPENDAHULUAN"; break;
            case 2: babTitleText.text = "BAB II\nTINJAUAN PUSTAKA"; break;
            case 3: babTitleText.text = "BAB III\nMETODOLOGI PENELITIAN"; break;
            case 4: babTitleText.text = "BAB IV\nIMPLEMENTASI SISTEM"; break;
            case 5: babTitleText.text = "BAB V\nHASIL DAN UJI COBA"; break;
            case 6: babTitleText.text = "BAB VI\nKESIMPULAN DAN SARAN"; break;
            default: babTitleText.text = "DRAFT TA\nREVISI FINAL V2"; break;
        }
    }

    void SendStartNightSMS()
    {
        if (PhoneManager.instance == null) return;

        switch (currentNight)
        {
            case 1: PhoneManager.instance.ReceiveSMS("Raihandy", "Bro, ntar kalau Bab 1 udah kelar, langsung upload ke Drive kelompok ya. Ditunggu Ade Putri nih buat digabung."); break;
            case 2: PhoneManager.instance.ReceiveSMS("Andi", "Gila, dapet info dari kelas sebelah, katanya dosen penguji sempro besok killer banget. Lu mending kelarin malam ini bro."); break;
            case 3: PhoneManager.instance.ReceiveSMS("Ade Putri", "Andi sama Raihandy nanyain jobdesk backend-nya udah sampe mana? Tolong buruan di-push ke repisitori ya."); break;
            case 4: PhoneManager.instance.ReceiveSMS("Dosen Pembimbing", "Saya sudah cek bab metodologi kamu. Masih banyak yang kurang tepat, tolong perbaiki malam ini."); break;
            case 5: PhoneManager.instance.ReceiveSMS("Raihandy", "Bro, lu aman kan di kamar? Kok grup angkatan rame katanya ada yang aneh di sekitar kampus malam-malam gini."); break;
            case 6: PhoneManager.instance.ReceiveSMS("Nomor Tidak Dikenal", "MATIKAN LAPTOPNYA SEKARANG. JANGAN LIHAT KE BELAKANG."); break;
            default: PhoneManager.instance.ReceiveSMS("Sistem", "Selamat mengerjakan tugas akhir."); break;
        }
    }

    IEnumerator PlayIntroRoutine()
    {
        Time.timeScale = 0f; 
        
        if (introCanvas != null) introCanvas.SetActive(true);
        if (introText != null) introText.text = "NIGHT " + currentNight;

        yield return new WaitForSecondsRealtime(3f);

        if (introCanvas != null) introCanvas.SetActive(false);
        Time.timeScale = 1f;

        Debug.Log("[GAME] Intro selesai, selamat mengerjakan skripsi!");
        SendStartNightSMS();
    }

    void Update()
    {
        if (currentHour == 6) return;

        hourTimer += Time.deltaTime;
        
        if (hourTimer >= realSecondsPerHour)
        {
            hourTimer = 0f; 
            AdvanceHour();
        }
    }
    
    void AdvanceHour()
    {
        if (currentHour == 12) currentHour = 1;
        else currentHour++;

        UpdateTimeUI();
        Debug.Log($"TIME UPDATE: It is now {currentHour} AM");

        if (currentHour == 2 && !is2AMTriggered)
        {
            is2AMTriggered = true;
            if (currentNight == 1) IncreaseRandomMonsters(2, 1); 
        }

        if (currentHour == 3 && !is3AMTriggered)
        {
            is3AMTriggered = true;
            if (currentNight == 1) IncreaseAllMonsters(2); 
            else if (currentNight >= 2 && currentNight <= 4) IncreaseRandomMonsters(2, 2); 
        }

        if (currentHour == 6)
        {
            if (doorMonster != null) doorMonster.StopAllCoroutines(); 
            if (windowMonster != null) windowMonster.StopAllCoroutines(); 
            if (toiletMonster != null) toiletMonster.StopAllCoroutines(); 

            if (requireFullProgress) 
            {
                if (laptopManager.currentProgress < 100f) GameOver(); 
                else WinGame(); 
            }
            else 
            {
                WinGame(); 
            }
        }
    }

    void IncreaseAllMonsters(int amount)
    {
        if (doorMonster != null) doorMonster.aiLevel += amount;
        if (windowMonster != null) windowMonster.aiLevel += amount;
        if (toiletMonster != null) toiletMonster.aiLevel += amount;
    }

    void IncreaseRandomMonsters(int count, int amount)
    {
        List<MonsterAI> activeMonsters = new List<MonsterAI>();
        if (doorMonster != null) activeMonsters.Add(doorMonster);
        if (windowMonster != null) activeMonsters.Add(windowMonster);
        if (toiletMonster != null) activeMonsters.Add(toiletMonster);

        if (activeMonsters.Count <= count)
        {
            foreach (var m in activeMonsters) m.aiLevel += amount;
            return;
        }

        for (int i = 0; i < count; i++)
        {
            int randIndex = Random.Range(0, activeMonsters.Count);
            activeMonsters[randIndex].aiLevel += amount;
            activeMonsters.RemoveAt(randIndex); 
        }
    }

    void ApplyNightSettings()
    {
        int index = currentNight - 1; 

        if (index >= 0 && index < nightSettings.Length)
        {
            if (doorMonster != null) doorMonster.aiLevel = nightSettings[index].doorAiLevel;
            if (windowMonster != null) windowMonster.aiLevel = nightSettings[index].windowAiLevel;
            if (toiletMonster != null) toiletMonster.aiLevel = nightSettings[index].toiletAiLevel;
        }
    }

    // --- FUNGSI WIN GAME YANG DI-UPGRADE ---
    void WinGame()
    {
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (fpsController != null) fpsController.enabled = false;

        if (currentNight < 6)
        {
            // MENANG MALAM BIASA (1-5)
            if (winCanvas != null) winCanvas.SetActive(true);
            
            // Catet prestasi di buku memori
            int highestNight = PlayerPrefs.GetInt("HighestNight", 1);
            if (currentNight + 1 > highestNight)
            {
                PlayerPrefs.SetInt("HighestNight", currentNight + 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            // TAMAT BOS! (MALAM 6)
            if (goodEndingCanvas != null) goodEndingCanvas.SetActive(true);
            // Gak usah nge-save plus satu lagi, biar mentok di 6
        }
    }

    public void GameOver()
    {
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (fpsController != null) fpsController.enabled = false;
    }

    void UpdateTimeUI()
    {
        if (timeText != null) timeText.text = currentHour + " AM";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- TOMBOL-TOMBOL NAVIGASI BARU ---

    // Dicolok ke tombol "Next Night" di Win Canvas
    public void NextNight()
    {
        Time.timeScale = 1f;
        
        // Atur agar ronde selanjutnya mainin malam berikutnya
        PlayerPrefs.SetInt("CurrentPlayNight", currentNight + 1);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Dicolok ke tombol "Back to Menu" di Win/GameOver/GoodEnding Canvas
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        // Ganti "MainMenu" pakai nama scene menu awal lu nanti
        SceneManager.LoadScene("MainMenu"); 
    }
}