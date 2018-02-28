using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Threading.Tasks;
using bitboard = System.UInt64;

public class AIMovement : MonoBehaviour
{
    public enum Turn { ICE, FIRE };

    [DllImport("Assets/Plugins/AILibrary.dll")]
    public static extern void Seeker(bitboard white, bitboard black, Turn t, ref int from, ref int to);

    public char[,] aiAllowedMoves { set; get; }

    private BoardManager bm = BoardManager.Instance;
    private Piece[,] Breakmans;
    private List<GameObject> activeBreakman;
    private bitboard white;
    private bitboard black;

    public async void MakeAIMove()
    {
        white = 0;
        black = 0;
        Breakmans = bm.Breakmans;
        activeBreakman = bm.activeBreakman;

        ConvertGameboard();

        int from = 48; int to = 41;
        Seeker(white, black, bm.isIceTurn ? Turn.ICE : Turn.FIRE, ref from, ref to);

        int toX = to % 8;
        int toY = to / 8;
        int fromX = from % 8;
        int fromY = from / 8;

        await Task.Delay(System.TimeSpan.FromSeconds(2));

        AnimateAiMovement(toX, toY, fromX, fromY);
        UpdatePiecePosition(toX, toY, fromX, fromY);

        bm.isIceTurn = !bm.isIceTurn;
    }

    private void ConvertGameboard()
    {
        //Convert Gameboard
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (Breakmans[x, y] != null)
                {
                    if (Breakmans[x, y].isIce) { white = AI.Board.set(white, y * 8 + x); }
                    if (!Breakmans[x, y].isIce) { black = AI.Board.set(black, y * 8 + x); }
                }
            }
        }
    }

    private void AnimateAiMovement(int toX, int toY, int fromX, int fromY)
    {
        aiAllowedMoves = Breakmans[fromX, fromY].PossibleMove();
        char aiTemp = aiAllowedMoves[toX, toY];

        if (Breakmans[toX, toY] != null)
        {
            // Capture a piece
            activeBreakman.Remove(Breakmans[toX, toY].gameObject);
            Destroy(Breakmans[toX, toY].gameObject, .5f);
            Piece.playAnimation(Breakmans[fromX, fromY], aiTemp, toX, toY, true);
        }
        else
        {
            Piece.playAnimation(Breakmans[fromX, fromY], aiTemp, toX, toY, false);
        }

        //Piece selectedBreakman = Breakmans[fromX, fromY];
        if ((bm.isIceTurn && Breakmans[fromX, fromY].CurrentY + 1 == 7) || (!bm.isIceTurn && Breakmans[fromX, fromY].CurrentY - 1 == 0))
        {
            bm.EndGame();
            return;
        }
    }

    private void UpdatePiecePosition(int toX, int toY, int fromX, int fromY)
    {
        Breakmans[fromX, fromY].transform.position = bm.GetTileCenter(toX, toY);
        Breakmans[fromX, fromY].SetPosition(toX, toY);
        Breakmans[toX, toY] = Breakmans[fromX, fromY];
        Breakmans[fromX, fromY] = null;
    }
}
