using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject quitPanel;

    AsyncOperation loadingScene;

    public GameObject mainMenu;

    private GameCore gameCore;
    void Start()
    {
        quitPanel.SetActive(false);
        loadingScene = SceneManager.LoadSceneAsync("BreakGame", LoadSceneMode.Additive);
    }

    void Awake()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
    }

    public void singlePlayerButtonClick()
    {
        StartCoroutine(PlaySinglePlayerGame());
    }

    public IEnumerator PlaySinglePlayerGame()
    {
        string time = DateTime.Now.ToString("h:mm:ss tt");

        Debug.Log("started loading GameScene at" + time);
        

        //gameCore.MySide = GameCore.Turn.ICE;
        //gameCore.aILevel = GameCore.AILevel.Intermediate;
        gameCore.isMasterClient = true;
        gameCore.isSinglePlayer = true;

        while (!loadingScene.isDone)
        {
            Debug.Log(loadingScene.progress);
            yield return null;
        }
       
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BreakGame"));
        SceneManager.UnloadSceneAsync("MainMenu");
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
