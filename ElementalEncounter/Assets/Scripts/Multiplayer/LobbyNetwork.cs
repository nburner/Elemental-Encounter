using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyNetwork : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Button hostGameButton;

    void Start()
    {
        hostGameButton.enabled = false;
        print("Connecting to server...");
        PhotonNetwork.ConnectUsingSettings("0.1.0");
    }

    void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.playerName = PlayerNetwork.Instance.PlayerName;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    void OnJoinedLobby()
    {
        print("Joined lobby.");
        hostGameButton.enabled = true;
    }
}
