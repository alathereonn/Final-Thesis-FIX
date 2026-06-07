using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    public Image blackScreen;
    public float fadeSpeed = 12f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        blackScreen.color = new Color(0, 0, 0, 0);
        blackScreen.raycastTarget = false; 
    }

    // JURUS UPGRADE: Sekarang dia nerima 2 perintah (Tengah gelap & Akhir terang)
    public void DoTransition(Action midAction, Action onComplete = null)
    {
        StartCoroutine(TransitionRoutine(midAction, onComplete));
    }

    private IEnumerator TransitionRoutine(Action midAction, Action onComplete)
    {
        blackScreen.raycastTarget = true; // Kunci layar biar tombol ga bisa diklik paksa

        // 1. FASE GELAP
        while (blackScreen.color.a < 1f)
        {
            Color c = blackScreen.color;
            c.a += fadeSpeed * Time.deltaTime;
            blackScreen.color = c;
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 1f);

        // EKSEKUSI TENGAH (Matiin/Nyalain Canvas 2D)
        if (midAction != null) midAction.Invoke();

        yield return new WaitForSeconds(0.1f); // Tahan kedipnya bentar

        // 2. FASE TERANG
        while (blackScreen.color.a > 0f)
        {
            Color c = blackScreen.color;
            c.a -= fadeSpeed * Time.deltaTime;
            blackScreen.color = c;
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        
        blackScreen.raycastTarget = false; // Buka kunci layar

        // EKSEKUSI AKHIR (Mundurin kamera setelah terang)
        if (onComplete != null) onComplete.Invoke(); 
    }
}