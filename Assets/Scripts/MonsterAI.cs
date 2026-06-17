using System.Collections;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("AI Settings")]
    public string monsterName = "Room Door Monster";
    public float moveInterval = 4f; 
    
    [HideInInspector]
    public int aiLevel = 1; 

    [Header("Current Status (Read Only)")]
    public int currentStage = 0;
    public int maxStage = 3;

    [Header("Interaction State")]
    public bool isDoorOpen = false;

    // --- KABEL BARU BUAT JUMPSCARE GOYANG ---
    [Header("Jumpscare References")]
    public GameObject jumpscareCanvas; 
    [Tooltip("Masukin RectTransform dari Gambar Jumpscare (anaknya Canvas)")]
    public RectTransform jumpscareImageRT; // Wajib dicolok!
    public GameManager gameManager;    
    public GameObject peekCanvas;      
    public SC_FPSController fpsController; 

    [Header("3D Camera Shake (Opsional)")]
    public Camera mainCamera; // Colok Main Camera player lu ke sini
    public bool use3DCameraShake = false; 
    [Tooltip("Magnitude kamera 3D nggak usah gede-gede, 0.2 atau 0.5 udah bikin pusing")]
    public float cameraShakeMagnitude = 0.2f;

    [Header("Jumpscare Shake Settings")]
    [Tooltip("Seberapa brutal jumpscarenya goyang (disarankan angka gede, misal 50-100)")]
    public float shakeMagnitude = 70f;
    [Tooltip("Durasi jumpscare bergoyang sebelum pindah scene Game Over")]
    public float shakeDuration = 1.5f;

    // --- UPGRADE AUDIO MULTI-STAGE ---
    [Header("Audio Settings (Per Stage)")]
    public AudioSource[] stageAudioSources = new AudioSource[3];
    public AudioClip[] stageSounds = new AudioClip[3];

    private Coroutine aiMovementCoroutine;
    private Coroutine jumpscareTimerCoroutine; 
    private static bool hasPlayedFirstStageDialog = false;

    void Start()
    {
        hasPlayedFirstStageDialog = false; 
        aiMovementCoroutine = StartCoroutine(ProcessMovementRNG());
    }

    IEnumerator ProcessMovementRNG()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            if (peekCanvas != null && peekCanvas.activeSelf && isDoorOpen)
            {
                continue; 
            }

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

        // --- SISTEM DIALOG DINAMIS BERDASARKAN MALAM ---
        if (!hasPlayedFirstStageDialog)
        {
            hasPlayedFirstStageDialog = true; 
            if (MonologueManager.instance != null)
            {
                string reactionText = "";
                float showTime = 3f;

                // Cek malam ke berapa lewat GameManager
                if (gameManager != null)
                {
                    switch (gameManager.currentNight)
                    {
                        case 1: reactionText = "What? What the hell was that? I must be exhausted."; break;
                        case 2: reactionText = "There's that sound again. I'm not imagining this."; break;
                        case 3: reactionText = "Am I losing my mind?"; break;
                        case 4: reactionText = "It's getting closer, Iko."; showTime = 4f; break;
                        case 5: reactionText = "Alright. Now, I'm pissed."; showTime = 4f; break;
                        case 6: reactionText = "Come on! Please, just let me finish this!"; showTime = 4f; break;
                        default: reactionText = "What was that?"; break;
                    }
                }
                else
                {
                    reactionText = "What? What the hell was that???";
                }

                MonologueManager.instance.ShowMonologue(reactionText, showTime);
            }
        }

        int audioIndex = currentStage - 1;
        if (audioIndex >= 0 && audioIndex < stageAudioSources.Length && audioIndex < stageSounds.Length)
        {
            if (stageAudioSources[audioIndex] != null && stageSounds[audioIndex] != null)
            {
                stageAudioSources[audioIndex].PlayOneShot(stageSounds[audioIndex]);
            }
        }

        if (currentStage == maxStage)
        {
            Debug.Log($"[WARNING] {monsterName} IS AT THE DOOR! STARTING JUMPSCARE TIMER!");
            jumpscareTimerCoroutine = StartCoroutine(JumpscareCountdown());
        }
    }

    IEnumerator JumpscareCountdown()
    {
        float timeLimit = 20f - gameManager.currentNight;
        float timer = 0f;
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
        if (jumpscareTimerCoroutine != null)
        {
            StopCoroutine(jumpscareTimerCoroutine);
            jumpscareTimerCoroutine = null;
        }
    }

    void TriggerJumpscare()
    {
        Debug.Log($"[GAME OVER] KENA JUMPSCARE SAMA {monsterName}!");

        // Berhentiin waktu dunia 3D
        Time.timeScale = 0f;

        StopAllCoroutines(); 
        if (fpsController != null) fpsController.canMove = false;
        
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;

        if (peekCanvas != null) peekCanvas.SetActive(false);

        // Mulai sutradara jumpscare brutal
        StartCoroutine(ShowJumpscareSequence());
    }

    // --- SUTRADARA JUMPSCARE BRUTAL ---
    IEnumerator ShowJumpscareSequence()
    {
        if (jumpscareCanvas != null) jumpscareCanvas.SetActive(true);

        // Pastiin komponen audio jumpscare lu di set ke "Play On Awake" atau di-play manual di sini le
        
        Vector2 originalUIPos = Vector2.zero;
        if (jumpscareImageRT != null) originalUIPos = jumpscareImageRT.anchoredPosition;

        // Simpan posisi asli kamera 3D biar ga miring selamanya
        Vector3 originalCamPos = Vector3.zero;
        if (mainCamera != null) originalCamPos = mainCamera.transform.localPosition;

        float elapsed = 0f;

        // GOYANG BRUTAL BERDASARKAN WAKTU NYATA (Wajib pakai unscaledDeltaTime)
        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Ngitung waktu walau Time.timeScale = 0

            // 1. Goyang UI 2D
            if (jumpscareImageRT != null)
            {
                Vector2 uiShakeOffset = Random.insideUnitCircle * shakeMagnitude;
                jumpscareImageRT.anchoredPosition = originalUIPos + uiShakeOffset;
            }

            // 2. Goyang Kamera 3D (Baru jalan kalau dicentang dan dicolok!)
            if (use3DCameraShake && mainCamera != null)
            {
                Vector3 camShakeOffset = Random.insideUnitSphere * cameraShakeMagnitude;
                mainCamera.transform.localPosition = originalCamPos + camShakeOffset;
            }

            yield return null; // Tunggu frame selanjutnya (di Realtime)
        }

        // Balikin ke tengah semua biar rapi
        if (jumpscareImageRT != null) jumpscareImageRT.anchoredPosition = originalUIPos;
        if (mainCamera != null) mainCamera.transform.localPosition = originalCamPos;
        
        // --- TAMBAHIN INI BIAR KURSORNYA MUNCUL LAGI BUAT NGEKLIK MENU ---
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        
        // Pindah ke scene Game Over
        if (jumpscareCanvas != null) jumpscareCanvas.SetActive(false);
        if (gameManager != null) gameManager.GameOver(); 
    }
}