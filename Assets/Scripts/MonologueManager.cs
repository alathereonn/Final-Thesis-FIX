using UnityEngine;
using TMPro;
using System.Collections;

public class MonologueManager : MonoBehaviour
{
    public static MonologueManager instance; 

    [Header("UI References")]
    public TextMeshProUGUI subtitleText;

    [Header("Settings")]
    public float defaultDisplayTime = 3f; 

    private Coroutine activeMonologue;

    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Pastikan pas game mulai, teksnya bersih dan ngumpet dulu
        if (subtitleText != null) 
        {
            subtitleText.text = ""; 
            subtitleText.gameObject.SetActive(false); 
        }
    }

    public void ShowMonologue(string message, float duration = 0f)
    {
        if (activeMonologue != null)
        {
            StopCoroutine(activeMonologue);
        }

        float timeToDisplay = (duration > 0f) ? duration : defaultDisplayTime;
        activeMonologue = StartCoroutine(DisplayRoutine(message, timeToDisplay));
    }

    IEnumerator DisplayRoutine(string message, float duration)
    {
        if (subtitleText != null)
        {
            subtitleText.text = message;
            
            // ---> INI KUNCINYA BANG! PAKSA MUNCULIN OBJEKNYA <---
            subtitleText.gameObject.SetActive(true); 
        }

        // Pake Realtime biar kebal dari sistem pause/time freeze
        yield return new WaitForSecondsRealtime(duration);

        if (subtitleText != null)
        {
            subtitleText.text = "";
            
            // ---> MATIIN LAGI BIAR RAPI <---
            subtitleText.gameObject.SetActive(false); 
        }
    }
}