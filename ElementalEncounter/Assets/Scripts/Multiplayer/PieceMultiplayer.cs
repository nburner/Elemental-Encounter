using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMultiplayer : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isIce;

    public void SetPosition(int x,int y)
    {
        CurrentX = x;
        CurrentY = y;
        
    }

    public char[,] PossibleMove()
    {
        char[,] r = new char[8, 8];
        PieceMultiplayer b;

        if (isIce)
        {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 7)
            {
                b = NetworkBoardManager.Instance.BreakmanNet[CurrentX - 1, CurrentY + 1];
                if (b == null || !b.isIce)
                {
                    r[CurrentX - 1, CurrentY + 1] = 'l';
                }
            }

            // Diagonal Right 
            if (CurrentX != 7 && CurrentY != 7)
            {
                b = NetworkBoardManager.Instance.BreakmanNet[CurrentX + 1, CurrentY + 1];
                if (b == null || !b.isIce)
                {
                    r[CurrentX + 1, CurrentY + 1] = 'r';
                }
            }

            // Middle
            if (CurrentY != 7)
            {
                b = NetworkBoardManager.Instance.BreakmanNet[CurrentX, CurrentY + 1];
                if (b == null)
                {
                    r[CurrentX, CurrentY + 1] = 'm';
                }
            }
        }
        else
        {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 0)
            {
                b = NetworkBoardManager.Instance.BreakmanNet[CurrentX - 1, CurrentY - 1];
                if (b == null || b.isIce)
                {
                    r[CurrentX - 1, CurrentY - 1] = 'l';
                }
            }

            // Diagonal Right 
            if (CurrentX != 7 && CurrentY != 0)
            {
                b = NetworkBoardManager.Instance.BreakmanNet[CurrentX + 1, CurrentY - 1];
                if (b == null || b.isIce)
                {
                    r[CurrentX + 1, CurrentY - 1] = 'r';
                }
            }

            // Middle
            if (CurrentY != 0)
            {
                b = NetworkBoardManager.Instance.BreakmanNet[CurrentX, CurrentY - 1];
                if (b == null)
                {
                    r[CurrentX, CurrentY - 1] = 'm';
                }
            }
        }

        return r;
    }
}
