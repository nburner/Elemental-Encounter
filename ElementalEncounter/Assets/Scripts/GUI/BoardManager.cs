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
    public GameObject netWinMenu;
    public GameObject netLossMenu;
    public GameObject CurrentTurnText;
    public GameObject IceTerrain;
    public GameObject FireTerrain;
    public GameObject ClashTerrain;
    public GameObject UndoButton;
    public GameObject DisconnectPanel;
    public GameObject ChatPanel;
    public GameObject messageText;
    public GameObject mapPanel;
    public GameObject timeOutPanel;
    public GameObject moveLogPanel;
    public InputField inputField;
    public Text timeOutText;
    public Transform chatMessageContainer;
    public Transform moveLogContainer;
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
    private float timerCount = 180f;
    private GameCore.Turn LastTurn;
    private NetworkGame.NetworkManager networkLogic;
    private Vector3 BoardCenter = new Vector3(4f, 0, 4f);
    private List<GameObject> capturedFirePieces = new List<GameObject>();
    private List<GameObject> capturedIcePieces = new List<GameObject>();
    private string playerName;
    private string playerOpponent;
    #endregion 
    #endregion

    private void Start()
    {
        string time = DateTime.Now.ToString("h:mm:ss tt");
        Debug.Log("Made it to the start function at    " + time);

        MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        MusicControler mc = GameObject.Find("MusicController").GetComponent<MusicControler>();
        mc.PlayMapMusic(gameCore.Map);

		TopDownHUDButton = GameObject.Find("HideMiniMap_Button").GetComponent<Button>();
		TopDown = MainCamera.GetComponentInChildren<RawImage>(true).gameObject;

		TopDown.SetActive(true);
		TopDownHUDButton.gameObject.SetActive(true);

		
        string capture = "CapturedFirePiece";
        for (int i = 1; i < 17; i++){
             capturedFirePieces.Add(GameObject.Find(capture + i.ToString()));
            capturedFirePieces[i - 1].SetActive(false);
        }
        

        
        capture = "CapturedIcePiece";
        for (int i = 1; i < 17; i++) {
            capturedIcePieces.Add(GameObject.Find(capture + i.ToString()));
            capturedIcePieces[i - 1].SetActive(false);
        }
        

        Debug.Log("Ice Array Length = " + capturedIcePieces.Count);
        Debug.Log("Fire Array Length = " + capturedFirePieces.Count);

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
        ChatPanel.SetActive(false);
        if (!gameCore.isSinglePlayer)
        {   //Multiplayer
            networkLogic = GameObject.Find("NetworkManager").GetComponent<NetworkGame.NetworkManager>();
            networkTimer = Timer.GetComponent<Text>();
            UndoButton.SetActive(false);
            ChatPanel.SetActive(true);
            playerName = PhotonNetwork.playerName;
            playerOpponent = PhotonNetwork.otherPlayers[0].NickName;
            if (!gameCore.isMasterClient)
            {
                WhatIsMySide();
            }
        }
        else
        {
            playerName = (gameCore.MySide==GameCore.Turn.FIRE)?"Fire":"Ice";
            playerOpponent = "Computer"; 
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
        turnCount = 1;

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
        foreach (Transform child in moveLogContainer)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    #region Option Buttons
    public void MuteSoundEffectsButtonClick()
    {
        gameCore.sound = !gameCore.sound;

        for (int i = 0; i < activePieces.Count; i++) if (activePieces[i] != null) activePieces[i].GetComponent<AudioSource>().mute = !gameCore.sound;
    }
    GameObject TopDown = null;
    Button TopDownHUDButton = null;
	public void HideTopDownButtonClick()
    {
        TopDown.SetActive(!TopDown.activeSelf);
		TopDownHUDButton.gameObject.SetActive(TopDown.activeSelf);

		TopDownHUDButton.GetComponent<Text>().text = TopDown.activeSelf ? "Hide Map" : "Show Map";
    }
    
    public void ResetCameraButtonClick()
    {
        ResetCamera();
    }
    #endregion

    #region Undo Stuff
    private Queue<UndoEntry> UndoQueue;
    private Stack<int> BreakAnimations;
    private bool UndoInProgress = false;
    public void UndoButtonClick()
    {
        BoardHighlights.Instance.HideHighlights();
        if (gameCore.CurrentTurn == gameCore.MySide) gameCore.Undo();
    }


    internal void QueueUndo(Move m, GameCore.Turn t, bool cap)
    {
        UndoQueue.Enqueue(new UndoEntry
        {
            move = m,
            turn = t,
            capture = cap
        });
    }
    internal IEnumerator Undo()
    {
        UndoInProgress = true;
        UndoEntry entry = UndoQueue.Dequeue();

        int numOfChildren = moveLogContainer.transform.childCount;
        if (numOfChildren > 0)
        {
            Destroy(moveLogContainer.transform.GetChild(numOfChildren - 1).gameObject);
        }
        turnCount--;

        if (Pieces[entry.move.To].Undone) Pieces[entry.move.To].transform.position = GetTileCenter(Pieces[entry.move.To].Position); // Getting the center of the tile where the piece is moving
        Pieces[entry.move.To].Undone = true;

        Pieces[entry.move.To].SetPosition(entry.move.From);
        Pieces[entry.move.From] = Pieces[entry.move.To];
        PlayUndoAnimationMove(Pieces[entry.move.From], entry.move, entry.capture);

        CurrentTurnText.GetComponent<Text>().text = "Undo in progress";
        CurrentTurnText.GetComponent<Text>().color = Color.black;

		if (!entry.capture) Pieces[entry.move.To] = null;
        else yield return PlayUndoAnimationBreak(Pieces[entry.move.To], entry.move, entry.capture);
        
        yield return new WaitWhile(() => Pieces[entry.move.From].GetComponent<Animation>().isPlaying);
        
        UndoInProgress = false;

		if (gameCore.CurrentTurn == GameCore.Turn.ICE) {
			capturedIcePieces[gameCore.IceCount - 1].SetActive(false);
		}
		else {
			capturedFirePieces[gameCore.FireCount - 1].SetActive(false);
		}

		if (HinterXHinter == null)
        {
            HinterXHinter = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.HINTER, gameCore.MySide == GameCore.Turn.ICE ? AI.Turn.ICE : AI.Turn.FIRE, UpdateHint);
        }
        HinterXHinter.GetMove(gameCore.Pieces);
        hintReady = false;
        
        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            LastTurn = GameCore.Turn.ICE;
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Waiting for "+playerOpponent : playerName;
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.black : Color.blue;
            LastTurn = GameCore.Turn.FIRE;
        }
        else
        {
            LastTurn = GameCore.Turn.ICE;
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? playerName : "Waiting for "+playerOpponent;
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.red : Color.black;
            LastTurn = GameCore.Turn.FIRE;
        }
    }

    void PlayUndoAnimationMove(Piece selectedPiece, Move m, bool capture)
    {
        if (gameCore.animations)
        {
            string animation;
            if (!capture)
            {
                if (m.Direction == Move.Laterality.LEFT) animation = "Left";
                else if (m.Direction == Move.Laterality.RIGHT) animation = "Right";
                else animation = "Forward";
            }
            else
            {
                string[] animationList = { "", "", "Bounce - Left", "Roll - Left", "Smash - Left", "Bounce - Right", "Roll - Right", "Smash - Right", "Diagonal - Left", "Dig - Left", "Top - Left", "Diagonal - Right", "Dig - Right", "Top - Right" };

                animation = animationList[BreakAnimations.Peek()];
            }

            selectedPiece.GetComponent<Animation>()[animation].speed = -1;
            selectedPiece.GetComponent<Animation>()[animation].time = selectedPiece.GetComponent<Animation>()[animation].length;
            selectedPiece.GetComponent<Animation>().Play(animation);
        }
        else selectedPiece.transform.position = GetTileCenter(selectedPiece.Position);

        selectedPiece.PlayMoveSound(false);
    }
    IEnumerator PlayUndoAnimationBreak(Piece selectedPiece, Move m, bool capture)
    {
		string animation;
		Piece unbreakingPiece;

		if (gameCore.animations) {
			int breakAnimation = BreakAnimations.Pop();

			unbreakingPiece = SpawnPiece(breakAnimation, m.To);
			animation = unbreakingPiece.GetComponent<Animation>().clip.name;
			unbreakingPiece.GetComponent<Animation>()[animation].speed = -1;
			unbreakingPiece.GetComponent<Animation>()[animation].time = unbreakingPiece.GetComponent<Animation>()[animation].length;

			unbreakingPiece.GetComponent<Animation>().Play(animation);
			
			yield return new WaitWhile(() => unbreakingPiece.GetComponent<Animation>().isPlaying);

			Destroy(unbreakingPiece.gameObject);
			activePieces.Remove(unbreakingPiece.gameObject);
		}

        GameObject go = Instantiate(piecePrefabs[Pieces[m.From].isIce ? 1 : 0], GetTileCenter(m.To.X, m.To.Y + (Pieces[m.From].isIce ? -1 : 1)), Quaternion.identity) as GameObject;
        go.GetComponent<AudioSource>().mute = !gameCore.sound;
        go.transform.SetParent(transform);
        Pieces[m.To] = go.GetComponent<Piece>();
        Pieces[m.To].SetPosition(m.To);
        activePieces.Add(go);

        animation = go.GetComponent<Animation>().clip.name;
        go.GetComponent<Animation>()[animation].time = go.GetComponent<Animation>()[animation].length;
        go.GetComponent<Animation>()[animation].speed = 1;
        go.GetComponent<Animation>().Play(animation);

        yield return null;
        go.GetComponent<Animation>().Stop();
        Pieces[m.To].transform.position = GetTileCenter(m.To);		        
    }
    #endregion

    //This function is used by the GUI to validate a potential move by the local user, and send it to the Game Core if it's good
    private void MakeLocalMove(Coordinate to)
    {
        Move myMove;
        try
        {
            myMove = new Move(selectedPiece.Position, to);

            if (gameCore.PossibleMoves(selectedPiece.isIce, selectedPiece.Position).Contains(myMove)) //If Valid Move
            {
                if (!gameCore.isSinglePlayer) networkLogic.SendMove(myMove, gameCore.MySide);
                isMyTurn = false;
                gameCore.UpdateBoard(myMove);
            }
        }
        catch (ArgumentException e)
        {
            //Move constructor throws on invalid move
            Debug.Log(e.Message);
        }
		        
        timerCount = 180f;
        Timer.SetActive(false);
        selectedPiece = null;
    }
    private void ResetCamera()
    {
        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            MainCamera.transform.position = BoardCenter + new Vector3(0, 6, -9f);
            MainCamera.transform.rotation = Quaternion.Euler(MainCamera.transform.rotation.eulerAngles.x, 0f, 0f);
        }
        else
        {
            MainCamera.transform.position = BoardCenter + new Vector3(0, 6, 9f);
            MainCamera.transform.rotation = Quaternion.Euler(MainCamera.transform.rotation.eulerAngles.x, 180f, 0f);
        }
        MainCamera.transform.LookAt(BoardCenter);
    }
    public void ChangeSide()
    {
        gameCore.boardManager = this;
        //Debug.Log("Started RestAndInitilizeBoard funcion");
        ResetAndInitializeBoard();
        //Debug.Log("Finishied RestAndInitilizeBoard funcion");
        ResetCamera();
        
        isMyTurn = gameCore.MySide == GameCore.Turn.ICE;
        if (!gameCore.isSinglePlayer) Timer.SetActive(true);
        
        LastTurn = GameCore.Turn.FIRE;
        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            LastTurn = GameCore.Turn.ICE;
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Waiting for " + playerOpponent : playerName;
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.black : Color.blue;
            LastTurn = GameCore.Turn.FIRE;
        }
        else
        {
            LastTurn = GameCore.Turn.ICE;
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? playerName : "Waiting for " + playerOpponent;
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
        if (!gameCore.isSinglePlayer) networkLogic.OnMatchFinished();

        if (LastTurn != gameCore.MySide)
        {
            if (gameCore.isSinglePlayer) StartCoroutine(ShowsAfterSeconds(2, loseMenu));
            else StartCoroutine(ShowsAfterSeconds(2, netLossMenu));
        }
        else
        {
            if (gameCore.isSinglePlayer) StartCoroutine(ShowsAfterSeconds(2, winMenu));
            else StartCoroutine(ShowsAfterSeconds(2, netWinMenu));
        }

        Timer.SetActive(false);
        isMyTurn = false;
    }
    //delays the Menu
    IEnumerator ShowsAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        panelContainer.SetActive(true);
        mapPanel.SetActive(true);
        obj.SetActive(true);
    }
    #region Networking functions
    public void EndGameNetwork()
    {
        networkLogic.SendEndGame();
    }
    public void SendTimeOut()
    {
        networkLogic.TimeOut();
        timeOutPanel.SetActive(true);
        timeOutText.text = "You have lost due to Time Out!";
        
    }
    public void ReceiveTimeOut()
    {
        timeOutPanel.SetActive(true);
        timeOutText.text = "You have won due to Time Out!";
    }
    public void SendSide()
    {
        if (gameCore.MySide == GameCore.Turn.ICE) { networkLogic.WhatSide(0); }
        else { networkLogic.WhatSide(1); }
        ChangeSide();
    }
    public void ReceiveSide(int side)
    {
        if (side == 0) { gameCore.MySide = GameCore.Turn.FIRE; ChangeSide(); }
        else { gameCore.MySide = GameCore.Turn.ICE; ChangeSide(); }
    }
    public void WhatIsMySide()
    {
        networkLogic.RequestSide();
    }
    public void SendMessageChat()
    {
        InputField inputFieldBox = GameObject.Find("MessageInput").GetComponent<InputField>();
        if (inputFieldBox.text == "") return;
        networkLogic.SendMessageChat(playerName+":  " + inputFieldBox.text);
        GameObject textInstance = Instantiate(messageText) as GameObject;
        textInstance.transform.SetParent(chatMessageContainer);

        textInstance.GetComponentInChildren<Text>().text = playerName + ":  " + inputFieldBox.text;
        inputFieldBox.text = "";
    }
    public void ReceiveMessage(string message)
    {
        GameObject textInstance = Instantiate(messageText) as GameObject;
        textInstance.transform.SetParent(chatMessageContainer);

        textInstance.GetComponentInChildren<Text>().text = message;
    }

	static int turnCount = 1;
    public void MoveLogtoGUI(Move move)
    {
        int FromX, FromY, ToY, ToX;
        FromX = move.From.X;
        FromY = move.From.Y;
        ToX = move.To.X;
        ToY = move.To.Y;
		string moveText = "Turn " + (turnCount++) + ": " + ((gameCore.CurrentTurn == GameCore.Turn.FIRE) ? "Fire" : "Ice") + " - " + (char)(FromX + 'A') + (FromY + 1) + " to " + (char)(ToX + 'A') + (ToY + 1) + "";
        GameObject textInstance = Instantiate(messageText) as GameObject;
        textInstance.transform.SetParent(moveLogContainer);

        textInstance.GetComponentInChildren<Text>().text = moveText;
    }
    #endregion
    public void ResetBoard()
    {
        ResetAndInitializeBoard();
    }
    public void PlayCaptureSound(Piece piece, int captureChoice)
    {
		float[] delays = { -1, -1, 60, 50, 75, 60, 50, 75, 70, 60, 80, 70, 60, 80 };        
        if(captureChoice > 1 && captureChoice < delays.Length) piece.GetComponent<AudioSource>().PlayDelayed(delays[captureChoice]/60f);
    }

    //This function is called by the Game Core to tell the GUI that a valid move has been made, and the screen needs to be updated
    public void UpdateGUI(Move move)
    {
        if (Pieces[move.From].Undone) Pieces[move.From].transform.position = GetTileCenter(Pieces[move.From].Position); // Getting the center of the tile where the piece is moving
        Pieces[move.From].Undone = false;

		BoardHighlights.Instance.HideHighlights();

        bool takingPiece = Pieces[move.To] != null;
        if (takingPiece)
        {
            activePieces.Remove(Pieces[move.To].gameObject);
            Destroy(Pieces[move.To].gameObject);

            if (gameCore.CurrentTurn == GameCore.Turn.ICE) {
                capturedFirePieces[gameCore.FireCount].SetActive(true);
            }
            else {
                capturedIcePieces[gameCore.IceCount].SetActive(true);
            }

        }
        StartCoroutine(Pieces[move.From].PlayMoveSound()); //sound effect
        int breakAnimation = Piece.playAnimation(Pieces[move.From], move, takingPiece, gameCore.animations);
		if (breakAnimation > 0) {
			BreakAnimations.Push(breakAnimation);
            PlayCaptureSound(Pieces[move.To], breakAnimation); //sound effect
		}

        Pieces[move.From].transform.position = GetTileCenter(move.To); // Getting the center of the tile where the piece is moving
        Pieces[move.From].SetPosition(move.To);
        Pieces[move.To] = Pieces[move.From];
        Pieces[move.From] = null; // Removing the piece from the 
        MoveLogtoGUI(move);
        if (gameCore.MySide == GameCore.Turn.ICE)
        {
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? "Waiting for " + playerOpponent : playerName;
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.black : Color.blue;
        }
        else
        {
            CurrentTurnText.GetComponent<Text>().text = (LastTurn == GameCore.Turn.FIRE) ? playerName : "Waiting for " + playerOpponent;
            CurrentTurnText.GetComponent<Text>().color = (LastTurn == GameCore.Turn.FIRE) ? Color.red : Color.black;
        }
        LastTurn = gameCore.CurrentTurn;

		BoardHighlights.Instance.HighlightPreviousMove(move);
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

    public Piece SpawnPiece(int prefab, Coordinate c, bool animation = true) { return SpawnPiece(prefab, c.X, c.Y, animation); }
    public Piece SpawnPiece(int prefab, int x, int y, bool animation = true)
    {
        GameObject go = Instantiate(piecePrefabs[prefab], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Pieces[x, y] = go.GetComponent<Piece>();
        Pieces[x, y].SetPosition(x, y);
        activePieces.Add(go);
        if (animation) go.GetComponent<Animation>().Play();
        go.GetComponent<AudioSource>().mute = !gameCore.sound;
        return Pieces[x, y];
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
        
        if (Input.GetMouseButtonDown(0) && cursorLocation != null && isMyTurn && !panelContainer.activeInHierarchy && !UndoInProgress)
        {
            if (selectedPiece == null) SelectPiece(cursorLocation);
            else MakeLocalMove(cursorLocation);
        }


        var d = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.UpArrow) && MainCamera.transform.rotation.eulerAngles.x < 85 && !inputField.isFocused) {
            var axis = Vector3.Cross(Vector3.up, MainCamera.transform.position - BoardCenter);            
            MainCamera.transform.RotateAround(BoardCenter, axis.normalized, -.5f);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && MainCamera.transform.rotation.eulerAngles.x > 10 && !inputField.isFocused) {
            var axis = Vector3.Cross(Vector3.up, MainCamera.transform.position - BoardCenter);
            MainCamera.transform.RotateAround(BoardCenter, axis.normalized, .5f);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && !inputField.isFocused) MainCamera.transform.RotateAround(BoardCenter, Vector3.up, .5f);

        else if (Input.GetKey(KeyCode.RightArrow) && !inputField.isFocused) MainCamera.transform.RotateAround(BoardCenter, Vector3.down, .5f);

        if ((((MainCamera.transform.position - BoardCenter).magnitude > 7 && d > 0) || (d < 0 && (MainCamera.transform.position - BoardCenter).magnitude < 12)) && 
            !moveLogContainer.GetComponentInParent<ScrollableBox>().isOver && 
            (gameCore.isSinglePlayer || !chatMessageContainer.GetComponentInParent<ScrollableBox>().isOver))
            MainCamera.transform.position = Vector3.MoveTowards(MainCamera.transform.position, BoardCenter, d * 2.5f);

        //while (MainCamera.transform.position.y < 2) MainCamera.transform.Translate(0, 1, 0);
        //while (MainCamera.transform.position.y > 16) MainCamera.transform.Translate(0, 1, 0);
    }

    private void LateUpdate()
    {
        if (Timer.activeInHierarchy != false)
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
        //Highlight the clicked space
        BoardHighlights.Instance.HighlightClickedSpace(loc);
        //Check if piece has any possible 
        List<Move> moves = gameCore.PossibleMoves(Pieces[loc].isIce, loc);
        if (moves.Count == 0) return;

        selectedPiece = Pieces[loc];
        BoardHighlights.Instance.HighlightAllowedMoves(moves);
        BoardHighlights.Instance.HighlightClickedSpace(loc);
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
    bool hintReady = false, hintShown = false; Move hintMove;
    //Called by the hint ai once the hint has been calculated
    private void UpdateHint(Move move)
    {
        hintMove = move;
        hintReady = true;
    }
    //Called by the button on screen
    public void GetHint()
    {
		BoardHighlights.Instance.HideHighlights();
		if (hintShown) {
			hintShown = false;
			return;
		}
        if (isMyTurn) StartCoroutine(showHint());
    }
    //Waits until the hint is ready, then shows it on the board
    private IEnumerator showHint()
    {
        while (!hintReady) yield return null;

        BoardHighlights.Instance.HighlightHint(hintMove);
        BoardHighlights.Instance.HighlightClickedSpace(hintMove);
		hintShown = true;
        yield return null;
    }
    #endregion
}
