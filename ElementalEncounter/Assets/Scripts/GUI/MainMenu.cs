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

    private bool startGame;

    private GameCore gameCore;
    void Start()
    {
        quitPanel.SetActive(false);
        startGame = false;
        //loadingScene = SceneManager.LoadSceneAsync("BreakGame", LoadSceneMode.Additive);

        //StartCoroutine(PlaySinglePlayerGame());
        //loadingScene.allowSceneActivation = false;
    }

    void Awake()
    {
        GameObject core = GameObject.Find("GameCore");
        if (core == null)
        {
            gameCore = new GameObject("GameCore").AddComponent<GameCore>();
        }
        else gameCore = core.GetComponent<GameCore>();
    }

    public void singlePlayerButtonClick()
    {
        startGame = true;
        gameCore.isSinglePlayer = true;
        gameCore.aILevel = GameCore.AILevel.Intermediate;
        gameCore.MySide = GameCore.Turn.ICE;
        SceneManager.LoadScene("BreakGame");
    }

    public IEnumerator PlaySinglePlayerGame()
    {
       // string time = DateTime.Now.ToString("h:mm:ss tt");

        //Debug.Log("started loading GameScene at" + time);
        

        //gameCore.MySide = GameCore.Turn.ICE;
        //gameCore.aILevel = GameCore.AILevel.Intermediate;
        gameCore.isSinglePlayer = true;

        while (!loadingScene.isDone)
        {
            Debug.Log(loadingScene.progress);

            if (startGame == true) loadingScene.allowSceneActivation = true;

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
