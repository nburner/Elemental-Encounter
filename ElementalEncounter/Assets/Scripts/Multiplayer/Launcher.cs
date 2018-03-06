using UnityEngine;
using UnityEngine.UI;

namespace NetworkGame
{
    public class Launcher : Photon.PunBehaviour 
    {
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        #region Public Variables
        public PhotonLogLevel LogLevel = PhotonLogLevel.ErrorsOnly;
        public byte MaxPlayersPerRoom = 2;
        public GameObject controlPanel;
        public GameObject progressLabel;
        public GameObject optionsPanel;
        public GameObject hostGamePanel;
        public GameObject connectToGamePanel;
        public InputField roomName;

        #endregion

        #region Private Variables
        // Update the game version everytime we create a new install
        private string _gameVersion = "0.1.0";
        private GameCore gameCore;

        #endregion


        #region MonoBehaviour CallBacks

        void Awake()
        {
            // No need to be in lobby to view list of available games
            PhotonNetwork.autoJoinLobby = false;
            
            // Allows all clients in same room to have their levels synced
            PhotonNetwork.automaticallySyncScene = true;

            PhotonNetwork.logLevel = LogLevel;
            GameObject core = GameObject.Find("GameCore");
            if (core == null)
            {
                gameCore = new GameObject("Temp Game Core").AddComponent<GameCore>();
                gameCore.isSinglePlayer = false;
                gameCore.aILevel = GameCore.AILevel.Easy;
                gameCore.MySide = GameCore.Turn.ICE;
            }
            else gameCore = core.GetComponent<GameCore>();
        }

        void Start()
        {
            //controlPanel.SetActive(true);
            optionsPanel.SetActive(true);
            progressLabel.SetActive(false);
            hostGamePanel.SetActive(false);
            connectToGamePanel.SetActive(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempt connection to Photon Cloud Network. If already connected, join random room.
        /// </summary>
        public void Connect()
        {
            isConnecting = true;
            //progressLabel.SetActive(true);
            optionsPanel.SetActive(false);

            //If connected, join random room
            if (PhotonNetwork.connected)
            {
                //PhotonNetwork.JoinRandomRoom();
                optionsPanel.SetActive(false);
                hostGamePanel.SetActive(true);
            }
            else
            {
                //Connect to online server
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        public void CreateNewRoom()
        {
            PhotonNetwork.CreateRoom(roomName.text, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
            hostGamePanel.SetActive(false);
            progressLabel.SetActive(true);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() called.");
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
                //PhotonNetwork.JoinRandomRoom();
                optionsPanel.SetActive(false);
                hostGamePanel.SetActive(true);
            }
        }

        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            Debug.LogWarning("OnDisconnectedFromPhoton() called.");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }

        public override void OnJoinedRoom()
        {
            // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.room.PlayerCount == 2)
            {
                // #Critical
                // Load the Room Level. 
                PhotonNetwork.LoadLevel("BreakGame");
                gameCore.isMasterClient = true;
                gameCore.isSinglePlayer = true;
            }
        }

        #endregion
    }
}