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
        //gameCore.MySide = GameCore.Turn.ICE;
        //gameCore.aILevel = GameCore.AILevel.Intermediate;
        gameCore.isMasterClient = true;
        gameCore.isSinglePlayer = true;
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
