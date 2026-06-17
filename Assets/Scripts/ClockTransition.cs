using UnityEngine;
using System.Collections;
using TMPro;

public class FnafTransition : MonoBehaviour
{
    [Header("Referensi Angka")]
    public RectTransform textAngka5;
    public RectTransform textAngka6;

    [Header("Referensi Tombol")]
    public GameObject continueButton;
    public GameObject backButton;

    [Header("Pengaturan Animasi")]
    public float waktuTungguAwal = 2f; 
    public float durasiAnimasi = 1.5f; 
    public float jarakGeser = 150f;    

    // Fungsi Start akan otomatis jalan ketika WinCanvas dinyalakan oleh GameManager
    void Start()
    {
        // Sembunyikan tombol di awal layar kemenangan muncul
        if (continueButton != null) continueButton.SetActive(false);
        if (backButton != null) backButton.SetActive(false);

        StartCoroutine(MulaiRollerFNAF());
    }

    IEnumerator MulaiRollerFNAF()
    {
        // Pakai Realtime agar tidak nge-freeze saat GameManager bikin Time.timeScale = 0
        yield return new WaitForSecondsRealtime(waktuTungguAwal);

        Vector2 posisiAwal5 = textAngka5.anchoredPosition;
        Vector2 posisiAwal6 = textAngka6.anchoredPosition;

        Vector2 posisiTarget5 = new Vector2(posisiAwal5.x, posisiAwal5.y + jarakGeser);
        Vector2 posisiTarget6 = new Vector2(posisiAwal6.x, posisiAwal5.y); 

        float waktu = 0f;

        while (waktu < durasiAnimasi)
        {
            // Pakai unscaledDeltaTime (waktu asli dunia nyata, kebal pause)
            waktu += Time.unscaledDeltaTime;
            
            float t = Mathf.SmoothStep(0f, 1f, waktu / durasiAnimasi);

            textAngka5.anchoredPosition = Vector2.Lerp(posisiAwal5, posisiTarget5, t);
            textAngka6.anchoredPosition = Vector2.Lerp(posisiAwal6, posisiTarget6, t);

            yield return null; 
        }

        textAngka5.anchoredPosition = posisiTarget5;
        textAngka6.anchoredPosition = posisiTarget6;

        yield return new WaitForSecondsRealtime(0.5f);

        // Munculkan tombol Continue & Back!
        if (continueButton != null) continueButton.SetActive(true);
        if (backButton != null) backButton.SetActive(true);
    }
}