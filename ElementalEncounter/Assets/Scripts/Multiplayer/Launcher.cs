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
            lc = GameObject.Find("Canvas").GetComponent<LobbyCanvas>();
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

        public void OnHostGame_Click()
        {
            PhotonNetwork.CreateRoom("Test", new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }

        public void OnJoinGame_Click()
        {
            lc.DisplayJoinGamePanel();
        }

        public override void OnConnectedToMaster()
        {
            lc.DisplayMenuPanel();
        }

        public override void OnDisconnectedFromPhoton()
        {

        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Room created successfully.");
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.room.PlayerCount == MaxPlayersPerRoom)
            {
                PhotonNetwork.LoadLevel("BreakGame");
            }
        }

        #endregion
    }
}