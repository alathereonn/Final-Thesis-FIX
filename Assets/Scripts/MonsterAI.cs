using System.Collections;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("AI Settings")]
    public string monsterName = "Room Door Monster";
    public float moveInterval = 4f; 
    
    // Disembunyiin dari Inspector karena angkanya diisi otomatis sama GameManager
    [HideInInspector]
    public int aiLevel = 1; 

    // public int currentNight = 1; <--- UDAH DIHAPUS TOTAL

    [Header("Current Status (Read Only)")]
    public int currentStage = 0;
    public int maxStage = 3;

    [Header("Jumpscare References")]
    public GameObject jumpscareCanvas; 
    public GameManager gameManager;    
    public GameObject peekCanvas;      
    public SC_FPSController fpsController; 

    [Header("Audio Settings")]
    public AudioSource audioSource;    
    public AudioClip stepSound;        
    public AudioClip atDoorSound;      

    private Coroutine aiMovementCoroutine;
    private Coroutine jumpscareTimerCoroutine; 

    void Start()
    {
        aiMovementCoroutine = StartCoroutine(ProcessMovementRNG());
    }

    IEnumerator ProcessMovementRNG()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            if (currentStage < maxStage)
            {
                int diceRoll = Random.Range(1, 21);
                if (diceRoll <= aiLevel)
                {
                    AdvanceStage();
                }
            }
        }
    }

    void AdvanceStage()
    {
        currentStage++;
        Debug.Log($"[TENSION] {monsterName} advanced to Stage {currentStage}!");

        if (audioSource != null && stepSound != null)
        {
            audioSource.PlayOneShot(stepSound);
        }

        if (currentStage == maxStage)
        {
            Debug.Log($"[WARNING] {monsterName} IS AT THE DOOR! STARTING JUMPSCARE TIMER!");
            
            if (audioSource != null && atDoorSound != null)
            {
                audioSource.PlayOneShot(atDoorSound);
            }

            jumpscareTimerCoroutine = StartCoroutine(JumpscareCountdown());
        }
    }

    IEnumerator JumpscareCountdown()
    {
        // --------------------------------------------------------
        // RUMUS BARU: SEKARANG NANYA KE BOS INI MALAM KE BERAPA!
        // --------------------------------------------------------
        float timeLimit = 20f - gameManager.currentNight;
        float timer = 0f;

        Debug.Log($"[TIMER] Player punya waktu {timeLimit} detik buat ngecek pintu!");

        while (timer < timeLimit)
        {
            timer += Time.deltaTime;
            yield return null; 
        }

        TriggerJumpscare();
    }

    public void RepelMonster()
    {
        currentStage = 0;
        Debug.Log($"[SAFE] {monsterName} has been repelled to Stage 0.");

        if (jumpscareTimerCoroutine != null)
        {
            StopCoroutine(jumpscareTimerCoroutine);
            jumpscareTimerCoroutine = null;
            Debug.Log("[SAFE] Bom waktu berhasil dijinakkan!");
        }
    }

    void TriggerJumpscare()
    {
        Debug.Log($"[GAME OVER] KENA JUMPSCARE SAMA {monsterName}!");

        Time.timeScale = 0f;

        StopAllCoroutines(); 
        if (fpsController != null) fpsController.canMove = false;
        
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        if (peekCanvas != null) peekCanvas.SetActive(false);

        StartCoroutine(ShowJumpscareSequence());
    }

    IEnumerator ShowJumpscareSequence()
    {
        if (jumpscareCanvas != null) jumpscareCanvas.SetActive(true);
        
        yield return new WaitForSecondsRealtime(1.5f); 

        if (jumpscareCanvas != null) jumpscareCanvas.SetActive(false);
        
        if (gameManager != null) gameManager.GameOver(); 
    }
}