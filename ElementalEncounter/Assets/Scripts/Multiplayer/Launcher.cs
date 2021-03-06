﻿using System;
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
        public ToggleGroup mapToggleGroup;
        public ToggleGroup turnToggleGroup;


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
            GameObject core = GameObject.Find("GameCore"); 
            if(core == null) gameCore = new GameObject("GameCore").AddComponent<GameCore>();
            else gameCore = core.GetComponent<GameCore>();
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
            SetMapChoice();
            SetTurnChoice();
            gameCore.isMasterClient = true;
            if (PhotonNetwork.playerName == "Player") PhotonNetwork.playerName = "Host";
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
            lc.DisplayHostGamePanel();
        }

        public void OnJoinGame_Click()
        {
            SetMapChoice();
            gameCore.isMasterClient = false;
            if (PhotonNetwork.playerName == "Player") PhotonNetwork.playerName = "Client";
            PhotonNetwork.JoinRandomRoom();
            lc.DisplayJoinGamePanel();
        }

        public void OnMainMenu_Click()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("MainMenu");
        }

        #endregion

        private void SetMapChoice()
        {
            IEnumerable<Toggle> mapToggles = mapToggleGroup.ActiveToggles();
            string mapText = "";
            foreach (var toggle in mapToggles)
            {
                if (toggle.enabled)
                {
                    mapText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
                }
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
        }

        private void SetTurnChoice()
        {
            IEnumerable<Toggle> turnToggles = turnToggleGroup.ActiveToggles();
            string turnText = "";
            foreach (var toggle in turnToggles)
            {
                if (toggle.enabled)
                {
                    turnText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
                }
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
        }

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
            lc.DisplayErrorPanel();
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            Debug.Log(cause);
            lc.DisplayErrorPanel();
            PhotonNetwork.Disconnect();
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

        public void CancelHost()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #endregion
    }
}