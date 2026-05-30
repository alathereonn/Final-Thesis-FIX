using UnityEngine;
using System.Collections;

public class SmoothPeekCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;     
    public Transform doorAnchor;       
    public Transform defaultCameraPos; 
    public SC_FPSController fpsController; 
    
    // TAMPILAN BARU: Slot buat masukin UI Canvas 2D
    [Header("UI 2D")]
    public GameObject peekCanvas; 

    [Header("Settings")]
    public float transitionDuration = 0.5f; 
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 

    private Coroutine activeTransition; 
    private bool isAtDoor = false;     

    // Dipanggil dari raycast pas ngeklik pintu 3D
    public void InteractWithDoor()
    {
        // FIX BUG SPAM KLIK: Kalau kamera lagi gerak, cuekin aja kliknya
        if (activeTransition != null) return; 

        if (!isAtDoor)
        {
            // MENGINTIP: Matiin pergerakan player
            if (fpsController != null) fpsController.canMove = false;
            
            activeTransition = StartCoroutine(MoveCamera(defaultCameraPos, doorAnchor, false));
            peekCanvas.GetComponent<Door2DManager>().ResetView();
            
            isAtDoor = true;
        }
    }

    // FUNGSI BARU: Dipanggil pas ngeklik tombol "Back" di layar 2D
    public void StopPeeking()
    {
        // FIX BUG SPAM KLIK: Kalau kamera lagi gerak, cuekin aja kliknya
        if (activeTransition != null) return; 
        
        // Langsung matiin Canvas 2D biar layar bersih
        if (peekCanvas != null) peekCanvas.SetActive(false);

        // Kamera mundur balik ke posisi player
        activeTransition = StartCoroutine(MoveCamera(doorAnchor, defaultCameraPos, true));
        isAtDoor = false;
    }

    IEnumerator MoveCamera(Transform start, Transform target, bool isReturning)
    {
        float elapsedTime = 0f;
        Vector3 startPos = start.position;
        Quaternion startRot = start.rotation;
        Vector3 targetPos = target.position;
        Quaternion targetRot = target.rotation;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / transitionDuration);

            playerCamera.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        playerCamera.position = targetPos;
        playerCamera.rotation = targetRot;

        // LOGIKA SETELAH KAMERA SELESAI GERAK:
        if (!isReturning)
        {
            // Kalau baru nyampe pintu: Nyalain Layar 2D & Bebasin Mouse
            if (peekCanvas != null) peekCanvas.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None; // Buka kunci mouse
            Cursor.visible = true; // Munculin kursor mouse
        }
        else
        {
            // Kalau udah balik ke badan player: Idupin pergerakan & Kunci Mouse
            if (fpsController != null) fpsController.canMove = true;
            
            Cursor.lockState = CursorLockMode.Locked; // Kunci mouse ke tengah (ala FPS)
            Cursor.visible = false; // Sembunyiin kursor
        }

        activeTransition = null;
    }
}