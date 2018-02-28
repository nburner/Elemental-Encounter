using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isIce;
    public static BoardManager bm;

    public void SetPosition(int x,int y)
    {
        CurrentX = x;
        CurrentY = y;
        
    }

    public char[,] PossibleMove()
    {
        char[,] r = new char[8, 8];
        Piece b;

        if (isIce)
        {
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 7)
            {
                b = BoardManager.Instance.Breakmans[CurrentX - 1, CurrentY + 1];
                if (b == null || !b.isIce)
                {
                    r[CurrentX - 1, CurrentY + 1] = 'l';
                }
            }

            // Diagonal Right 
            if (CurrentX != 7 && CurrentY != 7)
            {
                b = BoardManager.Instance.Breakmans[CurrentX + 1, CurrentY + 1];
                if (b == null || !b.isIce)
                {
                    r[CurrentX + 1, CurrentY + 1] = 'r';
                }
            }

            // Middle
            if (CurrentY != 7)
            {
                b = BoardManager.Instance.Breakmans[CurrentX, CurrentY + 1];
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
                b = BoardManager.Instance.Breakmans[CurrentX - 1, CurrentY - 1];
                if (b == null || b.isIce)
                {
                    r[CurrentX - 1, CurrentY - 1] = 'l';
                }
            }

            // Diagonal Right 
            if (CurrentX != 7 && CurrentY != 0)
            {
                b = BoardManager.Instance.Breakmans[CurrentX + 1, CurrentY - 1];
                if (b == null || b.isIce)
                {
                    r[CurrentX + 1, CurrentY - 1] = 'r';
                }
            }

            // Middle
            if (CurrentY != 0)
            {
                b = BoardManager.Instance.Breakmans[CurrentX, CurrentY - 1];
                if (b == null)
                {
                    r[CurrentX, CurrentY - 1] = 'm';
                }
            }
        }

        return r;
    }

    public static void playAnimation(Piece selectedPiece, char moveDirection, int x, int y, bool capture)
    {
        bm = BoardManager.Instance;

        if (capture == false)
        {
            //If Movement is Left, plays left animation
            if (moveDirection == 'l')
            {
                selectedPiece.GetComponent<Animation>().Play("Left");
            }

            //If Movement is Right, plays right animation
            else if (moveDirection == 'r')
            {
                selectedPiece.GetComponent<Animation>().Play("Right");
            }

            //If movement is Forward, plays forward animation
            else
            {
                selectedPiece.GetComponent<Animation>().Play("Forward");
            }
        }
        else
        {
            if (bm.isIceTurn == false)
            {
                //If capture is Left, plays left capture animation
                if (moveDirection == 'l')
                {
                    selectedPiece.GetComponent<Animation>().Play("LeftBreak");
                    bm.SpawnBreakman(5, x, y);
                    Destroy(bm.Breakmans[x, y], 3f);
                }

                //If capture is Right, plays right capture animation
                else
                {
                    selectedPiece.GetComponent<Animation>().Play("RightBreak");
                    bm.SpawnBreakman(4, x, y);
                    Destroy(bm.Breakmans[x, y], 3f);
                }
            }
            else
            {
                //If capture is Left, plays left capture animation
                if (moveDirection == 'l')
                {
                    selectedPiece.GetComponent<Animation>().Play("LeftBreak");
                    bm.SpawnBreakman(2, x, y);
                    Destroy(bm.Breakmans[x, y], 3f);
                }

                //If capture is Right, plays right capture animation
                else
                {
                    selectedPiece.GetComponent<Animation>().Play("RightBreak");
                    bm.SpawnBreakman(3, x, y);
                    Destroy(bm.Breakmans[x, y], 3f);
                }
            }
        }
    }
}
