using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bitboard = System.UInt64;

class LogEntry
{
	public GameCore.Turn turn;
	public Move move;
	public bool capture;
}
public class GameCore : MonoBehaviour
{
    public enum Turn { ICE, FIRE };
    public enum AILevel { Intermediate, Beginner, Novice, Expert };
    public enum MapChoice { ICE, FIRE, CLASH };

    public bool isSinglePlayer;
    public bool animations=true;
    public bool music = true;
    public bool sound = true;
    public Turn MySide;
    public Turn CurrentTurn;
    public MapChoice Map;
    public bool isMasterClient;
    private AI.AI ai;
    public Board<char> Pieces;
    private int IcePieceCount;
    private int FirePieceCount;
	private Stack<LogEntry> Log;
    public BoardManager boardManager { get; set; }
    public AILevel aILevel;
    // private GameCharacter character;
    public float MainMenuAudioStartTime = 0;

    public int IceCount
    {
        get
        {
            return IcePieceCount;
        }
    }

    public int FireCount
    {
        get
        {
            return FirePieceCount;
        }
    }

    public bool GameOver
    {
        get
        {
            for (int x = 0; x < 8; x++) if (Pieces[x, 7] == 'W' || Pieces[x, 0] == 'B') return true;
            
            return IcePieceCount == 0 || FirePieceCount == 0;
        }
    }


    void Start()
    {
        //InitializeGameCore();
    }

    private void InitializeGameCore()
    {
        Pieces = new Board<char>();
        for (int x = 0; x < 8; x++) for (int y = 0; y < 8; y++) Pieces[x, y] = default(char);
        for (int x = 0; x < 8; x++) for (int y = 0; y < 2; y++) Pieces[x, y] = 'W';
        for (int x = 0; x < 8; x++) for (int y = 6; y < 8; y++) Pieces[x, y] = 'B';

        IcePieceCount = 16;
        FirePieceCount = 16;

        CurrentTurn = Turn.ICE;
		Log = new Stack<LogEntry>();
    }

    public void Play()
    {
        InitializeGameCore();

        if (ai != null) Destroy(ai);
        if (isSinglePlayer) {
            if (aILevel == AILevel.Expert) ai = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.HyperSeeker, MySide == Turn.ICE ? AI.Turn.FIRE : AI.Turn.ICE, UpdateBoard);
            if (aILevel == AILevel.Intermediate) ai = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.SEEKER, MySide == Turn.ICE ? AI.Turn.FIRE : AI.Turn.ICE, UpdateBoard);
            if (aILevel == AILevel.Novice) ai = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.DARYLS_PET, MySide == Turn.ICE ? AI.Turn.FIRE : AI.Turn.ICE, UpdateBoard);
            if (aILevel == AILevel.Beginner) ai = gameObject.AddComponent<AI.AI>().Initialize(AI.AIType.MonteCarlo, MySide == Turn.ICE ? AI.Turn.FIRE : AI.Turn.ICE, UpdateBoard);
        }

        CurrentTurn = Turn.ICE;

        if (CurrentTurn == MySide) boardManager.GetLocalMove();
        else {
            if(isSinglePlayer) ai.GetMove(Pieces);

        }
    }
    
    public void UpdateBoard(Move move)
    {
        Log.Push(new LogEntry {
            move = move,
            turn = CurrentTurn,
            capture = Pieces[move.To] == 'W' || Pieces[move.To] == 'B'
        });

        if (Pieces[move.To] == 'W') IcePieceCount--;
        if (Pieces[move.To] == 'B') FirePieceCount--;

        Pieces[move.To] = Pieces[move.From.X, move.From.Y];
        Pieces[move.From] = default(char);

        boardManager.UpdateGUI(move);
        if (GameOver) {
            boardManager.EndGame();
            if (!isSinglePlayer) boardManager.EndGameNetwork();
            return;
        }

        CurrentTurn = CurrentTurn == Turn.ICE ? Turn.FIRE : Turn.ICE;
        if (CurrentTurn == MySide) boardManager.GetLocalMove();
        else
        {
            if (isSinglePlayer) ai.GetMove(Pieces);
        }
    }

	public void Undo() {
		if (Log.Count <= 1) return;

		for (int i = 0; i < 2; i++) {
			LogEntry log = Log.Pop();

			if (log.capture) {
				if (log.turn == Turn.FIRE) IcePieceCount++;
				else FirePieceCount++;
			}

			Pieces[log.move.To] = log.capture ? (log.turn == Turn.FIRE ? 'W' : 'B') : default(char);
			Pieces[log.move.From] = log.turn == Turn.FIRE ? 'B' : 'W';

			boardManager.QueueUndo(log.move, log.turn, log.capture);
		}
	}

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public List<Move> PossibleMoves(bool isIce, Coordinate loc) { return PossibleMoves(isIce ? Turn.ICE : Turn.FIRE, loc); }
    public List<Move> PossibleMoves(Turn turn, Coordinate loc)
    {
        List<Move> result = new List<Move>();

        int myForward = turn == Turn.ICE ? 1 : -1;
        int myTop = turn == Turn.ICE ? 7 : 0;
        char myPiece = turn == Turn.ICE ? 'W' : 'B';

        // Diagonal Left
        if (loc.X != 0 && loc.Y != myTop && Pieces[loc.X - 1, loc.Y + myForward] != myPiece) result.Add(new Move(loc, new Coordinate(loc.X - 1, loc.Y + myForward)));
        // Diagonal Right 
        if (loc.X != 7 && loc.Y != myTop && Pieces[loc.X + 1, loc.Y + myForward] != myPiece) result.Add(new Move(loc, new Coordinate(loc.X + 1, loc.Y + myForward)));
        // Middle
        if (loc.Y != myTop && Pieces[loc.X, loc.Y + myForward] == default(char)) result.Add(new Move(loc, new Coordinate(loc.X, loc.Y + myForward)));

        return result;
    }    
}
