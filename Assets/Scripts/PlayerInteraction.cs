using UnityEngine;
using UnityEngine.EventSystems; // 1. WAJIB DITAMBAHIN BUAT SATPAM UI

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f; 
    public Camera playerCamera;         

    void Update()
    {
        // Garis merah bantuan di tab Scene
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance, Color.red);

        // 2. LOGIKA SATPAM UI
        // Kalau mouse lagi nge-hover di atas UI (termasuk tombol Try Again), batalkan klik 3D!
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // Berhenti di sini, kode di bawahnya nggak bakal dijalanin
        }

        // Kalau aman (nggak ngeklik UI), baru cek klik kiri buat interaksi 3D
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            InteractablePeek targetInteractable = hit.collider.GetComponent<InteractablePeek>();
            
            if (targetInteractable != null)
            {
                Debug.Log("Sip, nemu script interaksi! Menjalankan interaksi...");
                targetInteractable.Interact();
            }
        }
        else
        {
            Debug.Log("Gak kena apa-apa, coba lagi!");
        }
    }
}