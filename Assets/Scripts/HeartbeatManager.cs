using UnityEngine;

public class HeartbeatManager : MonoBehaviour
{
    [Header("Monsters to Monitor")]
    public MonsterAI doorMonster;
    public MonsterAI windowMonster;
    public MonsterAI toiletMonster;

    [Header("Heartbeat Audio")]
    public AudioSource heartbeatSource;
    
    [Tooltip("Isi kasetnya: Index 0 (Stage 1 - Pelan), Index 1 (Stage 2 - Sedang), Index 2 (Stage 3 - Kencang)")]
    public AudioClip[] heartbeatClips = new AudioClip[3]; 

    private int currentHighestStage = 0;

    void Update()
    {
        // 1. Cek stage masing-masing monster saat ini
        int doorStage = doorMonster != null ? doorMonster.currentStage : 0;
        int windowStage = windowMonster != null ? windowMonster.currentStage : 0;
        int toiletStage = toiletMonster != null ? toiletMonster.currentStage : 0;

        // 2. Cari tahu angka stage yang paling tinggi di antara ketiganya (Mathf.Max otomatis milih angka terbesar)
        int highestStage = Mathf.Max(doorStage, Mathf.Max(windowStage, toiletStage));

        // 3. Kalau ada perubahan level bahaya, ganti kasetnya!
        if (highestStage != currentHighestStage)
        {
            currentHighestStage = highestStage;
            UpdateHeartbeatAudio();
        }
    }

    void UpdateHeartbeatAudio()
    {
        // Kalau semua monster aman (Stage 0), matiin detak jantung
        if (currentHighestStage == 0)
        {
            heartbeatSource.Stop();
            return;
        }

        // Kalau ada bahaya (Stage 1, 2, atau 3), putar kaset yang sesuai levelnya
        int index = currentHighestStage - 1;
        if (index >= 0 && index < heartbeatClips.Length)
        {
            if (heartbeatClips[index] != null)
            {
                heartbeatSource.clip = heartbeatClips[index];
                heartbeatSource.Play(); // Langsung mainkan loop jantungnya
            }
        }
    }
}