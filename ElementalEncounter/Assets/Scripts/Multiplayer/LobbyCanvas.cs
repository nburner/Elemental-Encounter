using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject menuPanel;
    public GameObject hostGamePanel;
    public GameObject joinGamePanel;
    public GameObject connectionGamePanel;
    public GameObject launcher;
    // Use this for initialization
    void Start ()
    {
        DisplayMenuPanel();
        launcher = GameObject.Find("Launcher");
        NetworkGame.Launcher launcherobj = launcher.GetComponent<NetworkGame.Launcher>();
        launcherobj.Connect();
    }

    public void DisplayMenuPanel()
    {
        menuPanel.SetActive(true);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(false);
        connectionGamePanel.SetActive(false);
    }

    public void DisplayHostGamePanel()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(true);
        joinGamePanel.SetActive(false);
        connectionGamePanel.SetActive(false);
    }

    public void DisplayJoinGamePanel()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(true);
        connectionGamePanel.SetActive(false);
    }

    public void DisplayConnectionPanel()
    {
        menuPanel.SetActive(false);
        hostGamePanel.SetActive(false);
        joinGamePanel.SetActive(true);
        connectionGamePanel.SetActive(false);
    }
}
