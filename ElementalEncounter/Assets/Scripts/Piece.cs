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
            if (bm.isMyTurn == false)
            {
                //If capture is Left, plays left capture animation
                if (moveDirection == 'l')
                {
                    selectedPiece.GetComponent<Animation>().Play("LeftBreak");
                    bm.SpawnPiece(5, x, y);
                    Destroy(bm.Pieces[x, y], 3f);
                }

                //If capture is Right, plays right capture animation
                else
                {
                    selectedPiece.GetComponent<Animation>().Play("RightBreak");
                    bm.SpawnPiece(4, x, y);
                    Destroy(bm.Pieces[x, y], 3f);
                }
            }
            else
            {
                //If capture is Left, plays left capture animation
                if (moveDirection == 'l')
                {
                    selectedPiece.GetComponent<Animation>().Play("LeftBreak");
                    bm.SpawnPiece(2, x, y);
                    Destroy(bm.Pieces[x, y], 3f);
                }

                //If capture is Right, plays right capture animation
                else
                {
                    selectedPiece.GetComponent<Animation>().Play("RightBreak");
                    bm.SpawnPiece(3, x, y);
                    Destroy(bm.Pieces[x, y], 3f);
                }
            }
        }
    }
}
