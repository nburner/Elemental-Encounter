using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region Common Properties and Fields
    //List of Gameobject for spawning pieces on the board
    public List<GameObject> piecePrefabs;
    public List<GameObject> boardPrefabs;
    public List<GameObject> activePieces;


    public static BoardManager Instance { set; get; }
    public Board<Piece> Pieces { set; get; }
    public bool isMyTurn = true;

    private Piece selectedPiece;
    private bool isClicked = false;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    public GameCore gameCore;
    private AI.AI HinterXHinter;
    private Camera IceCamera, FireCamera;
    private NetworkGame.NetworkManager networkLogic;

    public GameObject winMenu;
    public GameObject loseMenu;
    #endregion

    private void Start()
    {
        string time = DateTime.Now.ToString("h:mm:ss tt");

        Debug.Log("Made it to the start function at    " + time);

        IceCamera = GameObject.Find("IceCamera").GetComponent<Camera>();
        //FireCamera = GameObject.Find("FireCamera").GetComponent<Camera>();

        GameObject core = GameObject.Find("GameCore");
        GameObject network = GameObject.Find("NetworkManager");
        if (core == null)
        {
            gameCore = new GameObject("Temp Game Core").AddComponent<GameCore>();
            gameCore.isSinglePlayer = true;
            gameCore.aILevel = GameCore.AILevel.Intermediate;
            gameCore.MySide = GameCore.Turn.ICE;
        }
        else gameCore = core.GetComponent<GameCore>();

        if (network != null)
        {   //Multiplayer
            networkLogic = GameObject.Find("NetworkManager").GetComponent<NetworkGame.NetworkManager>();
            gameCore = networkLogic.GetGameCore();
        }

        gameCore.boardManager = this;
        //SpawnAllBoardSpaces();
        //Debug.Log("Started RestAndInitilizeBoard funcion");
        ResetAndInitializeBoard();
        //Debug.Log("Finishied RestAndInitilizeBoard funcion");

        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            //IceCamera.gameObject.SetActive(true);
            ////FireCamera.gameObject.SetActive(false);
        }
        else
        {
            //IceCamera.gameObject.SetActive(false);
            //FireCamera.gameObject.SetActive(true);
        }

        time = DateTime.Now.ToString("h:mm:ss tt");

        Debug.Log("Finished loading GameScene at   " + time);

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void ResetAndInitializeBoard()
    {
        
        foreach (GameObject go in activePieces)
        {
            Destroy(go);
        }
        BoardHighlights.Instance.HideHighlights();

        Debug.Log("Started spawning Pieces");
        SpawnAllPieces();
        Debug.Log("Finished spawning Pieces");

        if (HinterXHinter != null) Destroy(HinterXHinter);

        gameCore.Play();

        HinterXHinter = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.HINTER, gameCore.MySide == GameCore.Turn.ICE ? AI.Turn.ICE : AI.Turn.FIRE, UpdateHint);
    }
    //This function is used by the GUI to validate a potential move by the local user, and send it to the Game Core if it's good
    private void MakeLocalMove(Coordinate to)
    {
        Move myMove;
        try {
            myMove = new Move(selectedPiece.Position, to);

            if (gameCore.PossibleMoves(selectedPiece.isIce, selectedPiece.Position).Contains(myMove)) //If Valid Move
            {
                if (!gameCore.isSinglePlayer) networkLogic.SendMove(myMove,gameCore.MySide);
                isMyTurn = false;
                gameCore.UpdateBoard(myMove);
            }
        }
        catch (ArgumentException e) {
            //Move constructor throws on invalid move
            Debug.Log(e.Message);
        }
                        
        isClicked = false;
        
        BoardHighlights.Instance.HideHighlights();

        selectedPiece = null;
    }

    public void MakeNetworkMove(Coordinate From, Coordinate To)
    {
        Move myMove;
        try
        {
            myMove = new Move(From, To);
            gameCore.UpdateBoard(myMove);
        }
        catch (ArgumentException e)
        {
            //Move constructor throws on invalid move
            Debug.Log(e.Message);
        }
    }


    #region Called By The Game Core
    //This function is used by the game core to tell the GUI that it is the local user's turn
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

    //This function is called by the Game Core to tell the GUI that the game is over
    public void EndGame()
    {
        //This Boolean is almost certainly wrong
        if (isMyTurn != (gameCore.MySide == GameCore.Turn.ICE))
        {
            loseMenu.SetActive(true);
        }
        else
        {
            winMenu.SetActive(true);
        }
    }
    public void EndGameNetwork()
    {
        networkLogic.SendEndGame();
    }

    public void ResetBoard()
    {
        ResetAndInitializeBoard();
    }

    //This function is called by the Game Core to tell the GUI that a valid move has been made, and the screen needs to be updated
    public void UpdateGUI(Move move)
    {
        bool takingPiece = Pieces[move.To] != null;
        if (takingPiece)
        {
            activePieces.Remove(Pieces[move.To].gameObject);
            Destroy(Pieces[move.To].gameObject, .5f);
        }
        Piece.playAnimation(Pieces[move.From], move, takingPiece);

        Pieces[move.From].transform.position = GetTileCenter(move.To);
        Pieces[move.From].SetPosition(move.To);
        Pieces[move.To] = Pieces[move.From];
        Pieces[move.From] = null;
    }
    #endregion

    #region Spawners
    private void SpawnAllPieces()
    {
        activePieces = new List<GameObject>();
        Pieces = new Board<Piece>();
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
    public void SpawnPiece(int index, Coordinate c) { SpawnPiece(index, c.X, c.Y); }
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

    #region Get Board Click
    private Coordinate cursorLocation = null;
    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("BreakPlane")))
        {
            cursorLocation = new Coordinate((int)hit.point.x, (int)hit.point.z);
        }
        else cursorLocation = null;
    }
    private void Update()
    {
        UpdateSelection();

        if (Input.GetMouseButtonDown(0) && cursorLocation != null && isMyTurn)
        {
            if (selectedPiece == null) SelectPiece(cursorLocation);
            else MakeLocalMove(cursorLocation);
        }
    }
    private void SelectPiece(Coordinate loc)
    {
        //Check if piece exists
        if (Pieces[loc] == null) return;
        //Check if piece "isIce" is the same as my "isIce" (aka, make sure it's my piece)
        if (Pieces[loc].isIce != (gameCore.MySide == GameCore.Turn.ICE)) return;
        //Check if piece has any possible 
        List<Move> moves = gameCore.PossibleMoves(Pieces[loc].isIce, loc);
        if (moves.Count == 0) return;

        selectedPiece = Pieces[loc];
        BoardHighlights.Instance.HighlightAllowedMoves(moves);
                
        isClicked = true;
    }
    public Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
    public Vector3 GetTileCenter(Coordinate c)
    {
        return GetTileCenter(c.X, c.Y);
    }
    #endregion

    #region Hint Stuff
    bool hintReady = false; Move hintMove;
    //Called by the hint ai once the hint has been calculated
    private void UpdateHint(Move move)
    {
        hintMove = move;
        hintReady = true;
    }
    //Called by the button on screen
    public void GetHint()
    {
        StartCoroutine(showHint());
    }
    //Waits until the hint is ready, then shows it on the board
    private IEnumerator showHint()
    {
        while (!hintReady) yield return null;
        
        BoardHighlights.Instance.HighlightHint(hintMove);
        yield return null;
    }
    #endregion
}
