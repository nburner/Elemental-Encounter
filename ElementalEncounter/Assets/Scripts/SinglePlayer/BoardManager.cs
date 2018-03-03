using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //List of Gameobject for spawning pieces on the board
    public List<GameObject> piecePrefabs;
    public List<GameObject> boardPrefabs;
    public List<GameObject> activePieces;

    public bool isMyTurn = true;

    public static BoardManager Instance { set; get; }
    public Piece[,] Pieces { set; get; }

    private Piece selectedPiece;
    private bool isClicked = false;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private int selectionX = -1;
    private int selectionY = -1;
    private GameCore gameCore;
    private AI.AI HinterXHinter;
    private Camera IceCamera, FireCamera;
    private NetworkGame.NetworkManager networkLogic;

    private void Start()
    {
        IceCamera = GameObject.Find("IceCamera").GetComponent<Camera>();
        FireCamera = GameObject.Find("FireCamera").GetComponent<Camera>();

        GameObject core = GameObject.Find("GameCore");
        if (core == null)
        {
            gameCore = new GameObject("Temp Game Core").AddComponent<GameCore>();
            gameCore.isSinglePlayer = true;
            gameCore.aILevel = GameCore.AILevel.Intermediate;
            gameCore.MySide = GameCore.Turn.ICE;
        }
        else gameCore = core.GetComponent<GameCore>();

        if (gameCore.isSinglePlayer == false)
        {   //Multiplayer
            networkLogic = GameObject.Find("NetworkManager").GetComponent<NetworkGame.NetworkManager>();
            gameCore = networkLogic.GetGameCore();
        }

        gameCore.boardManager = this;
        SpawnAllBoardSpaces();
        InitializeBoard();

        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            IceCamera.gameObject.SetActive(true);
            FireCamera.gameObject.SetActive(false);
        }
        else
        {
            IceCamera.gameObject.SetActive(false);
            FireCamera.gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #region Spawners
    private void SpawnAllPieces()
    {
        activePieces = new List<GameObject>();
        Pieces = new Piece[8, 8];
        //spawn Ice team
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(0, i, 0);
            SpawnPiece(0, i, 1);
        }
        //spawn Fire team
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(1, i, 6);
            SpawnPiece(1, i, 7);
        }
    }
    public void SpawnPiece(int index, int x, int y)
    {
        GameObject go = Instantiate(piecePrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Pieces[x, y] = go.GetComponent<Piece>();
        Pieces[x, y].SetPosition(x, y);
        activePieces.Add(go);
    }

    private void SpawnAllBoardSpaces()
    {
        bool isBlackPiece = false;
        bool[,] Spacesboard = new bool[8, 8];
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
    private void SpawnBoardSpace(int index, int x, int y)
    {
        GameObject another = Instantiate(boardPrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        another.transform.SetParent(transform);
    }

    #endregion

    private void UpdateHint(int fromX, int fromY, int toX, int toY)
    {
        hintReady = true;
        hintToX = toX;
        hintToY = toY;
        HintFromX = fromX;
        hintFromY = fromY;
    }
    bool hintReady = false; int hintToX; int hintToY; int HintFromX; int hintFromY;
    private IEnumerator showHint()
    {
        while (!hintReady) yield return null;
        
        char[,] move = new char[8, 8];
        move[hintToX, hintToY] = GetMoveDirection(HintFromX, hintToX);
        move[HintFromX, hintFromY] = GetMoveDirection(hintToX, HintFromX);
        BoardHighlights.Instance.HighlightAllowedMoves(move);

        yield return null;
    }

    public void GetHint()
    {
        StartCoroutine(showHint());
    }

    internal void GetLocalMove()
    {
        if (HinterXHinter == null)
        {
            HinterXHinter = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.HINTER, gameCore.MySide == GameCore.Turn.ICE ? AI.Turn.ICE : AI.Turn.FIRE, UpdateHint);
        }
        HinterXHinter.GetMove(gameCore.Pieces);
        hintReady = false;
        isMyTurn = true;
    }
    private void Update()
    {
        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedPiece == null)
                {
                    // select the Piece
                    SelectPiece(selectionX, selectionY);
                    Debug.Log("Selected");
                }
                else
                {
                    // Move the Piece
                    //MovePiece(selectionX, selectionY);
                    MakeLocalMove(selectionX, selectionY);
                }
            }
        }
    }

    private void SelectPiece(int x, int y)
    {

        if (Pieces[x, y] == null)
            return;

        if ((Pieces[x, y].isIce && gameCore.MySide == GameCore.Turn.FIRE) || (!Pieces[x, y].isIce && gameCore.MySide == GameCore.Turn.ICE))
            return;
        bool hasAtleastOneMove = false;
        var moves = gameCore.PossibleMove(Pieces[x, y].isIce, x, y);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j] != default(char))
                {
                    hasAtleastOneMove = true;
                    break;
                }
            }
            if (hasAtleastOneMove) break;
        }

        if (!hasAtleastOneMove) return;

        selectedPiece = Pieces[x, y];
        BoardHighlights.Instance.HighlightAllowedMoves(moves);

        if (!isClicked)
        {
            isClicked = !isClicked;
        }

    }

    //private void MovePiece(int x, int y)
    //{
    //    // TODO: Add winning conditions. Make in different functions.
    //    char temp = gameCore.PossibleMove(selectedPiece.isIce, selectedPiece.CurrentX, selectedPiece.CurrentY)[x, y];
    //    if (temp != default(char))
    //    {
    //        Piece b = Pieces[x, y];
    //        if (b != null && b.isIce != isMyTurn)
    //        {
    //            // Capture a piece
    //            activePieces.Remove(b.gameObject);
    //            Destroy(b.gameObject);
    //            Piece.playAnimation(selectedPiece, temp, x, y, true);
    //        }
    //        else
    //        {
    //            Piece.playAnimation(selectedPiece, temp, x, y, false);
    //        }

    //        if (isMyTurn)
    //        {
    //            if (selectedPiece.CurrentY + 1 == 7)
    //            {
    //                EndGame();
    //                return;
    //            }
    //        }
    //        else
    //        {
    //            if (selectedPiece.CurrentY - 1 == 0)
    //            {
    //                EndGame();
    //                return;
    //            }
    //        }

    //        Pieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;

    //        selectedPiece.transform.position = GetTileCenter(x, y);
    //        selectedPiece.SetPosition(x, y);
    //        Pieces[x, y] = selectedPiece;

    //        isMyTurn = !isMyTurn;
    //        //ai.GetMove(Pieces, FinishAIMove);
    //    }

    //    if (isClicked)
    //    {
    //        isClicked = !isClicked;
    //    }
    //    BoardHighlights.Instance.HideHighlights();

    //    selectedPiece = null;

    //}

    private void MakeLocalMove(int x, int y)
    {
        char moveDirection = gameCore.PossibleMove(selectedPiece.isIce, selectedPiece.CurrentX, selectedPiece.CurrentY)[x, y];
        if (moveDirection != default(char) && isMyTurn) //If Valid Move
        {
            isMyTurn = false;
            gameCore.UpdateBoard(selectedPiece.CurrentX, selectedPiece.CurrentY, x, y);
        }

        if (isClicked)
        {
            isClicked = !isClicked;
        }
        BoardHighlights.Instance.HideHighlights();

        selectedPiece = null;
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



    public Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    public void EndGame()
    {
        if (isMyTurn)
        {
            Debug.Log("Ice Wins!");
        }
        else
        {
            Debug.Log("Fire Wins!");
        }

        ResetBoard();
    }

    private void InitializeBoard()
    {
        SpawnAllPieces();
        gameCore.Play();
        HinterXHinter = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.HINTER, gameCore.MySide == GameCore.Turn.ICE ? AI.Turn.ICE : AI.Turn.FIRE, UpdateHint);
    }
    private void ResetBoard()
    {
        foreach (GameObject go in activePieces)
        {
            Destroy(go);
        }
        BoardHighlights.Instance.HideHighlights();

        SpawnAllPieces();
        if (HinterXHinter != null) Destroy(HinterXHinter);
        gameCore.Play();
    }
    public void UpdateGUI(int fromX, int fromY, int toX, int toY)
    {
        bool takingPiece = Pieces[toX, toY] != null;
        if (takingPiece)
        {
            activePieces.Remove(Pieces[toX, toY].gameObject);
            Destroy(Pieces[toX, toY].gameObject, .5f);
        }
        Piece.playAnimation(Pieces[fromX, fromY], GetMoveDirection(fromX, toX), toX, toY, takingPiece);

        Pieces[fromX, fromY].transform.position = GetTileCenter(toX, toY);
        Pieces[fromX, fromY].SetPosition(toX, toY);
        Pieces[toX, toY] = Pieces[fromX, fromY];
        Pieces[fromX, fromY] = null;

        if (toY == 0 || toY == 7) { EndGame(); return; } //If not returned then Fire will go first

        //isMyTurn = !isMyTurn;
    }

    private char GetMoveDirection(int fromX, int toX)
    {
        return fromX == toX ? 'm' :
            fromX - 1 == toX ? 'l' : 'r';
    }
}
