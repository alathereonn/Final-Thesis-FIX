using UnityEngine;

public class DaySpawner : MonoBehaviour
{
    [Header("Muncul di Hari Ke Berapa?")]
    public int targetDay = 2; // Objek ini cuma muncul di Day 2

    void Start()
    {
        // Cek sekarang pemain lagi main di hari ke berapa (default 1)
        int currentDay = PlayerPrefs.GetInt("CurrentPlayNight", 1);

        // Kalau hari saat ini kurang dari target hari, sembunyikan objeknya
        if (currentDay < targetDay)
        {
            gameObject.SetActive(false);
        }
        else 
        {
            // Kalau harinya udah pas (Day 2 atau lebih), munculkan
            gameObject.SetActive(true);
        }
    }
}