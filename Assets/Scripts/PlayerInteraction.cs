using UnityEngine;
using UnityEngine.EventSystems; 
using TMPro; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f; 
    public Camera playerCamera;         

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText;

    void Update()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance, Color.red);

        // 1. DEFAULT: Sembunyikan teks setiap saat
        if (promptText != null) 
        {
            promptText.gameObject.SetActive(false);
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; 
        }

        // 2. CEK SOROTAN LASER
        CheckHover();

        // 3. CEK KLIK UNTUK INTERAKSI
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    void CheckHover()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Debug.Log("LASER NABRAK: " + hit.collider.gameObject.name);
            if (hit.collider.GetComponent<InteractablePeek>() != null)
            {
                ShowPrompt("[Left Click] to take a peek");
            }
            else if (hit.collider.GetComponent<LaptopInteract>() != null)
            {
                ShowPrompt("[Left Click] to start your Thesis's Progress");
            }
            // ---> INI TAMBAHAN BUAT KERTAS BANG <---
            else if (hit.collider.GetComponent<PaperInteract>() != null)
            {
                // Cek dulu apakah lagi baca kertas atau gak, kalau lagi baca teksnya disembunyiin
                PaperInteract paper = hit.collider.GetComponent<PaperInteract>();
                if (!paper.isReading) 
                {
                    ShowPrompt("[Left Click] to read paper");
                }
            }
        }
    }

    void ShowPrompt(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
            promptText.gameObject.SetActive(true); 
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            InteractablePeek targetPeek = hit.collider.GetComponent<InteractablePeek>();
            if (targetPeek != null)
            {
                targetPeek.Interact();
            }

            // ---> INI TAMBAHAN BUAT KLIK KERTAS BANG <---
            PaperInteract targetPaper = hit.collider.GetComponent<PaperInteract>();
            if (targetPaper != null && !targetPaper.isReading)
            {
                targetPaper.BukaKertas();
            }
        }
    }
}