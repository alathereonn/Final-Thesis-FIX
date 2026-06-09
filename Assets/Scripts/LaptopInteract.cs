using UnityEngine;
using System.Collections; // Wajib buat pakai Coroutine

public class LaptopInteract : MonoBehaviour
{
    [Header("Referensi Objek 3D")]
    public Transform playerCamera;       // Masukin Main Camera ke sini
    public MonoBehaviour fpsController;  // Objek Player lu
    public Transform laptopViewpoint;    // Objek kosong yang barusan lu bikin

    [Header("Referensi UI 2D")]
    public GameObject laptopCanvas;     

    [Header("Pengaturan Zoom")]
    public float maxInteractDistance = 3f; 
    public float zoomSpeed = 5f;         // Semakin gede semakin cepet zoom-nya

    private bool isInteracting = false;
    private Vector3 originalCamPosition;
    private Quaternion originalCamRotation;
    private Coroutine currentZoomCoroutine;

    void OnMouseDown()
    {
        float distance = Vector3.Distance(transform.position, fpsController.transform.position);
        
        // Cek jarak dan pastiin lagi gak dalam proses zoom/ngetik
        if (distance <= maxInteractDistance && !isInteracting)
        {
            StartCoroutine(ZoomInToLaptop());
        }
    }

    void Update()
    {
        // TOMBOL PANIK: ESC atau Klik Kanan buat keluar
        if (isInteracting && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            StartCoroutine(ZoomOutFromLaptop());
        }
    }

    IEnumerator ZoomInToLaptop()
    {
        isInteracting = true;
        fpsController.enabled = false; // Matiin jalan di awal zoom biar gak lari-lari

        if (PhoneManager.instance != null)
        {
            PhoneManager.instance.ForceHidePhone();
        }

        // 1. Simpan posisi dan rotasi awal kamera buat nanti balik
        originalCamPosition = playerCamera.position;
        originalCamRotation = playerCamera.rotation;

        // 2. Proses Pergerakan Kamera Mulus (Zoom In)
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime * zoomSpeed;

            // Gerakin posisi kamera dari awal ke viewpoint laptop secara mulus
            playerCamera.position = Vector3.Lerp(originalCamPosition, laptopViewpoint.position, progress);
            
            // Putar arah kamera dari awal ke viewpoint laptop secara mulus
            playerCamera.rotation = Quaternion.Slerp(originalCamRotation, laptopViewpoint.rotation, progress);
            
            yield return null; // Tunggu satu frame
        }

        // 3. Zoom Selesai, Nyalain Layar UI
        if (laptopCanvas != null) laptopCanvas.SetActive(true);
    }

    IEnumerator ZoomOutFromLaptop()
    {
        // 1. Matikan UI dulu biar gak ngehalangin pemandangan
        if (laptopCanvas != null) laptopCanvas.SetActive(false);

        // 2. Proses Pergerakan Kamera Mulus (Zoom Out Kembali ke Badan Player)
        float progress = 0;
        
        // Kita butuh tau posisi kamera saat ini pas tombol panik dipencet
        Vector3 camCurrentPos = playerCamera.position;
        Quaternion camCurrentRot = playerCamera.rotation;

        while (progress < 1)
        {
            progress += Time.deltaTime * zoomSpeed;

            // Balikin posisi dan rotasi kamera ke asal
            playerCamera.position = Vector3.Lerp(camCurrentPos, originalCamPosition, progress);
            playerCamera.rotation = Quaternion.Slerp(camCurrentRot, originalCamRotation, progress);
            
            yield return null; // Tunggu satu frame
        }

        // 3. Zoom Out Selesai, Balikin Kontrol Player
        isInteracting = false;
        fpsController.enabled = true;
        
        if (PhoneManager.instance != null)
        {
            PhoneManager.instance.AllowPhoneUsage();
        }
    }
}