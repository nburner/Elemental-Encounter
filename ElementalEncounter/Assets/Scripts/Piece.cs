using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isIce;

    public void SetPosition(int x,int y)
    {
        CurrentX = x;
        CurrentY = y;
        
    }

    public bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        Piece b;

        if (isIce)
        {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 7)
            {
                b = BoardManager.Instance.Breakmans[CurrentX - 1, CurrentY + 1];
                if (b == null || !b.isIce)
                {
                    r[CurrentX - 1, CurrentY + 1] = true;
                }
            }

            // Diagonal Right 
            if (CurrentX != 7 && CurrentY != 7)
            {
                b = BoardManager.Instance.Breakmans[CurrentX + 1, CurrentY + 1];
                if (b == null || !b.isIce)
                {
                    r[CurrentX + 1, CurrentY + 1] = true;
                }
            }

            // Middle
            if (CurrentY != 7)
            {
                b = BoardManager.Instance.Breakmans[CurrentX, CurrentY + 1];
                if (b == null)
                {
                    r[CurrentX, CurrentY + 1] = true;
                }
            }
        }
        else
        {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 0)
            {
                b = BoardManager.Instance.Breakmans[CurrentX - 1, CurrentY - 1];
                if (b == null || b.isIce)
                {
                    r[CurrentX - 1, CurrentY - 1] = true;
                }
            }

            // Diagonal Right 
            if (CurrentX != 7 && CurrentY != 0)
            {
                b = BoardManager.Instance.Breakmans[CurrentX + 1, CurrentY - 1];
                if (b == null || b.isIce)
                {
                    r[CurrentX + 1, CurrentY - 1] = true;
                }
            }

            // Middle
            if (CurrentY != 0)
            {
                b = BoardManager.Instance.Breakmans[CurrentX, CurrentY - 1];
                if (b == null)
                {
                    r[CurrentX, CurrentY - 1] = true;
                }
            }
        }

        return r;
    }
}
