using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{ 
    public static NetworkGameManager Instance { get; set; }
    public GameObject menu;
    public GameObject hostMenu;
    public GameObject clientMenu;

    void Start ()
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
        menu.SetActive(false);
        hostMenu.SetActive(true);
    }

    public void OnJoinButtonClick()
    {
        Debug.Log("Joining Server...");
    }

    public void OnBackButtonClick()
    {
        menu.SetActive(true);
        hostMenu.SetActive(false);
        clientMenu.SetActive(false);
    }
}
