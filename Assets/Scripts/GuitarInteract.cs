using UnityEngine;

public class GuitarInteract : MonoBehaviour
{
    [Header("Pengaturan Teks & Prompt")]
    public string teksPrompt = "[Left Click] to play guitar"; 
    
    [TextArea(2, 4)] 
    public string teksDialog = "Gitar tua... Udah lama ga disentuh gara-gara pusing mikirin revisian TA.";
    public float durasiMuncul = 3f; 

    // Sekarang dia cuma jalan kalau dipanggil bos laser
    public void Interact()
    {
        if (MonologueManager.instance != null)
        {
            MonologueManager.instance.ShowMonologue(teksDialog, durasiMuncul);
        }
    }
}