using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject controlPanel;
    public GameObject progressLabel;
    public GameObject optionsPanel;
    public GameObject createRoomPanel;
    public GameObject joinRoomPanel;

    // Use this for initialization
    void Start ()
    {
		
	}

    public void DisplayOptionsPanel()
    {
        optionsPanel.SetActive(true);
        controlPanel.SetActive(false);
        progressLabel.SetActive(false);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }

    public void DisplayProgressPanel()
    {
        optionsPanel.SetActive(false);
        controlPanel.SetActive(false);
        progressLabel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }

    public void DisplayCreateRoomPanel()
    {
        optionsPanel.SetActive(false);
        controlPanel.SetActive(false);
        progressLabel.SetActive(false);
        createRoomPanel.SetActive(true);
        joinRoomPanel.SetActive(false);
    }

    public void DisplayJoinRoomPanel()
    {
        optionsPanel.SetActive(false);
        controlPanel.SetActive(false);
        progressLabel.SetActive(false);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(true);
    }
}
