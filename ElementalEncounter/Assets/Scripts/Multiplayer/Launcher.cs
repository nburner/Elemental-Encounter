using UnityEngine;
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
        private string _gameVersion = "0.1.0";
        private GameCore gameCore;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            // Speeds up client connection
            PhotonNetwork.autoJoinLobby = true;          
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
                Debug.Log("Connected");
            }
        }

        #region Button Click Methods

        public void OnHostGame_Click()
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }

        public void OnJoinGame_Click()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        #endregion

        #region Photon Methods

        /// <summary>
        /// Called when PhotonNetwork.autoJoinLobby is set to false.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon server.");
        }

        public override void OnDisconnectedFromPhoton()
        {
            Debug.Log("Disconnected from Photon server.");
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            Debug.Log(cause);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Room created successfully.");
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
                BoardManager.Instance.gameCore = gameCore;
            }
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("Failed to join random room.");
            PhotonNetwork.LoadLevel("Game_Lobby");
        }

        #endregion

        #endregion
    }
}