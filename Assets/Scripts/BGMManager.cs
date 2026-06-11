using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;
    
    // TAMBAHAN: Kita butuh AudioSource buat muter musiknya
    public AudioSource musicSource; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return; // Penting biar kode di bawah gak jalan di objek yang hancur
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Logika bungkam musik di scene game
        if (scene.name == "SampleScene") 
        {
            if (musicSource != null && musicSource.isPlaying) musicSource.Stop();
        }
        // Logika nyalain musik balik kalau abang balik ke menu
        else if (scene.name == "MainMenu" && musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}