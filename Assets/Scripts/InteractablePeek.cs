using UnityEngine;

public class InteractablePeek : MonoBehaviour 
{
    [Header("References")]
    public SmoothPeekCamera cameraSystem; 

    public void Interact()
    {
        if (cameraSystem != null)
        {
            cameraSystem.InteractWithDoor();
        }
    }
}