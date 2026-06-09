using UnityEngine;
using UnityEngine.EventSystems; 
using TMPro; // WAJIB DITAMBAHIN BUAT BACA TEKS UI

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f; 
    public Camera playerCamera;         

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText; // Tempat masang teks "Press to Open"

    void Update()
    {
        // Garis merah bantuan di tab Scene
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance, Color.red);

        // 1. DEFAULT: Sembunyikan teks setiap saat (akan dinyalakan lagi di bawah kalau laser kena target)
        if (promptText != null) 
        {
            promptText.gameObject.SetActive(false);
        }

        // LOGIKA SATPAM UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; 
        }

        // 2. CEK SOROTAN LASER (Tampilin Teks kalau pas kena)
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
            // A. Kalau yang disorot punya script ngintip pintu
            if (hit.collider.GetComponent<InteractablePeek>() != null)
            {
                ShowPrompt("[Left Click] to take a peek");
            }
            // B. Kalau yang disorot punya script buka laptop
            else if (hit.collider.GetComponent<LaptopInteract>() != null)
            {
                ShowPrompt("[Left Click] to start your Thesis's Progress");
            }
            // C. Abang bisa tambahin "else if" lain di sini kalau ada laci, lemari, dll.
        }
    }

    void ShowPrompt(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
            promptText.gameObject.SetActive(true); // Nyalakan teksnya
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Cek interaksi pintu
            InteractablePeek targetPeek = hit.collider.GetComponent<InteractablePeek>();
            if (targetPeek != null)
            {
                targetPeek.Interact();
            }

            // (Catatan: Interaksi laptop abang pakai OnMouseDown di script LaptopInteract, 
            // jadi nggak perlu dipanggil dari sini, otomatis jalan sendiri kalau diklik).
        }
    }
}