using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Make sure the scene name matches exactly with your room scene
        SceneManager.LoadScene("SampleScene"); 
    }
}