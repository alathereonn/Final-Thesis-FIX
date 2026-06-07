using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    public SmoothPeekCamera cameraSystem;

    public void TriggerBackWithFade()
    {
        if (cameraSystem != null) 
        {
            cameraSystem.StopPeeking();
        }
    }
}