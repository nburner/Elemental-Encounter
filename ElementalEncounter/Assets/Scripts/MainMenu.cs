using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject quitPanel;

    private GameCore gameCore;
    void Start()
    {
        quitPanel.SetActive(false);
    }

    void Awake()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
    }

    public void PlaySinglePlayerGame()
    {
        //gameCore.StartSinglePlayerGame(GameCore.Turn.ICE, GameCore.AILevel.Intermediate);
        SceneManager.LoadScene("BreakGame");
    }
    public void PlayMultiplayerGame()
    {
        SceneManager.LoadScene("Game_Lobby");
    }

    public void ShowQuitGamePanel()
    {
        quitPanel.SetActive(true);
    }

    public void PlayerConfirmedQuit()
    {
        Application.Quit();
    }

    public void PlayerDeniedQuit()
    {
        quitPanel.SetActive(false);
    }
}
