using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bitboard = System.UInt64;

public class GameCore : MonoBehaviour
{
    public enum Turn { ICE, FIRE };
    public enum AILevel { Intermediate };

    public bool isSinglePlayer;
    public Turn MySide;
    public Turn CurrentTurn;
    public bool isMasterClient;
    private AI.AI ai;
    private char[,] Pieces;
    private int IcePieceCount;
    private int FirePieceCount;
    public BoardManager boardManager;
    // private GameCharacter character;
    public static GameCore Instance;

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
        Instance = this;

        Pieces = new char[8, 8];
        for (int x = 0; x < 8; x++) for (int y = 0; y < 8; y++) Pieces[x, y] = default(char);
        for (int x = 0; x < 8; x++) for (int y = 0; y < 2; y++) Pieces[x, y] = 'W';
        for (int x = 0; x < 8; x++) for (int y = 6; y < 8; y++) Pieces[x, y] = 'B';

        IcePieceCount = 16;
        FirePieceCount = 16;

        CurrentTurn = Turn.ICE;
    }

    internal void StartSinglePlayerGame(Turn mySide, AILevel aILevel)
    {
        MySide = mySide;
        CurrentTurn = Turn.ICE;
        isMasterClient = true;
        isSinglePlayer = true;

        boardManager = new BoardManager();
        boardManager = BoardManager.Instance;

        if (aILevel == AILevel.Intermediate) ai = new AI.AI(AI.AIType.SEEKER, mySide == Turn.ICE ? AI.Turn.BLACK : AI.Turn.WHITE, this);

        if (CurrentTurn == MySide) boardManager.GetLocalMove();
        else ai.GetMove();
    }

    public void UpdateBoard(int fromX, int fromY, int toX, int toY)
    {
        if (Pieces[toX, toY] == 'W') IcePieceCount--;
        if (Pieces[toX, toY] == 'B') FirePieceCount--;

        Pieces[toX, toY] = Pieces[fromX, fromY];
        Pieces[fromX, fromY] = default(char);

        boardManager.FinishAIMove(fromX, fromY, toX, toY);

        CurrentTurn = CurrentTurn == Turn.ICE ? Turn.FIRE : Turn.ICE;

        if (CurrentTurn == MySide) boardManager.GetLocalMove();
        else
        {
            if (isSinglePlayer) ai.GetMove();
        }
    }

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this);
        Debug.Log("GameCore.Awake()");
    }

    public char[,] PossibleMove(bool isIce, int x, int y) { return PossibleMove(isIce ? Turn.ICE : Turn.FIRE, x, y); }
    public char[,] PossibleMove(Turn turn, int x, int y)
    {
        char[,] r = new char[8, 8];

        int myForward = turn == Turn.ICE ? 1 : -1;
        int myTop = turn == Turn.ICE ? 7 : 0;
        char myPiece = turn == Turn.ICE ? 'W' : 'B';

        // Diagonal Left
        if (x != 0 && y != myTop && Pieces[x - 1, y + myForward] != myPiece) r[x - 1, y + myForward] = 'l';
        // Diagonal Right 
        if (x != 7 && y != myTop && Pieces[x + 1, y + myForward] != myPiece) r[x + 1, y + myForward] = 'r';
        // Middle
        if (y != myTop && Pieces[x, y + myForward] == default(char)) r[x, y + myForward] = 'm';

        return r;
    }

    public void ConvertToBitboards(out bitboard white, out bitboard black)
    {
        white = 0; black = 0;
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                {
                    if (Pieces[x, y] == 'W') { white = AI.Board.set(white, y * 8 + x); }
                    if (Pieces[x, y] == 'B') { black = AI.Board.set(black, y * 8 + x); }
                }
    }
}
