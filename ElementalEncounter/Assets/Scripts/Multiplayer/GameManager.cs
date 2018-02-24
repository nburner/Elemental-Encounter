using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkGame
{
    public class GameManager : Photon.PunBehaviour
    {
        NetworkBoardManager board;
        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("BreakGameMultiplayer");
        }

        #region Photon Messages
        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void SendMove(int x, int y, int fromX, int fromY)
        {
            int[] aData = { x, y, fromX, fromY };
            PhotonNetwork.RaiseEvent(0, aData, true, null);
        }


        #endregion


        #region Public Methods
        void Awake()
        {
            board = FindObjectOfType(typeof(NetworkBoardManager)) as NetworkBoardManager;
            PhotonNetwork.OnEventCall += this.OnEvent;
        }

        public void OnEvent(byte eventcode, object content, int senderid)
        {
            int[] data = content as int[];
            board.MoveBreakman(data[0], data[1], data[2], data[3]);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion
    }
}

