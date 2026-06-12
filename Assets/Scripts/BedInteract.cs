using UnityEngine;

public class BedInteract : MonoBehaviour
{
    [Header("Pengaturan Teks & Prompt")]
    public string teksPrompt = "[Left Click] to rest"; 
    
    [TextArea(2, 4)] 
    public string teksDialog = "Kasur kesayangan... Pengen rebahan, tapi script model IndoBERT buat deteksi headline beritanya belum selesai di-train.";
    public float durasiMuncul = 4f; 

    public void Interact()
    {
        if (MonologueManager.instance != null)
        {
            MonologueManager.instance.ShowMonologue(teksDialog, durasiMuncul);
        }
    }
}