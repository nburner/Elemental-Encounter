using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControler : MonoBehaviour {

    public string startGame;

	public void PlayGame()
    {
        SceneManager.LoadScene(startGame);
    }

    public void PlayMultiplayerGame()
    {

    }

    public void OptionsMenu()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
