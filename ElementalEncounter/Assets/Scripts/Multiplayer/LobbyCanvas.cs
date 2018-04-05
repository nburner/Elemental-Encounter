using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvas : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject menuPanel;
    public GameObject hostGamePanel;
    public GameObject joinGamePanel;
    public GameObject connectionGamePanel;
    public GameObject errorGamePanel;
    public GameObject gameOptions;

    public void DisplayMenuPanel()
    {
        menuPanel.SetActive(true);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(false);
        connectionGamePanel.SetActive(false);
        errorGamePanel.SetActive(false);
        gameOptions.SetActive(false);
    }

    public void DisplayHostGamePanel()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(true);
        joinGamePanel.SetActive(false);
        connectionGamePanel.SetActive(false);
        errorGamePanel.SetActive(false);
        gameOptions.SetActive(false);
    }

    public void DisplayJoinGamePanel()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(true);
        connectionGamePanel.SetActive(false);
        errorGamePanel.SetActive(false);
        gameOptions.SetActive(false);
    }

    public void DisplayConnectionPanel()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(false);
        connectionGamePanel.SetActive(true);
        errorGamePanel.SetActive(false);
        gameOptions.SetActive(false);
    }

    public void DisplayErrorPanel()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(false);
        connectionGamePanel.SetActive(false);
        errorGamePanel.SetActive(true);
        gameOptions.SetActive(false);
    }
    public void DisplayGameOptions()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(false);
        connectionGamePanel.SetActive(false);
        errorGamePanel.SetActive(false);
        gameOptions.SetActive(true);
    }
}
