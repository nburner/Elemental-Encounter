﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject quitPanel;

    void Start()
    {
        quitPanel.SetActive(false);
    }

    public void PlaySinglePlayerGame()
    {
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
