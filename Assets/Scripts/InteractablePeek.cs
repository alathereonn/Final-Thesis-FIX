using UnityEngine;

public class InteractablePeek : MonoBehaviour 
{
    [Header("References")]
    public SmoothPeekCamera cameraSystem; 

    public void Interact()
    {
        if (cameraSystem != null)
        {
            if (PhoneManager.instance != null)
            {
                PhoneManager.instance.ForceHidePhone();
            }
            cameraSystem.InteractWithDoor();
        }
    }
}