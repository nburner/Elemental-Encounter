using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : Breakman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        Breakman b;

        if(isIce)
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
