using UnityEngine;
using System.Collections;

public class SmoothPeekCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;     
    public Transform doorAnchor;       
    public Transform defaultCameraPos; 
    public SC_FPSController fpsController; 
    
    [Header("UI 2D")]
    public GameObject peekCanvas; 

    [Header("Settings")]
    public float transitionDuration = 0.5f; 
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 

    private Coroutine activeTransition; 
    private bool isAtDoor = false;     

    public void InteractWithDoor()
    {
        if (activeTransition != null) return; // Cegah spam klik

        if (!isAtDoor)
        {
            if (fpsController != null) fpsController.canMove = false; // Kunci player
            
            // MULAI ALUR MASUK
            activeTransition = StartCoroutine(EnterPeekSequence());
            peekCanvas.GetComponent<Door2DManager>().ResetView();
            isAtDoor = true;
        }
    }

    public void StopPeeking()
    {
        if (activeTransition != null) return; // Cegah spam klik
        
        // MULAI ALUR KELUAR
        activeTransition = StartCoroutine(ExitPeekSequence());
        isAtDoor = false;
    }

    // --- ALUR HIBRIDA MASUK ---
    IEnumerator EnterPeekSequence()
    {
        // 1. Kamera Pindah Dulu
        yield return StartCoroutine(MoveCameraOnly(defaultCameraPos, doorAnchor));

        // 2. Kamera Nyampe -> Layar Gelap -> Muncul 2D Canvas -> Terang Lagi
        if (FadeManager.instance != null)
        {
            FadeManager.instance.DoTransition(
                midAction: () => {
                    if (peekCanvas != null) peekCanvas.SetActive(true);
                    Cursor.lockState = CursorLockMode.None; 
                    Cursor.visible = true; 
                },
                onComplete: () => {
                    activeTransition = null; // Transisi selesai total
                }
            );
        }
        else // Fallback kalau lu lupa pasang FadeManager
        {
            if (peekCanvas != null) peekCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
            activeTransition = null;
        }
    }

    // --- ALUR HIBRIDA KELUAR ---
    IEnumerator ExitPeekSequence()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 

        // 1. Layar Gelap -> Ilangin 2D Canvas -> Layar Terang Lagi
        if (FadeManager.instance != null)
        {
            FadeManager.instance.DoTransition(
                midAction: () => {
                    if (peekCanvas != null) peekCanvas.SetActive(false);
                },
                onComplete: () => {
                    // 2. Pas Layar UDAH TERANG, baru Kamera Mundur
                    StartCoroutine(MoveCameraBackAndFinish());
                }
            );
        }
        else
        {
            if (peekCanvas != null) peekCanvas.SetActive(false);
            StartCoroutine(MoveCameraBackAndFinish());
        }
        yield return null;
    }

    IEnumerator MoveCameraBackAndFinish()
    {
        yield return StartCoroutine(MoveCameraOnly(doorAnchor, defaultCameraPos));
        if (fpsController != null) fpsController.canMove = true;if (PhoneManager.instance != null)
        {
            PhoneManager.instance.AllowPhoneUsage();
        }
        activeTransition = null; // Transisi selesai total
    }

    // Mesin Penggerak Kamera Murni 
    IEnumerator MoveCameraOnly(Transform start, Transform target)
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
    }
}