﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject quitPanel;

    AsyncOperation loadingScene;

    public GameObject mainMenu;
    public GameObject gameOptions;
    public ToggleGroup aiToggleGroup;
    public ToggleGroup mapToggleGroup;
    public ToggleGroup turnToggleGroup;

    //private bool startGame;

    private GameCore gameCore;
    void Start()
    {
        mainMenu.SetActive(true);
        gameOptions.SetActive(false);
        quitPanel.SetActive(false);
        //startGame = false;
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
        //startGame = true;
        //gameCore.isSinglePlayer = true;
        //gameCore.aILevel = GameCore.AILevel.Intermediate;
        //gameCore.MySide = GameCore.Turn.ICE;
        //SceneManager.LoadScene("BreakGame");

        mainMenu.SetActive(false);
        gameOptions.SetActive(true);
    }

    public void StartGame()
    {
        IEnumerable<Toggle> aiToggles = aiToggleGroup.ActiveToggles();
        IEnumerable<Toggle> mapToggles = mapToggleGroup.ActiveToggles();
        IEnumerable<Toggle> turnToggles = turnToggleGroup.ActiveToggles();
        string aiText = "";
        string mapText = "";
        string turnText = "";
        foreach (var toggle in aiToggles)
        {
            if (toggle.enabled)
            {
                aiText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
            }
        }
        foreach (var toggle in mapToggles)
        {
            if (toggle.enabled)
            {
                mapText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
            }
        }
        foreach (var toggle in turnToggles)
        {
            if (toggle.enabled)
            {
                turnText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
            }
        }

        switch (aiText)
        {
            case "Easy":
                gameCore.aILevel = GameCore.AILevel.Easy;
                break;
            case "Intermediate":
                gameCore.aILevel = GameCore.AILevel.Intermediate;
                break;
        }

        switch (mapText)
        {
            case "Ice":
                gameCore.Map = GameCore.MapChoice.ICE;
                break;
            case "Fire":
                gameCore.Map = GameCore.MapChoice.FIRE;
                break;
            case "Clash":
                gameCore.Map = GameCore.MapChoice.CLASH;
                break;
        }

        switch (turnText)
        {
            case "Ice":
                gameCore.MySide = GameCore.Turn.ICE;
                break;
            case "Fire":
                gameCore.MySide = GameCore.Turn.FIRE;
                break;
        }
        gameCore.isSinglePlayer = true;

        SceneManager.LoadScene("BreakGame");
    }

    //public IEnumerator PlaySinglePlayerGame()
    //{
    //   // string time = DateTime.Now.ToString("h:mm:ss tt");

    //    //Debug.Log("started loading GameScene at" + time);


    //    //gameCore.MySide = GameCore.Turn.ICE;
    //    //gameCore.aILevel = GameCore.AILevel.Intermediate;
    //    gameCore.isSinglePlayer = true;

    //    while (!loadingScene.isDone)
    //    {
    //        Debug.Log(loadingScene.progress);

    //        if (startGame == true) loadingScene.allowSceneActivation = true;

    //        yield return null;
    //    }

    //    SceneManager.SetActiveScene(SceneManager.GetSceneByName("BreakGame"));
    //    SceneManager.UnloadSceneAsync("MainMenu");
    //}
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
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        gameOptions.SetActive(false);
        quitPanel.SetActive(false);
    }
}
