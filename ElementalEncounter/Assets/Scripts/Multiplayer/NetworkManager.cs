using System;
using System.Collections;
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

            //if (!PhotonNetwork.isMasterClient)
            //{
            //    gameCore.MySide = GameCore.Turn.FIRE;
            //    Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            //}
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
            BoardManager.Instance.panelContainer.SetActive(true);
            BoardManager.Instance.mapPanel.SetActive(true);
            BoardManager.Instance.DisconnectPanel.SetActive(true);
            BoardManager.Instance.isMyTurn = false;
            //LeaveRoom();

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            }

        }
        public override void OnDisconnectedFromPhoton()
        {
            LeaveToMainMenu();
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

        public void LeaveToMainMenu()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("MainMenu");
        }

        void OnEnable()
        {
            PhotonNetwork.OnEventCall += this.OnEvent;
        }
        void OnDisable()
        {
            PhotonNetwork.OnEventCall -= this.OnEvent;
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

        public void TimeOut()
        {
            PhotonNetwork.RaiseEvent(2, null, true, null);
        }
        public void WhatSide(int side)
        {
            int[] aData = { side };
            PhotonNetwork.RaiseEvent(3, aData, true, null);
        }
        public void RequestSide()
        {
            PhotonNetwork.RaiseEvent(4, null, true, null);
        }
        public void SendMessageChat(string aData)
        {
            PhotonNetwork.RaiseEvent(5, aData, true, null);
        }

        public void OnEvent(byte eventcode, object content, int senderid)
        {
            int[] data = content as int[];
            string dataMessage = content as string;

            if (eventcode == 0)
            {
                Move opponentMove = new Move(new Coordinate(data[0], data[1]), new Coordinate(data[2], data[3]));
                BoardManager.Instance.gameCore.UpdateBoard(opponentMove);
                BoardManager.Instance.Timer.SetActive(true);
            }
            if (eventcode == 1)
            {
                BoardManager.Instance.EndGame();
            }
            if (eventcode == 2)
            {
                BoardManager.Instance.ReceiveTimeOut();
            }
            if(eventcode == 3)
            {
                int mySide = data[0];
                BoardManager.Instance.ReceiveSide(mySide);
            }
            if(eventcode == 4)
            {
                BoardManager.Instance.SendSide();
            }
            if(eventcode == 5)
            {
                BoardManager.Instance.ReceiveMessage(dataMessage);
            }
        }

        #endregion
    }
}

