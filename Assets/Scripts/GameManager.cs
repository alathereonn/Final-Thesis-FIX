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
    public TextMeshProUGUI nightText;
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
        // PlayIntroDialogue();
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
            case 1: dialogMessage = "Two years stuck in this room... I need to let go of this perfectionism. It has to be tonight."; break;
            case 2: dialogMessage = "Why is my writing never good enough? The air in this room... it's getting suffocating."; break;
            case 3: dialogMessage = "Tari graduated six months ago. I really am left behind. Just me and this damn room."; break;
            case 4: dialogMessage = "Focus. Just ignore the noises. Mom and Dad are waiting for me to finish..."; break;
            case 5: dialogMessage = "I can't tell what's real anymore. The words on this screen, or whatever is standing in the corner?"; break;
            case 6: 
                dialogMessage = "The final night. It's either I finish this program, or I lose my mind completely."; 
                displayTime = 5f; 
                break;
            default: dialogMessage = "Back to work..."; displayTime = 3f; break;
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
            case 1: babTitleText.text = "CHAPTER I\nINTRODUCTION"; break;
            case 2: babTitleText.text = "CHAPTER II\nLITERATURE REVIEW"; break;
            case 3: babTitleText.text = "CHAPTER III\nRESEARCH METHODOLOGY"; break;
            case 4: babTitleText.text = "CHAPTER IV\nSYSTEM IMPLEMENTATION"; break;
            case 5: babTitleText.text = "CHAPTER V\nRESULTS AND TESTING"; break;
            case 6: babTitleText.text = "CHAPTER VI\nCONCLUSION AND RECOMMENDATIONS"; break;
            default: babTitleText.text = "THESIS DRAFT\nFINAL REVISION V2"; break;
        }
    }

    void SendStartNightSMS()
    {
        if (PhoneManager.instance == null) return;

        switch (currentNight)
        {
            case 1: PhoneManager.instance.ReceiveSMS("Mom", "How are you doing? Your father and I are always praying for you. Don't push yourself too hard, and make sure you eat."); break;
            case 2: PhoneManager.instance.ReceiveSMS("Tari", "Hey, are you still at your place? We haven't talked since my graduation. Let me know if you need help with Chapter 2."); break;
            case 3: PhoneManager.instance.ReceiveSMS("Thesis Advisor", "This is your final warning. If the draft isn't on my desk by this week, you are facing a potential drop-out."); break;
            case 4: PhoneManager.instance.ReceiveSMS("Mom", "Why aren't you replying to my messages? The neighbors keep asking when you're graduating... I just smile at them. Keep going, son."); break;
            case 5: PhoneManager.instance.ReceiveSMS("Tari", "Iko... are you really still in that room? Please open the door. You're not alone in there."); break;
            case 6: PhoneManager.instance.ReceiveSMS("Unknown", "THE PRESSURE WILL CRUSH YOU. WE WILL FINISH YOU. DO NOT LOOK BACK."); break;
            default: PhoneManager.instance.ReceiveSMS("System", "Good luck with your thesis."); break;
        }
    }

    // --- FUNGSI BARU: BUAT NGIRIM TEKS KERTAS ---
    public string GetPaperText()
    {
        switch (currentNight)
        {
            case 1: return "To-do List: Finish Chapter 1. I've rewritten this paragraph dozens of times and it still feels like garbage. This boarding house feels so dead since Tari graduated six months ago. We promised to graduate together... whatever. I have to finish this tonight.";
            case 2: return "Chapter 2 is a dead end. Are my standards too high or is my brain just rotting? Mom's text this morning made my chest tight. They don't force me, but their silent expectations are suffocating. By the way... I swear I saw a shadow moving in the corner of my eye while staring at the monitor.";
            case 3: return "Tari texted me again. I feel pathetic. She's already working, and I'm still rotting in this room. Something is wrong with this place. The things on my desk keep shifting, and I know I didn't move them. Please tell me it's just sleep deprivation.";
            case 4: return "My advisor threatened me with a drop-out! I can't fail. But how am I supposed to focus when the whispers from the bathroom are getting louder?! This isn't just academic stress anymore. Something was born from my own fear and expectations, and now it's watching me.";
            case 5: return "The line between reality and illusion is gone. That text from Tari earlier... I know for a fact it wasn't her. This room has become a prison. That figure... it's not a ghost. It's the manifestation of my own failures. And it's hungry.";
            case 6: return "CHAPTER 6: CONCLUSION.\n\nThey won't let me leave.\nThe pressure is alive. The expectations are suffocating.\n\nTHE ONLY WAY OUT IS TO FINISH THIS.\nKEEP YOUR EYES ON THE SCREEN.\nDO. NOT. LOOK. BACK.";
            default: return "Blank note...";
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

        PlayIntroDialogue();
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
        if (nightText != null) nightText.text = "Night " + currentNight;
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