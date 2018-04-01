using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NetworkGame
{
    public class Launcher : Photon.PunBehaviour 
    {

        #region Public Variables
        public PhotonLogLevel LogLevel = PhotonLogLevel.ErrorsOnly;
        public byte MaxPlayersPerRoom = 2;
        public LobbyCanvas lc;

        #endregion

        #region Private Variables

        // Update the game version everytime we create a new install
        private string _gameVersion = "1";
        private bool connectionStatus;
        public GameCore gameCore;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            lc = GameObject.Find("Canvas").GetComponent<LobbyCanvas>();
            if (!PhotonNetwork.connected) { lc.DisplayConnectionPanel();}
            else { lc.DisplayMenuPanel(); }
            // Speeds up client connection
            PhotonNetwork.autoJoinLobby = true;          
            // Allows all clients in same room to have their levels synced
            PhotonNetwork.automaticallySyncScene = false;

            PhotonNetwork.logLevel = LogLevel;
            //Connect to online server
            //StartCoroutine(CheckInternetConnection((isConnected) =>
            //{
            //    // handle connection status here
            //    lc.DisplayErrorPanel();
            //}));
            //if (connectionStatus) lc.DisplayErrorPanel();
            //else Connect();
        }

        void Start()
        {
            gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
            gameCore.isSinglePlayer = false;
            Connect();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempt connection to Photon Cloud Network.
        /// </summary>
        public void Connect()
        {
            if (!PhotonNetwork.connected)
            {
                //Connect to online server
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #region Button Click Methods

        public void OnHostGame_Click()
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
            gameCore.MySide = GameCore.Turn.ICE;
            lc.DisplayHostGamePanel();
        }

        public void OnJoinGame_Click()
        {
            PhotonNetwork.JoinRandomRoom();
            lc.DisplayJoinGamePanel();
        }

        public void OnMainMenu_Click()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("MainMenu");
        }

        #endregion

        #region Photon Methods

        /// <summary>
        /// Called when PhotonNetwork.autoJoinLobby is set to false.
        /// </summary>
        public override void OnJoinedLobby()
        {
            lc.DisplayMenuPanel();
            Debug.Log("Connected to Photon server.");
        }

        public override void OnDisconnectedFromPhoton()
        {
            Debug.Log("Disconnected from Photon server.");
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            Debug.Log(cause);
            lc.DisplayErrorPanel();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Room created successfully.");
        }

        IEnumerator CheckInternetConnection(Action<bool> action)
        {
            WWW www = new WWW("http://google.com");
            yield return www;
            if (www.error != null)
            {
                action(false);
            }
            else
            {
                action(true);
            }
        }

        /// <summary>
        /// Called when a player has joined a room. When two players have connected, load the game scene.
        /// The level will automatically load for both the MasterClient and the Client because
        /// PhotonNetwork.automaticallySyncScene is set to true.
        /// </summary>
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.isMasterClient)
            {
                gameCore.MySide = GameCore.Turn.ICE;
            }
            else
            {
                gameCore.MySide = GameCore.Turn.FIRE;
            }

            if (PhotonNetwork.room.PlayerCount == MaxPlayersPerRoom)
            {
                PhotonNetwork.LoadLevel("BreakGame");
            }
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("Failed to join random room.");
            PhotonNetwork.LoadLevel("Game_Lobby");
        }

        public void ReturnToMainMenu()
        {
            PhotonNetwork.LoadLevel("MainMenu");
        }

        #endregion

        #endregion
    }
}