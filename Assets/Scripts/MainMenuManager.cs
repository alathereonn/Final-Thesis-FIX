using UnityEngine;
using UnityEngine.UI;
using TMPro; // Wajib ditambahin buat ngedit teks
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button continueButton;
    public TextMeshProUGUI continueButtonText;

    void Start()
    {
        // 1. Buka buku catatan, cek "HighestNight". Kalau kosong, defaultnya 1.
        int highestNight = PlayerPrefs.GetInt("HighestNight", 1);

        // 2. Logika Si Penjaga Pintu
        if (highestNight > 1)
        {
            // Kalau udah pernah lewat Night 1, nyalain tombol Continue
            continueButton.gameObject.SetActive(true);
            if (continueButtonText != null)
            {
                continueButtonText.text = "CONTINUE (NIGHT " + highestNight + ")";
            }
        }
        else
        {
            // Kalau masih cupu (Night 1), sembunyiin tombol Continue
            continueButton.gameObject.SetActive(false);
        }
    }

    // Dipanggil pas klik tombol "New Game"
    public void NewGame()
    {
        // Hapus paksa memori, balik ke Night 1
        PlayerPrefs.SetInt("HighestNight", 1);
        PlayerPrefs.SetInt("CurrentPlayNight", 1);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("SampleScene"); 
    }

    // Dipanggil pas klik tombol "Continue"
    public void ContinueGame()
    {
        // Lanjutin main di malam tertinggi yang udah kebuka
        int highestNight = PlayerPrefs.GetInt("HighestNight", 1);
        PlayerPrefs.SetInt("CurrentPlayNight", highestNight);
        PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene");
    }

    public void OpenOptionsScene()
    {
        SceneManager.LoadScene("OptionsMenu");
    }

    // Tambahkan fungsi ini di bawah fungsi OpenOptionsScene atau di mana saja di dalam class
    public void QuitGame()
    {
        Debug.Log("Game sedang ditutup..."); // Log ini buat ngetes di Unity Editor
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}