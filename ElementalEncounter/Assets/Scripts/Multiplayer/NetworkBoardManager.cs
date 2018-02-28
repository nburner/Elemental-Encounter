using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBoardManager : Photon.PunBehaviour
{
    public enum Turn { ICE, FIRE };
    public static NetworkBoardManager Instance { set; get; }
    public char[,] AllowedMoves { set; get; }

    public PieceMultiplayer[,] BreakmanNet { set; get; }
    public bool[,] Spacesboard { set; get; }
    private PieceMultiplayer selectedBreakman;
    private bool isClicked = false;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private int selectionX = -1;
    private int selectionY = -1;

    //List of Gameobject for spawning pieces on the board
    public List<GameObject> breakmanPrefabs;
    public List<GameObject> boardPrefabs;
    private List<GameObject> activeBreakman;

    //public bool isIceTurn = true;
    //public bool hasMoved = true;
    public bool isMasterClient = PhotonNetwork.isMasterClient;
    public Turn WhoseTurn;
    private void Start()
    {
        Instance = this;
        SpawnAllBoardSpaces();
        SpawnAllBreakPieces();
        //if (isMasterClient)
        //{
        //    hasMoved = false;
        //    isIceTurn = true;
        WhoseTurn = Turn.ICE;
        //}
        //else
        //{
        //    WhoseTurn = Turn.FIRE;
        //    isIceTurn = false;
        //    hasMoved = true;
        //}
    }
    private void Update()
    {
        UpdateSelection();
        Selection();
    }

    void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }

    public void SendMove(int x, int y, int fromX, int fromY, Turn T)
    {
        int[] aData = { x, y, fromX, fromY , (int)T };
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
            MoveBreakman(data[0], data[1], data[2], data[3]);
            WhoseTurn = (Turn)data[4];
        }
        if (eventcode == 1)
        {
            EndGame();
        }

    }

    private void Selection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedBreakman == null)
                {
                    // select the Piece
                    if ((isMasterClient && WhoseTurn == Turn.ICE) || (!isMasterClient && WhoseTurn == Turn.FIRE))
                    {
                        SelectBreakMan(selectionX, selectionY);
                    }
                    Debug.Log("Selected");
                }
                else
                {
                    // Move the Piece
                    MoveBreakman(selectionX, selectionY);
                }
            }
        }
    }
    private void SelectBreakMan(int x, int y)
    {
        if (BreakmanNet[x, y] == null)
            return;

        if ((WhoseTurn == Turn.FIRE) == (BreakmanNet[x, y].isIce) )
            return;

        bool hasAtleastOneMove = false;
        AllowedMoves = BreakmanNet[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (AllowedMoves[i, j] != 0)
                    hasAtleastOneMove = true;
            }
        }
        if (!hasAtleastOneMove)
            return;

        selectedBreakman = BreakmanNet[x, y];
        BoardHighlights.Instance.HighlightAllowedMoves(AllowedMoves);

        if (!isClicked)
        {
            isClicked = !isClicked;
        }

    }

    private void MoveBreakman(int toX, int toY)
    {
        // TODO: Add winning conditions. Make in different functions.
        if (AllowedMoves[toX, toY] != 0)
        {
            PieceMultiplayer b = BreakmanNet[toX, toY];
            char temp = AllowedMoves[toX, toY];
            if (b != null && b.isIce != (WhoseTurn == Turn.ICE))
            {
                // Capture a piece
                activeBreakman.Remove(b.gameObject);
                Destroy(b.gameObject);
                playAnimation(selectedBreakman, temp, toX, toY, false);
            }

            if (WhoseTurn == Turn.ICE) {
                if (toY == 7) {
                    EndGame();
                    SendEndGame();
                    return;
                }
            }
            else {
                if (toY == 0) {
                    EndGame();
                    SendEndGame();
                    return;
                }
            }

            
            BreakmanNet[selectedBreakman.CurrentX, selectedBreakman.CurrentY] = null;
            playAnimation(selectedBreakman, temp, toX, toY, false);

            selectedBreakman.transform.position = GetTileCenter(toX, toY);
            selectedBreakman.SetPosition(toX, toY);
            BreakmanNet[toX, toY] = selectedBreakman;

            WhoseTurn = (WhoseTurn == Turn.ICE) ? Turn.FIRE : Turn.ICE;
            SendMove(toX, toY, selectedBreakman.CurrentX, selectedBreakman.CurrentY, WhoseTurn);
        }

        if (isClicked)
        {
            isClicked = !isClicked;
        }
        BoardHighlights.Instance.HideHighlights();
        selectedBreakman = null;
    }

    public void MoveBreakman(int toX, int toY, int fromX,int fromY)
    {
        bool takeAPiece = BreakmanNet[toX, toY] != null;
        if (takeAPiece) {
            activeBreakman.Remove(BreakmanNet[toX, toY].gameObject);
            Destroy(BreakmanNet[toX, toY].gameObject, .5f);
        }

        BreakmanNet[fromX, fromY].transform.position = GetTileCenter(toX, toY);
        BreakmanNet[fromX, fromY].SetPosition(toX, toY);
        BreakmanNet[toX, toY] = BreakmanNet[fromX, fromY];
        BreakmanNet[fromX, fromY] = null;

        //WhoseTurn = (WhoseTurn == Turn.ICE) ? Turn.FIRE : Turn.ICE; 
        if (toY == 0 || toY == 7) EndGame();
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("BreakPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }


    private void SpawnBreakman(int index, int x, int y)
    {
        GameObject go = Instantiate(breakmanPrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        BreakmanNet[x, y] = go.GetComponent<PieceMultiplayer>();
        BreakmanNet[x, y].SetPosition(x, y);
        activeBreakman.Add(go);
    }

    private void SpawnBoardSpace(int index, int x, int y)
    {
        GameObject another = Instantiate(boardPrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        another.transform.SetParent(transform);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void SpawnAllBreakPieces()
    {
        activeBreakman = new List<GameObject>();
        BreakmanNet = new PieceMultiplayer[8, 8];
        //spawn Ice team
        for (int i = 0; i < 8; i++)
        {
            SpawnBreakman(1, i, 0);
            SpawnBreakman(1, i, 1);
        }
        //spawn Fire team
        for (int i = 0; i < 8; i++)
        {
            SpawnBreakman(0, i, 6);
            SpawnBreakman(0, i, 7);
        }
    }

    private void SpawnAllBoardSpaces()
    {
        bool isBlackPiece = false;
        Spacesboard = new bool[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Spacesboard[i, j] = isBlackPiece;
                if (!isBlackPiece)
                {
                    SpawnBoardSpace(1, i, j);
                }
                else
                {
                    SpawnBoardSpace(0, i, j);
                }
                isBlackPiece = !isBlackPiece;
            }
            isBlackPiece = !isBlackPiece;
        }
    }

    private void EndGame()
    {
        if (WhoseTurn == Turn.ICE)
        {
            //isIceTurn = true;
            Debug.Log("Ice Wins!");
        }
        else
        {
            //isIceTurn = false;
            Debug.Log("Fire Wins!");
        }

        foreach (GameObject go in activeBreakman)
        {
            Destroy(go);
        }
        //isIceTurn = true; //This is provoking an error Needs to have another winning conditions separetely
        BoardHighlights.Instance.HideHighlights();
        //SpawnAllBreakPieces();
    }

    public void playAnimation(PieceMultiplayer selectedPiece, char moveDirection, int x, int y, bool capture)
    {
        if (capture == false)
        {
            //If Movement is Left, plays left animation
            if (moveDirection == 'l')
            {
                selectedPiece.GetComponent<Animation>().Play("Left");
            }

            //If Movement is Right, plays right animation
            if (moveDirection == 'r')
            {
                selectedPiece.GetComponent<Animation>().Play("Right");
            }

            //If movement is Forward, plays forward animation
            if (moveDirection == 'm')
            {
                selectedPiece.GetComponent<Animation>().Play("Forward");
            }
        }
        else
        {
            if (moveDirection == 'l')
            {
                selectedPiece.GetComponent<Animation>().Play("Capture - Left");
            }

            //If Movement is Right, plays right animation
            if (moveDirection == 'r')
            {
                selectedPiece.GetComponent<Animation>().Play("Capture - Right");
            }
        }
    }
}