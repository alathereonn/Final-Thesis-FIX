using System.Collections;
using System.Collections.Generic; // KABEL BARU BUAT SISTEM NGACAK
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

// 1. DATA STRUCTURE
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
    public TextMeshProUGUI babTitleText; // <--- TAMBAHKAN KABEL INI

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
    
    // SAKLAR WAKTU BARU
    private bool is2AMTriggered = false; 
    private bool is3AMTriggered = false;

    void Start()
    {
        StartCoroutine(PlayIntroRoutine());
        PlayIntroDialogue();
        // MonologueManager.instance.ShowMonologue("Hadeh... revisian TA ini nggak kelar-kelar, mana deadline besok.", 4f);
        UpdateBabTitle();
        ApplyNightSettings(); 
        UpdateTimeUI();
        Debug.Log($"GAME STARTED: Night {currentNight} - 12 AM");
    }

    // --- FUNGSI BARU BUAT DIALOG TIAP MALAM ---
    void PlayIntroDialogue()
    {
        string dialogMessage = "";
        float displayTime = 4f;

        // Tentukan teks berdasarkan malam ke berapa
        switch (currentNight)
        {
            case 1:
                dialogMessage = "Hadeh... revisian TA ini nggak kelar-kelar, mana deadline besok.";
                break;
            case 2:
                dialogMessage = "Semalam perasaan ada yang aneh... Ah udahlah, mending fokus ngetik Bab 2.";
                break;
            case 3:
                dialogMessage = "Udah malam ketiga... Kenapa hawanya di kamar ini makin ga enak ya?";
                break;
            case 4:
                dialogMessage = "Bodo amat sama suara-suara itu, kalau TA ga kelar gue bisa DO!";
                break;
            case 5:
                dialogMessage = "Tinggal dikit lagi... Gue ga boleh mati konyol sebelum sidang!";
                break;
            case 6:
                dialogMessage = "Malam penentuan... Selesaiin program ini sekarang, atau terjebak selamanya!";
                displayTime = 5f; // Kasih waktu lebih lama dikit buat dibaca karena panjang
                break;
            default:
                dialogMessage = "Lanjut nugas..."; // Jaga-jaga kalau ada bug nyasar ke malam 7
                displayTime = 3f;
                break;
        }

        // Tampilkan ke layar pakai sistem Singleton yang udah kita buat
        if (MonologueManager.instance != null)
        {
            MonologueManager.instance.ShowMonologue(dialogMessage, displayTime);
        }
    }

    void UpdateBabTitle()
    {
        // Kalau kabelnya lupa dipasang di Inspector, biar nggak error
        if (babTitleText == null) return; 

        switch (currentNight)
        {
            case 1:
                babTitleText.text = "BAB I\nPENDAHULUAN";
                break;
            case 2:
                babTitleText.text = "BAB II\nTINJAUAN PUSTAKA";
                break;
            case 3:
                babTitleText.text = "BAB III\nMETODOLOGI PENELITIAN";
                break;
            case 4:
                babTitleText.text = "BAB IV\nIMPLEMENTASI SISTEM";
                break;
            case 5:
                babTitleText.text = "BAB V\nHASIL DAN UJI COBA";
                break;
            case 6:
                babTitleText.text = "BAB VI\nKESIMPULAN DAN SARAN"; 
                // Malam bos terakhir, selesain kesimpulan!
                break;
            default:
                babTitleText.text = "DRAFT TA\nREVISI FINAL V2";
                break;
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

        // --- SISTEM BUFF AI DINAMIS (JAM 2 & JAM 3 PAGI) ---

        // Cek Jam 2 Pagi
        if (currentHour == 2 && !is2AMTriggered)
        {
            is2AMTriggered = true;
            if (currentNight == 1)
            {
                IncreaseRandomMonsters(2, 1); 
                Debug.Log("[DANGER] 2 AM (Night 1)! 2 Random Monsters got +1 AI!");
            }
        }

        // Cek Jam 3 Pagi
        if (currentHour == 3 && !is3AMTriggered)
        {
            is3AMTriggered = true;
            if (currentNight == 1)
            {
                IncreaseAllMonsters(2); 
                Debug.Log("[DANGER] 3 AM (Night 1)! ALL Monsters got +2 AI!");
            }
            else if (currentNight >= 2 && currentNight <= 4)
            {
                IncreaseRandomMonsters(2, 2); 
                Debug.Log($"[DANGER] 3 AM (Night {currentNight})! 2 Random Monsters got +2 AI!");
            }
            // Night 5 dan 6 diabaikan sesuai request lu
        }

        if (currentHour == 6)
        {
            if (doorMonster != null) doorMonster.StopAllCoroutines(); 
            if (windowMonster != null) windowMonster.StopAllCoroutines(); 
            if (toiletMonster != null) toiletMonster.StopAllCoroutines(); 

            if (requireFullProgress) 
            {
                if (laptopManager.currentProgress < 100f)
                {
                    Debug.Log("Player did not reach 100% Progress! NT!");
                    GameOver(); 
                }
                else 
                {
                    Debug.Log("Player reached 100% Progress! WIN!");
                    WinGame(); 
                }
            }
            else 
            {
                Debug.Log("🎉 6 AM! PLAYER SURVIVED!");                
                WinGame(); 
            }
        }
    }

    // --- FUNGSI PEMBANTU BUAT NAIKIN LEVEL AI ---

    void IncreaseAllMonsters(int amount)
    {
        if (doorMonster != null) doorMonster.aiLevel += amount;
        if (windowMonster != null) windowMonster.aiLevel += amount;
        if (toiletMonster != null) toiletMonster.aiLevel += amount;
    }

    void IncreaseRandomMonsters(int count, int amount)
    {
        // 1. Masukin semua monster yang aktif ke dalam List
        List<MonsterAI> activeMonsters = new List<MonsterAI>();
        if (doorMonster != null) activeMonsters.Add(doorMonster);
        if (windowMonster != null) activeMonsters.Add(windowMonster);
        if (toiletMonster != null) activeMonsters.Add(toiletMonster);

        // 2. Kalau jumlah slot monster kurang dari target ngacaknya, naikin aja semua
        if (activeMonsters.Count <= count)
        {
            foreach (var m in activeMonsters) m.aiLevel += amount;
            return;
        }

        // 3. Sistem Cabut Undian!
        for (int i = 0; i < count; i++)
        {
            // Pilih satu index acak dari sisa monster yang ada di List
            int randIndex = Random.Range(0, activeMonsters.Count);
            
            // Kasih buff ke monster terpilih
            activeMonsters[randIndex].aiLevel += amount;
            
            // Hapus monster itu dari List biar ga terpilih 2 kali
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

            Debug.Log($"[NIGHT {currentNight}] AI Levels applied from GameManager!");
        }
    }

    void WinGame()
    {
        if (winCanvas != null) winCanvas.SetActive(true);
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fpsController != null) fpsController.enabled = false;
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
        if (timeText != null)
        {
            timeText.text = currentHour + " AM";
        }
    }

    public void RestartGame()
    {
        Debug.Log("RESTARTING GAME...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}