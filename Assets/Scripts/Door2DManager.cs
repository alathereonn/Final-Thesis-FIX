using UnityEngine;
using UnityEngine.UI;
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
    public Sprite emptyCorridorSprite; // Gambar pas Stage 0 (Aman)

    [Header("Monster Stages (Isi yang ada aja)")]
    public Sprite stage1Sprite; 
    public Sprite stage2Sprite; 
    public Sprite stage3Sprite; 
    
    [Header("Danger (Max Stage)")]
    public Sprite monsterAtDoorSprite; // Gambar pas monster siap nerkam

    private bool isDoorOpen = false; 

    public void OnClickOpenDoor()
    {
        if (!isDoorOpen)
        {
            // Jika monster belum di depan pintu (belum Max Stage)
            if (monsterAI.currentStage < monsterAI.maxStage)
            {
                // Sistem Slot Pintar: Tentukan gambar berdasarkan stage saat ini
                switch (monsterAI.currentStage)
                {
                    case 0:
                        doorViewImage.sprite = emptyCorridorSprite;
                        break;
                    case 1:
                        // Kalau stage1 kosong, pakai gambar lorong kosong
                        doorViewImage.sprite = stage1Sprite != null ? stage1Sprite : emptyCorridorSprite;
                        break;
                    case 2:
                        // Kalau stage2 kosong, pakai gambar stage1
                        doorViewImage.sprite = stage2Sprite != null ? stage2Sprite : stage1Sprite;
                        break;
                    case 3:
                        // Kalau stage3 kosong, pakai gambar stage2
                        doorViewImage.sprite = stage3Sprite != null ? stage3Sprite : stage2Sprite;
                        break;
                    default:
                        doorViewImage.sprite = emptyCorridorSprite;
                        break;
                }
            }
            // Jika monster sudah di depan pintu (Max Stage)
            else if (monsterAI.currentStage == monsterAI.maxStage)
            {
                doorViewImage.sprite = monsterAtDoorSprite;
                StartCoroutine(StaredownRoutine());
            }
            
            isDoorOpen = true;
            if (interactButtonText != null) interactButtonText.text = "Close"; 
        }
        else 
        {
            doorViewImage.sprite = closedDoorSprite;
            isDoorOpen = false;
            if (interactButtonText != null) interactButtonText.text = "Open"; 
        }
    }

    IEnumerator StaredownRoutine()
    {
        // Kunci semua tombol biar player ga bisa kabur
        if (interactButton != null) interactButton.interactable = false;
        if (backButton != null) backButton.interactable = false;

        // Tatapan maut 1.5 detik
        yield return new WaitForSeconds(1.5f);

        monsterAI.RepelMonster();
        
        // Kembalikan pemandangan ke Stage 0 (Lorong Kosong)
        doorViewImage.sprite = emptyCorridorSprite;
        
        // Buka kunci tombol
        if (interactButton != null) interactButton.interactable = true;
        if (backButton != null) backButton.interactable = true;
    }

    public void ResetView()
    {
        doorViewImage.sprite = closedDoorSprite;
        isDoorOpen = false;
        
        if (interactButtonText != null) interactButtonText.text = "Open";
        if (interactButton != null) interactButton.interactable = true; 
        if (backButton != null) backButton.interactable = true; 
    }
}