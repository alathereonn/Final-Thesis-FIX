using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;

public class Door2DManager : MonoBehaviour
{
    [Header("Referensi")]
    public MonsterAI monsterAI; 
    public Image doorViewImage; 
    
    public Button tombolInteract; 
    public TextMeshProUGUI teksTombolInteract; 
    
    // SLOT BARU: Buat masukin tombol Back
    public Button tombolBack; 

    [Header("Sprite / Foto")]
    public Sprite fotoPintuTutup;
    public Sprite fotoLorongKosong;
    public Sprite fotoMonster;

    private bool isPintuTerbuka = false; 

    public void OnClickOpenDoor()
    {
        if (!isPintuTerbuka)
        {
            if (monsterAI.currentStage < monsterAI.maxStage)
            {
                // Aman, lorong kosong
                doorViewImage.sprite = fotoLorongKosong;
            }
            else if (monsterAI.currentStage == monsterAI.maxStage)
            {
                // JUMPSCARE PIGAI! Tunjukin foto monster dan mulai siksaan
                doorViewImage.sprite = fotoMonster;
                StartCoroutine(StaredownRoutine());
            }
            
            isPintuTerbuka = true;
            if (teksTombolInteract != null) teksTombolInteract.text = "Tutup"; 
        }
        else 
        {
            doorViewImage.sprite = fotoPintuTutup;
            isPintuTerbuka = false;
            if (teksTombolInteract != null) teksTombolInteract.text = "Buka"; 
        }
    }

    IEnumerator StaredownRoutine()
    {
        // KUNCI SEMUA TOMBOL: Player ga bisa nutup pintu ATAU kabur ke 3D
        if (tombolInteract != null) tombolInteract.interactable = false;
        if (tombolBack != null) tombolBack.interactable = false;

        // Player dipaksa natap muka monsternya selama 1.5 detik
        yield return new WaitForSeconds(1.5f);

        // Usir monster
        monsterAI.RepelMonster();
        
        // Ganti gambar jadi lorong kosong
        doorViewImage.sprite = fotoLorongKosong;
        
        // BUKA KUNCI SEMUA TOMBOL: Player udah bisa kabur atau nutup pintu
        if (tombolInteract != null) tombolInteract.interactable = true;
        if (tombolBack != null) tombolBack.interactable = true;
    }

    public void ResetView()
    {
        doorViewImage.sprite = fotoPintuTutup;
        isPintuTerbuka = false;
        
        // Reset semua tombol ke kondisi awal tiap kali player baru ngintip
        if (teksTombolInteract != null) teksTombolInteract.text = "Buka";
        if (tombolInteract != null) tombolInteract.interactable = true; 
        if (tombolBack != null) tombolBack.interactable = true; 
    }
}