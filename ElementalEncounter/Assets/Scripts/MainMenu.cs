using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlaySinglePlayerGame()
    {
        SceneManager.LoadScene("BreakGame");
    }
    public void PlayMultiplayerGame()
    {
        SceneManager.LoadScene("BreakGameMultiplayer");
    }

    public void QuitGame()
    {
        Debug.Log("Log");
        Application.Quit();

    }
}
