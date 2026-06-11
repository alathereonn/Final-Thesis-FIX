using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // WAJIB ADA INI BUAT NGENALIN SLIDER
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer mainMixer;

    [Header("UI Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    // Fungsi ini otomatis jalan pertama kali pas menu Options dibuka
    void Start()
    {
        // 1. Load data yang tersimpan. Kalau belum pernah main, kasih nilai default 1 (full)
        float savedMaster = PlayerPrefs.GetFloat("MasterVol", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 1f);

        // 2. Geser posisi gagang slider biar sesuai sama data yang disave
        if(masterSlider != null) masterSlider.value = savedMaster;
        if(musicSlider != null) musicSlider.value = savedMusic;
        if(sfxSlider != null) sfxSlider.value = savedSFX;

        // 3. Terapkan volumenya ke Audio Mixer
        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }

    public void SetMasterVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f) mainMixer.SetFloat("MasterVol", -80f);
        else mainMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);

        // SAVE POSISI SLIDER KE MEMORY
        PlayerPrefs.SetFloat("MasterVol", sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f) mainMixer.SetFloat("MusicVol", -80f);
        else mainMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        // SAVE POSISI SLIDER KE MEMORY
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f) mainMixer.SetFloat("SFXVol", -80f);
        else mainMixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);

        // SAVE POSISI SLIDER KE MEMORY
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }
}