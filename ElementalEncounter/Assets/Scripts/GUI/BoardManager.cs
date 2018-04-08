﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
class UndoEntry
{
    public GameCore.Turn turn;
    public Move move;
    public bool capture;
}
public class BoardManager : MonoBehaviour
{
    #region Common Properties and Fields
    #region public variables
    //List of Gameobject for spawning pieces on the board
    public List<GameObject> piecePrefabs;
    //public List<GameObject> boardPrefabs;
    public GameObject Timer;
    public GameObject panelContainer;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject CurrentTurnText;
    public GameObject IceTerrain;
    public GameObject FireTerrain;
    public GameObject ClashTerrain;
    public GameObject UndoButton;
    public GameObject DisconnectPanel;
    public static BoardManager Instance { set; get; }
    public Board<Piece> Pieces { set; get; }
    public bool isMyTurn = true;
    public bool testing = false;
    public GameCore gameCore;
    #endregion
    #region Private Variables
    private List<GameObject> activePieces = new List<GameObject>();
    private Text networkTimer;
    private Piece selectedPiece;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private AI.AI HinterXHinter;
    private Camera MainCamera;
    private bool isClicked = false;
    private float timerCount = 60f;
    private GameCore.Turn LastTurn;
    private NetworkGame.NetworkManager networkLogic;
    #endregion 
    #endregion

    private void Start()
    {
        string time = DateTime.Now.ToString("h:mm:ss tt");
        Debug.Log("Made it to the start function at    " + time);

        MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        IceTerrain.SetActive(false);
        FireTerrain.SetActive(false);
        ClashTerrain.SetActive(false);
        switch (gameCore.Map)
        {
            case GameCore.MapChoice.ICE:
                IceTerrain.SetActive(true);
                break;
            case GameCore.MapChoice.FIRE:
                FireTerrain.SetActive(true);
                break;
            case GameCore.MapChoice.CLASH:
                ClashTerrain.SetActive(true);
                break;
        }

        if (!gameCore.isSinglePlayer)
        {   //Multiplayer
            networkLogic = GameObject.Find("NetworkManager").GetComponent<NetworkGame.NetworkManager>();
            networkTimer = Timer.GetComponent<Text>();
            UndoButton.SetActive(false);
            if (!gameCore.isMasterClient)
            {
                WhatIsMySide();
            }
        }
        else
        {
            ChangeSide();
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

        GameObject core = GameObject.Find("GameCore");
        if (core == null)
        {
            gameCore = new GameObject("GameCore").AddComponent<GameCore>();
            gameCore.isSinglePlayer = true;
            gameCore.aILevel = GameCore.AILevel.Intermediate;
            gameCore.MySide = GameCore.Turn.ICE;
        }
        else gameCore = core.GetComponent<GameCore>();
    }
    private void ResetAndInitializeBoard()
    {
        UndoQueue = new Queue<UndoEntry>();
        BreakAnimations = new Stack<int>();

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

    private Queue<UndoEntry> UndoQueue;
    private Stack<int> BreakAnimations;
    private bool UndoInProgress = false;
    public void UndoButtonClick(){
        if(gameCore.CurrentTurn == gameCore.MySide) gameCore.Undo();
    }

    internal void QueueUndo(Move m, GameCore.Turn t, bool cap) {
        UndoQueue.Enqueue(new UndoEntry {
            move = m,
            turn = t,
            capture = cap
        });
    }
    internal IEnumerator Undo() {
        UndoInProgress = true;
        UndoEntry entry = UndoQueue.Dequeue();

		//StartCoroutine(Pieces[move.From].PlayMoveSound()); //sound effect
		Piece.playAnimationReverse(Pieces[entry.move.To], entry.move);
		
		Pieces[entry.move.To].transform.position = GetTileCenter(entry.move.From); // Getting the center of the tile where the piece is moving
		Pieces[entry.move.To].SetPosition(entry.move.From);
		Pieces[entry.move.From] = Pieces[entry.move.To];
		Pieces[entry.move.To] = null;
        //CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Fire Turn" : "Ice Turn";
        //LastTurn = gameCore.CurrentTurn;

        if (entry.capture)
        {
            //	activePieces.Remove(Pieces[move.To].gameObject);
            //	StartCoroutine(PlayCaptureSound(Pieces[move.To])); //sound effect
            //	Destroy(Pieces[move.To].gameObject, 1.4f);
            SpawnPiece(entry.turn == GameCore.Turn.ICE ? 1 : 0, entry.move.To);
        }

        yield return new WaitForSeconds(1.5f);
        UndoInProgress = false;
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
        timerCount = 60f;
        Timer.SetActive(false);
        selectedPiece = null;
    }

    public void ChangeSide()
    {
        gameCore.boardManager = this;
        //Debug.Log("Started RestAndInitilizeBoard funcion");
        ResetAndInitializeBoard();
        //Debug.Log("Finishied RestAndInitilizeBoard funcion");

        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            isMyTurn = true;
            MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, -4);
            MainCamera.transform.rotation = Quaternion.Euler(MainCamera.transform.rotation.eulerAngles.x, 0f, 0f);
            if(!gameCore.isSinglePlayer) Timer.SetActive(true);
        }
        else
        {
            isMyTurn = false;
            MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, 12);
            MainCamera.transform.rotation = Quaternion.Euler(MainCamera.transform.rotation.eulerAngles.x, 180f, 0f);
            if (!gameCore.isSinglePlayer) Timer.SetActive(true);
        }
        LastTurn = GameCore.Turn.FIRE;
        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            LastTurn = GameCore.Turn.ICE;
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Waiting for opponent" : "Ice Turn";
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.black : Color.blue;
            LastTurn = GameCore.Turn.FIRE;
        }
        else
        {
            LastTurn = GameCore.Turn.ICE;
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Fire Turn" : "Waiting for opponent";
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.red : Color.black;
            LastTurn = GameCore.Turn.FIRE;
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
        if (LastTurn != gameCore.MySide) StartCoroutine(ShowsAfterSeconds(2, loseMenu));
        else StartCoroutine(ShowsAfterSeconds(2, winMenu));
        isMyTurn = false;
    }
    //delays the Menu
    IEnumerator ShowsAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        panelContainer.SetActive(true);
        obj.SetActive(true);
    }

    public void EndGameNetwork()
    {
        networkLogic.SendEndGame();
    }
    public void SendTimeOut()
    {
        networkLogic.TimeOut();
    }
    public void ReceiveTimeOut()
    {
        EndGame();
    }
    public void SendSide()
    {
        if(gameCore.MySide == GameCore.Turn.ICE) { networkLogic.WhatSide(0); }
        else { networkLogic.WhatSide(1); }
        ChangeSide();
    }
    public void ReceiveSide(int side)
    {
        if(side == 0) { gameCore.MySide = GameCore.Turn.FIRE; ChangeSide(); }
        else { gameCore.MySide = GameCore.Turn.ICE; ChangeSide(); }
    }
    public void WhatIsMySide()
    {
        networkLogic.RequestSide();
    }
    public void ResetBoard()
    {
        ResetAndInitializeBoard();
    }
    public IEnumerator PlayCaptureSound(Piece p)
    {
        for (int i = 0; i < 60; i++) yield return null;    //This waits for a number of frames (animations go for 100)
        AudioSource.PlayClipAtPoint(p.captureSound, p.transform.position);
    }

    //This function is called by the Game Core to tell the GUI that a valid move has been made, and the screen needs to be updated
    public void UpdateGUI(Move move)
    {
        bool takingPiece = Pieces[move.To] != null;
        if (takingPiece)
        {
            activePieces.Remove(Pieces[move.To].gameObject);
            StartCoroutine(PlayCaptureSound(Pieces[move.To])); //sound effect
            Destroy(Pieces[move.To].gameObject, 1.4f);
        }
        StartCoroutine(Pieces[move.From].PlayMoveSound()); //sound effect
        Piece.playAnimation(Pieces[move.From], move, takingPiece);

        Pieces[move.From].transform.position = GetTileCenter(move.To); // Getting the center of the tile where the piece is moving
        Pieces[move.From].SetPosition(move.To);
        Pieces[move.To] = Pieces[move.From];
        Pieces[move.From] = null; // Removing the piece from the 
        if(gameCore.MySide == GameCore.Turn.ICE)
        {
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Waiting for opponent" : "Ice Turn";
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.black : Color.blue;
        }
        else
        {
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Fire Turn" : "Waiting for opponent";
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.red : Color.black;
        }
        LastTurn = gameCore.CurrentTurn;
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
            SpawnPiece(0, i, testing ? 2 : 0);
            SpawnPiece(0, i, testing ? 3 : 1);
        }
        //spawn Fire team
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(1, i, testing ? 4 : 6);
            SpawnPiece(1, i, testing ? 5 : 7);
        }
    }
    public void SpawnPiece(int prefab, Coordinate c) { SpawnPiece(prefab, c.X, c.Y); }
    public void SpawnPiece(int prefab, int x, int y)
    {
        GameObject go = Instantiate(piecePrefabs[prefab], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);    
        Pieces[x, y] = go.GetComponent<Piece>();
        Pieces[x, y].SetPosition(x, y);
        activePieces.Add(go);
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
        if (UndoQueue.Count > 0 && !UndoInProgress) StartCoroutine(Undo());

        UpdateSelection();

        if (Input.GetMouseButtonDown(0) && cursorLocation != null && isMyTurn && !panelContainer.activeInHierarchy)
        {
            if (selectedPiece == null) SelectPiece(cursorLocation);
            else MakeLocalMove(cursorLocation);
        }
    }

    private void LateUpdate()
    {
        if(Timer.activeInHierarchy != false)
        {
            timerCount -= Time.deltaTime;
            networkTimer.text = Mathf.RoundToInt(timerCount).ToString();
            if (timerCount < 0)
            {
                SendTimeOut();
            }
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
