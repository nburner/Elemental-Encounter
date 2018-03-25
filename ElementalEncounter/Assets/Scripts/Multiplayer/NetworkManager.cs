using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkGame
{
    public class NetworkManager : Photon.PunBehaviour
    {
        private GameCore gameCore;
        void LoadArena()
        {
            GameObject core = GameObject.Find("GameCore");
            if (core == null)
            {
                gameCore = new GameObject("Temp Game Core").AddComponent<GameCore>();
                gameCore.isSinglePlayer = false;
                gameCore.aILevel = GameCore.AILevel.Easy;
            }
            else gameCore = core.GetComponent<GameCore>();

            if (!PhotonNetwork.isMasterClient)
            {
                gameCore.MySide = GameCore.Turn.FIRE;
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork : Loading Level ");
            PhotonNetwork.LoadLevel("BreakGameMultiplayer");
        }
        public GameCore GetGameCore()
        {
            return gameCore;
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
            }
            PhotonNetwork.LoadLevel("Game_Lobby");

        }
        public override void OnDisconnectedFromPhoton()
        {
            LeaveRoom();
        }
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Game_Lobby");
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion
    }
}

