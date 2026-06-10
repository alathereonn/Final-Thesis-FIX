using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; 
using TMPro; 
using System.Collections;

public class Door2DManager : MonoBehaviour
{
    [Header("References")]
    public MonsterAI monsterAI; 
    public Image doorViewImage; 
    
    public Button interactButton; 
    public TextMeshProUGUI interactButtonText; 
    public Button backButton; 

    [Header("Door Status")]
    public Sprite closedDoorSprite; 
    public Sprite emptyCorridorSprite; 

    [Header("Monster Stages")]
    public Sprite stage1Sprite; 
    public Sprite stage2Sprite; 
    public Sprite stage3Sprite; 
    
    [Header("Danger (Max Stage)")]
    public Sprite monsterAtDoorSprite; 

    [Header("Video Transition Effect")]
    public RawImage staticVideoRawImage; 
    public VideoPlayer staticVideoPlayer; 
    public float fadeSpeed = 2f; 

    [Header("Audio Transition Effect")]
    public AudioSource staticAudioSource;
    [Range(0f, 1f)]
    public float maxAudioVolume = 0.7f; 

    // --- KABEL BARU: EFEK GEMPA/SHAKE ---
    [Header("Shake Effect")]
    [Tooltip("Seberapa brutal layarnya goyang (dalam pixel)")]
    public float maxShakeMagnitude = 15f; 

    private bool isDoorOpen = false; 

    public void OnClickOpenDoor()
    {
        if (!isDoorOpen)
        {
            if (monsterAI.currentStage < monsterAI.maxStage)
            {
                switch (monsterAI.currentStage)
                {
                    case 0: doorViewImage.sprite = emptyCorridorSprite; break;
                    case 1: doorViewImage.sprite = stage1Sprite != null ? stage1Sprite : emptyCorridorSprite; break;
                    case 2: doorViewImage.sprite = stage2Sprite != null ? stage2Sprite : stage1Sprite; break;
                    case 3: doorViewImage.sprite = stage3Sprite != null ? stage3Sprite : stage2Sprite; break;
                    default: doorViewImage.sprite = emptyCorridorSprite; break;
                }
            }
            else if (monsterAI.currentStage == monsterAI.maxStage)
            {
                doorViewImage.sprite = monsterAtDoorSprite;
                StartCoroutine(StaredownRoutine());
            }
            
            isDoorOpen = true;
            if (monsterAI != null) monsterAI.isDoorOpen = true; 
            if (interactButtonText != null) interactButtonText.text = "Close"; 
        }
        else 
        {
            doorViewImage.sprite = closedDoorSprite;
            isDoorOpen = false;
            if (monsterAI != null) monsterAI.isDoorOpen = false; 
            if (interactButtonText != null) interactButtonText.text = "Open"; 
        }
    }

    IEnumerator StaredownRoutine()
    {
        if (interactButton != null) interactButton.interactable = false;
        if (backButton != null) backButton.interactable = false;

        // Simpan posisi asli layar biar habis goyang bisa balik ke tengah
        Vector2 originalDoorPos = doorViewImage.rectTransform.anchoredPosition;
        Vector2 originalStaticPos = staticVideoRawImage != null ? staticVideoRawImage.rectTransform.anchoredPosition : Vector2.zero;

        // Nyalain Video
        if (staticVideoPlayer != null) staticVideoPlayer.Play();
        if (staticVideoRawImage != null) staticVideoRawImage.gameObject.SetActive(true);
        
        // Nyalain Audio
        if (staticAudioSource != null)
        {
            staticAudioSource.volume = 0f;
            staticAudioSource.Play();
        }

        SetStaticAlpha(0f); 

        // 1. FASE FADE IN (Makin statis = Makin goyang brutal)
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            float clampedAlpha = Mathf.Clamp01(alpha);
            
            SetStaticAlpha(clampedAlpha);
            if (staticAudioSource != null) staticAudioSource.volume = clampedAlpha * maxAudioVolume;
            
            // JURUS GOYANG DOMBRET
            Vector2 shakeOffset = Random.insideUnitCircle * (maxShakeMagnitude * clampedAlpha);
            doorViewImage.rectTransform.anchoredPosition = originalDoorPos + shakeOffset;
            if (staticVideoRawImage != null) staticVideoRawImage.rectTransform.anchoredPosition = originalStaticPos + shakeOffset;

            yield return null; 
        }

        // FASE PUNCAK (Ditahan bentar dalam kondisi goyang maksimal)
        float peakTimer = 0.2f;
        while (peakTimer > 0)
        {
            peakTimer -= Time.deltaTime;
            Vector2 shakeOffset = Random.insideUnitCircle * maxShakeMagnitude; // Goyang full!
            doorViewImage.rectTransform.anchoredPosition = originalDoorPos + shakeOffset;
            if (staticVideoRawImage != null) staticVideoRawImage.rectTransform.anchoredPosition = originalStaticPos + shakeOffset;
            yield return null;
        }

        monsterAI.RepelMonster();
        doorViewImage.sprite = emptyCorridorSprite; 

        // 2. FASE FADE OUT (Makin jernih = Goyangan mereda)
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            float clampedAlpha = Mathf.Clamp01(alpha);
            
            SetStaticAlpha(clampedAlpha);
            if (staticAudioSource != null) staticAudioSource.volume = clampedAlpha * maxAudioVolume;
            
            // Goyangan makin pelan
            Vector2 shakeOffset = Random.insideUnitCircle * (maxShakeMagnitude * clampedAlpha);
            doorViewImage.rectTransform.anchoredPosition = originalDoorPos + shakeOffset;
            if (staticVideoRawImage != null) staticVideoRawImage.rectTransform.anchoredPosition = originalStaticPos + shakeOffset;

            yield return null;
        }

        // 3. FASE RESET (Kembalikan semuanya ke posisi & kondisi normal)
        doorViewImage.rectTransform.anchoredPosition = originalDoorPos;
        if (staticVideoRawImage != null)
        {
            staticVideoRawImage.rectTransform.anchoredPosition = originalStaticPos;
            staticVideoRawImage.gameObject.SetActive(false);
        }
        
        if (staticVideoPlayer != null) staticVideoPlayer.Stop();
        if (staticAudioSource != null) staticAudioSource.Stop();

        if (interactButton != null) interactButton.interactable = true;
        if (backButton != null) backButton.interactable = true;
    }

    void SetStaticAlpha(float alpha)
    {
        if (staticVideoRawImage != null)
        {
            Color c = staticVideoRawImage.color;
            c.a = alpha;
            staticVideoRawImage.color = c;
        }
    }

    public void ResetView()
    {
        doorViewImage.sprite = closedDoorSprite;
        isDoorOpen = false;
        if (monsterAI != null) monsterAI.isDoorOpen = false; 
        
        if (interactButtonText != null) interactButtonText.text = "Open";
        if (interactButton != null) interactButton.interactable = true; 
        if (backButton != null) backButton.interactable = true; 
    }
}