using UnityEngine;

// 1. Ubah nama class jadi InteractablePeek
public class InteractablePeek : MonoBehaviour 
{
    [Header("References")]
    // 2. Ubah tipe datanya jadi SmoothPeekCamera
    public SmoothPeekCamera cameraSystem; 

    public void Interact()
    {
        if (cameraSystem != null)
        {
            cameraSystem.InteractWithDoor();
        }
    }
}