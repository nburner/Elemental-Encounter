using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkGameManager : MonoBehaviour
{ 
    public static NetworkGameManager Instance { get; set; }
    public GameObject menu;
    public GameObject hostMenu;
    public GameObject clientMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public InputField nameInput;

    private const int PORT_NUMBER = 6321;

    void Start()
    {
        Instance = this;
        hostMenu.SetActive(false);
        clientMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
	}

    public void OnConnectButtonClick()
    {
        menu.SetActive(false);
        clientMenu.SetActive(true);
    }

    public void OnHostButtonClick()
    {
        try
        {
            NetworkServer s = Instantiate(serverPrefab).GetComponent<NetworkServer>();
            s.Init();

            NetworkClient c = Instantiate(clientPrefab).GetComponent<NetworkClient>();
            c.clientName = nameInput.text;
            c.isHost = true;
            if (c.clientName == "")
            {
                c.clientName = "Host";
            }
            c.ConnectToServer("localhost", PORT_NUMBER);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        menu.SetActive(false);
        hostMenu.SetActive(true);
    }

    public void OnJoinButtonClick()
    {
        string hostAddress = GameObject.Find("ServerName").GetComponent<InputField>().text;
        if (hostAddress == "")
        {
            hostAddress = "localhost";
        }

        try
        {
            NetworkClient c = Instantiate(clientPrefab).GetComponent<NetworkClient>();
            c.clientName = nameInput.text;
            if (c.clientName == "")
            {
                c.clientName = "Client";
            }
            c.ConnectToServer(hostAddress, PORT_NUMBER);
            clientMenu.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void OnBackButtonClick()
    {
        menu.SetActive(true);
        hostMenu.SetActive(false);
        clientMenu.SetActive(false);

        NetworkServer s = FindObjectOfType<NetworkServer>();
        if (s != null)
        {
            Destroy(s.gameObject);
        }

        NetworkClient c = FindObjectOfType<NetworkClient>();
        if (c != null)
        {
            Destroy(c.gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BreakGame");
    }
}
