using UnityEngine;
using UnityEditor;

public class ResetSaveData
{
    // Ini bakal bikin tombol baru di menu bar Unity abang bagian atas
    [MenuItem("Tools/Reset Save Data (Clear PlayerPrefs)")]
    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🔥 SIP BANG! Semua data save, progress malam, dan foto Gallery udah di-reset bersih!");
    }
}