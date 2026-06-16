using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections; 

public class PhoneManager : MonoBehaviour
{
    public static PhoneManager instance;

    [Header("UI References")]
    public GameObject phoneUI; 
    public TextMeshProUGUI senderText;
    public TextMeshProUGUI messageText;
    public GameObject instructionUI; 

    // ---> INI COLOKAN BARU BUAT BLUR VOLUME <---
    [Header("Post Processing Effects")]
    public GameObject blurVolume;

    [Header("Animation Settings")]
    public float slideDuration = 0.3f; 
    public Vector2 hiddenPosition = new Vector2(0, -600f); 
    public Vector2 visiblePosition = new Vector2(0, 0f);   

    [Header("Instruction Settings")]
    public float instructionDuration = 4f; 

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip notifSound;
    
    [HideInInspector]
    public bool canUsePhone = true;

    private bool isPhoneActive = false;
    private RectTransform phoneRect;
    private Coroutine activeAnim;
    private Coroutine instructionFadeCor; 

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        canUsePhone = true; 
        activeAnim = null;
        instructionFadeCor = null; 
        
        if (phoneUI != null)
        {
            phoneRect = phoneUI.GetComponent<RectTransform>();
            phoneRect.anchoredPosition = hiddenPosition; 
            phoneUI.SetActive(false); 
        }

        if (instructionUI != null) instructionUI.SetActive(false);

        // ---> PASTIKAN BLUR MATI SAAT GAME BARU MULAI <---
        if (blurVolume != null) blurVolume.SetActive(false);
    }

    void Update()
    {
        if (canUsePhone && Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePhone();
        }
    }

    public void ReceiveSMS(string sender, string message)
    {
        if (senderText != null) senderText.text = sender;
        if (messageText != null) messageText.text = message;
        
        Canvas.ForceUpdateCanvases(); 

        if (audioSource != null && notifSound != null)
        {
            audioSource.PlayOneShot(notifSound);
        }

        if (instructionUI != null && !isPhoneActive)
        {
            instructionUI.SetActive(true);

            if (instructionFadeCor != null) 
            {
                StopCoroutine(instructionFadeCor);
            }
            instructionFadeCor = StartCoroutine(HideInstructionAfterDelay(instructionDuration));
        }
    }

    void TogglePhone()
    {
        if (activeAnim != null) return; 

        isPhoneActive = !isPhoneActive;

        if (isPhoneActive)
        {
            if (instructionUI != null) instructionUI.SetActive(false);
            
            if (instructionFadeCor != null)
            {
                StopCoroutine(instructionFadeCor);
                instructionFadeCor = null;
            }
            
            // ---> NYALAKAN BLUR PAS HP DIBUKA <---
            if (blurVolume != null) blurVolume.SetActive(true);

            phoneUI.SetActive(true);
            activeAnim = StartCoroutine(SlidePhoneAnim(visiblePosition, false));
        }
        else
        {
            // ---> MATIKAN BLUR PAS HP DISIMPEN <---
            if (blurVolume != null) blurVolume.SetActive(false);

            activeAnim = StartCoroutine(SlidePhoneAnim(hiddenPosition, true));
        }
    }

    IEnumerator HideInstructionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (instructionUI != null) 
        {
            instructionUI.SetActive(false);
        }
        instructionFadeCor = null; 
    }

    IEnumerator SlidePhoneAnim(Vector2 targetPos, bool disableAfter)
    {
        Vector2 startPos = phoneRect.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / slideDuration);
            
            phoneRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        phoneRect.anchoredPosition = targetPos;

        if (disableAfter && phoneUI != null)
        {
            phoneUI.SetActive(false);
        }

        activeAnim = null; 
    }

    public void ForceHidePhone()
    {
        canUsePhone = false; 

        if (isPhoneActive)
        {
            isPhoneActive = false;
            if (activeAnim != null) StopCoroutine(activeAnim); 
            
            if (phoneUI != null) phoneUI.SetActive(false);
            if (phoneRect != null) phoneRect.anchoredPosition = hiddenPosition; 
        }

        if (instructionFadeCor != null)
        {
            StopCoroutine(instructionFadeCor);
            instructionFadeCor = null;
        }

        if (instructionUI != null) instructionUI.SetActive(false); 

        // ---> PASTIKAN BLUR MATI KALAU HP DIPAKSA TUTUP <---
        if (blurVolume != null) blurVolume.SetActive(false);
    }

    public void AllowPhoneUsage()
    {
        canUsePhone = true; 
    }
}