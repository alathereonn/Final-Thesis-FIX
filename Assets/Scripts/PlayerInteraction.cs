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

        // ---> UPGRADE: PAKE LASER TEBAL BIAR KLIKNYA GAMPANG <---
        if (Physics.SphereCast(ray, 0.3f, out hit, interactDistance))
        {
            if (hit.collider.GetComponent<InteractablePeek>() != null)
            {
                ShowPrompt("[Left Click] to take a peek");
            }
            else if (hit.collider.GetComponent<LaptopInteract>() != null)
            {
                ShowPrompt("[Left Click] to start your Thesis's Progress");
            }
            else if (hit.collider.GetComponent<PaperInteract>() != null)
            {
                PaperInteract paper = hit.collider.GetComponent<PaperInteract>();
                if (!paper.isReading) 
                {
                    ShowPrompt("[Left Click] to read paper");
                }
            }
            // ---> INI DIA SENSOR GITARNYA BANG <---
            else if (hit.collider.GetComponent<GuitarInteract>() != null)
            {
                GuitarInteract barang = hit.collider.GetComponent<GuitarInteract>();
                ShowPrompt(barang.teksPrompt); 
            }
            else if (hit.collider.GetComponent<BedInteract>() != null)
            {
                BedInteract kasur = hit.collider.GetComponent<BedInteract>();
                ShowPrompt(kasur.teksPrompt); 
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

        if (Physics.SphereCast(ray, 0.3f, out hit, interactDistance))
        {
            InteractablePeek targetPeek = hit.collider.GetComponent<InteractablePeek>();
            if (targetPeek != null) targetPeek.Interact();

            PaperInteract targetPaper = hit.collider.GetComponent<PaperInteract>();
            if (targetPaper != null && !targetPaper.isReading) targetPaper.BukaKertas();
            
            GuitarInteract targetGuitar = hit.collider.GetComponent<GuitarInteract>();
            if (targetGuitar != null) targetGuitar.Interact();

            BedInteract targetBed = hit.collider.GetComponent<BedInteract>();
            if (targetBed != null) targetBed.Interact();
        }
    }
}