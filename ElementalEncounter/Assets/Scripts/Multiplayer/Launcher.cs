using UnityEngine;


namespace NetworkGame
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Public Variables



        #endregion

        public PhotonLogLevel LogLevel = PhotonLogLevel.ErrorsOnly;
        public byte MaxPlayersPerRoom = 2;
        public GameObject controlPanel;
        public GameObject progressLabel;

        #region Private Variables

        // Update the game version everytime we create a new install
        string _gameVersion = "0.1.0";

        #endregion


        #region MonoBehaviour CallBacks

        void Awake()
        {
            // No need to be in lobby to view list of available games
            PhotonNetwork.autoJoinLobby = false;
            
            // Allows all clients in same room to have their levels synced
            PhotonNetwork.automaticallySyncScene = true;

            PhotonNetwork.logLevel = LogLevel;
        }

        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempt connection to Photon Cloud Network. If already connected, join random room.
        /// </summary>
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // If connected, join random room
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // Connect to online server
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() called.");
            PhotonNetwork.JoinRandomRoom();
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
            Debug.Log("OnJoinedRoom() called.");
        }

        #endregion
    }
}