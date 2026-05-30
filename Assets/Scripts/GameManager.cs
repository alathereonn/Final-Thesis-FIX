using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

// 1. DATA STRUCTURE (Mesti di luar class GameManager biar kebaca Unity)
[System.Serializable]
public class NightSettings
{
    public int doorAiLevel;
    public int windowAiLevel;
    public int toiletAiLevel;
}

public class GameManager : MonoBehaviour
{
    [Header("Win Condition (UAS/Sempro)")]
    public bool requireFullProgress = true;  // Dulu: wajib100Persen
    public LaptopManager laptopManager; 

    [Header("UI Canvas")]
    public GameObject winCanvas; 
    public GameObject gameOverCanvas; 
    
    [Header("All Monsters (Employees)")]
    public MonsterAI doorMonster;
    public MonsterAI windowMonster;   // KABEL BARU
    public MonsterAI toiletMonster;   // KABEL BARU

    [Header("Night Configuration")]
    public int currentNight = 1;
    public NightSettings[] nightSettings = new NightSettings[6]; // ARRAY 6 MALAM

    [Header("Time Settings")]
    public TextMeshProUGUI timeText;
    public float realSecondsPerHour = 10f; 
    
    private int currentHour = 12; 
    private float hourTimer = 0f;
    private bool is3AMTriggered = false;

    void Start()
    {
        ApplyNightSettings(); // Terapkan level AI pas game mulai
        UpdateTimeUI();
        Debug.Log($"GAME STARTED: Night {currentNight} - 12 AM");
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

        // Jam 3 pagi: Setan pintu ngamuk dikit
        if (currentHour == 3 && !is3AMTriggered)
        {
            if (doorMonster != null) doorMonster.aiLevel += 3; 
            Debug.Log($"[DANGER] 3 AM! Door Monster AI Level increased!");
            is3AMTriggered = true;
        }

        if (currentHour == 6)
        {
            // STOP SEMUA SETAN BIAR GA ADA YANG NYERANG PAS MENANG
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

    // --- FUNGSI BARU: TERAPKAN AI LEVEL ---
    void ApplyNightSettings()
    {
        int index = currentNight - 1; // Array mulai dari 0

        // Cegah error kalau array belum diisi
        if (index >= 0 && index < nightSettings.Length)
        {
            if (doorMonster != null) doorMonster.aiLevel = nightSettings[index].doorAiLevel;
            if (windowMonster != null) windowMonster.aiLevel = nightSettings[index].windowAiLevel;
            if (toiletMonster != null) toiletMonster.aiLevel = nightSettings[index].toiletAiLevel;

            Debug.Log($"[NIGHT {currentNight}] AI Levels applied from GameManager!");
        }
    }

    // Dulu: MenangGame()
    void WinGame()
    {
        if (winCanvas != null) winCanvas.SetActive(true);
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GameOver()
    {
        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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