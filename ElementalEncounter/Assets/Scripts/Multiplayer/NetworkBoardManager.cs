using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBoardManager : MonoBehaviour
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

    public NetworkGame.GameManager manager;

    public bool isIceTurn = true;
    public bool hasMoved = true;
    public Turn isYourTurn;
    private void Start()
    {
        Instance = this;
        manager = gameObject.GetComponent("GameManager") as NetworkGame.GameManager;

        if (manager != null)
        {
            if (PhotonNetwork.isMasterClient)
            {
                hasMoved = false;
                isIceTurn = true;
                isYourTurn = Turn.ICE;
            }
            else
            {
                isIceTurn = false;
                hasMoved = true;
            }
        }
            
        SpawnAllBoardSpaces();
        SpawnAllBreakPieces();
    }
    private void Update()
    {
        UpdateSelection();

        if (!hasMoved)
        {
            Selection();
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
                    SelectBreakMan(selectionX, selectionY);
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

        if (BreakmanNet[x, y].isIce != isIceTurn)
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

    private void MoveBreakman(int x, int y)
    {
        // TODO: Add winning conditions. Make in different functions.
        if (AllowedMoves[x, y] != 0)
        {
            PieceMultiplayer b = BreakmanNet[x, y];
            char temp = AllowedMoves[x, y];
            if (b != null && b.isIce != isIceTurn)
            {
                // Capture a piece
                activeBreakman.Remove(b.gameObject);
                Destroy(b.gameObject);
                playAnimation(selectedBreakman, temp, x, y, false);
            }

            if (isIceTurn)
            {
                if (selectedBreakman.CurrentY + 1 == 7)
                {
                    EndGame();
                    return;
                }
            }
            else
            {
                if (selectedBreakman.CurrentY - 1 == 0)
                {
                    EndGame();
                    return;
                }
            }

            manager.SendMove(x, y, selectedBreakman.CurrentX, selectedBreakman.CurrentY);
            BreakmanNet[selectedBreakman.CurrentX, selectedBreakman.CurrentY] = null;
            playAnimation(selectedBreakman, temp, x, y, false);

            selectedBreakman.transform.position = GetTileCenter(x, y);
            selectedBreakman.SetPosition(x, y);
            BreakmanNet[x, y] = selectedBreakman;

            isIceTurn = !isIceTurn;
            hasMoved = !hasMoved;
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
        if (BreakmanNet[toX, toY] != null)
        {
            // Capture a piece
            activeBreakman.Remove(BreakmanNet[toX, toY].gameObject);
            Destroy(BreakmanNet[toX, toY].gameObject, .5f);

        }
        else
        {
        }

        //Piece selectedBreakman = Breakmans[fromX, fromY];
        if ((isIceTurn && BreakmanNet[fromX, fromY].CurrentY + 1 == 7) || (!isIceTurn && BreakmanNet[fromX, fromY].CurrentY - 1 == 0))
        {
            EndGame();
            return;
        }


        BreakmanNet[fromX, fromY].transform.position = GetTileCenter(toX, toY);
        BreakmanNet[fromX, fromY].SetPosition(toX, toY);

        BreakmanNet[toX, toY] = BreakmanNet[fromX, fromY];
        BreakmanNet[fromX, fromY] = null;

        isIceTurn = !isIceTurn;
        hasMoved = !hasMoved;
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
        if (isIceTurn)
        {
            Debug.Log("Ice Wins!");
        }
        else
        {
            Debug.Log("Fire Wins!");
        }

        foreach (GameObject go in activeBreakman)
        {
            Destroy(go);
        }

        isIceTurn = true; //This is provoking an error Needs to have another winning conditions separetely
        BoardHighlights.Instance.HideHighlights();
        SpawnAllBreakPieces();
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