// Help: https://www.youtube.com/playlist?list=PLLH3mUGkfFCVXrGLRxfhst7pffE9o2SQO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;

public class NetworkClient : MonoBehaviour
{
    public string clientName;
    public bool isHost;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private List<GameClient> players = new List<GameClient>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (System.Exception e)
        {
            Debug.Log("Socket error " + e.Message);
        }

        return socketReady;
    }
    

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }

    //Sende messages to the Server
    public void Send(string data)
    {
        if (!socketReady)
        {
            return;
        }
        writer.WriteLine(data);
        writer.Flush();
    }
    //Read messages from the server
    private void OnIncomingData(string data)
    {
        Debug.Log("Client:" + data);
        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "SWHO":
                for (int i = 0; i < aData.Length-1; i++)
                {
                    UserConnected(aData[i], false);
                }
                Send("CWHO|" + clientName + "|"+((isHost)?1:0).ToString());
                break;
            case "SCNN":
                UserConnected(aData[1], false);
                break;
            case "SMOV":
                if(!isHost)
                    BoardManager.Instance.MoveBreakmanNet(int.Parse(aData[1]), int.Parse(aData[2]),int.Parse(aData[3]), int.Parse(aData[4]));
                break;
        }
    }
    
    private void UserConnected(string name,bool host)
    {
        GameClient c = new GameClient();
        c.name = name;

        players.Add(c);

        if (players.Count == 3)
        {
            NetworkGameManager.Instance.StartGame();
        }

    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;
        writer.Close();
        reader.Close();
        socket.Close();

        socketReady = false;
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}
