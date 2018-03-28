﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkGame
{
    public class NetworkManager : Photon.PunBehaviour
    {
        public enum Turn { ICE, FIRE };
        private GameCore gameCore;
        void LoadArena()
        {
            gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();

            if (!PhotonNetwork.isMasterClient)
            {
                gameCore.MySide = GameCore.Turn.FIRE;
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            gameCore.isSinglePlayer = false;
            Debug.Log("PhotonNetwork : Loading Level ");
            PhotonNetwork.LoadLevel("BreakGame");
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
        public bool IsConnected()
        {
            return PhotonNetwork.connected;
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
            SceneManager.LoadScene("Game_Lobby");
        }

        void Awake()
        {
            PhotonNetwork.OnEventCall += this.OnEvent;
        }

        public void SendMove(Move move, GameCore.Turn T)
        {
            int[] aData = { move.From.X, move.From.Y, move.To.X, move.To.Y, (int)T };
            PhotonNetwork.RaiseEvent(0, aData, true, null);
        }

        public void SendEndGame()
        {
            PhotonNetwork.RaiseEvent(1, null, true, null);
        }

        public void OnEvent(byte eventcode, object content, int senderid)
        {
            int[] data = content as int[];
            if (eventcode == 0)
            {
                Move opponentMove = new Move(new Coordinate(data[0], data[1]), new Coordinate(data[2], data[3]));
                Coordinate From = new Coordinate(data[0], data[1]);
                Coordinate To = new Coordinate(data[2], data[3]);

                BoardManager.Instance.GetNetworkMove(From, To);
            }
            if (eventcode == 1)
            {
                BoardManager.Instance.EndGame();
            }

        }
        #endregion
    }
}

