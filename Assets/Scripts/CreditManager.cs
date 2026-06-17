using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        // PENTING: Pastikan "MainMenu" ini sama persis dengan nama scene menu utamamu
        SceneManager.LoadScene("MainMenu"); 
    }
}