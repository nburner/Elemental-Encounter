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
        public LobbyCanvas lc;
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
            // We need this to be true becuase we do NOT want random matchmaking
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
            lc = GameObject.Find("Canvas").GetComponent<LobbyCanvas>();
            lc.DisplayProgressPanel();
            Connect();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempt connection to Photon Cloud Network. If already connected, join random room.
        /// </summary>
        public void Connect()
        {
            isConnecting = true;            
            if (!PhotonNetwork.connected)
            {
                //Connect to online server
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        public void CreateNewRoom()
        {
            PhotonNetwork.CreateRoom(roomName.text, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
            lc.DisplayProgressPanel();
        }

        public void JoinARoom()
        {
            lc.DisplayJoinRoomPanel();
        }

        public override void OnJoinedLobby()
        {
            lc.DisplayOptionsPanel();
        }

        public override void OnDisconnectedFromPhoton()
        {
            lc.DisplayProgressPanel();
        }

        public override void OnJoinedRoom()
        {
            lc.DisplayProgressPanel();
            if (PhotonNetwork.room.PlayerCount == MaxPlayersPerRoom)
            {
                PhotonNetwork.LoadLevel("BreakGame");
            }
        }

        #endregion
    }
}