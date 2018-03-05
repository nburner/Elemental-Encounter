using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Coordinate Position { set; get; }
    public int CurrentX { get { return Position.X; } }
    public int CurrentY { get { return Position.Y; } }
    public bool isIce;
    public static BoardManager bm;

    public void SetPosition(int x,int y)
    {
        Position = new Coordinate(x, y);        
    }
    public void SetPosition(Coordinate c)
    {
        Position = c;
    }

    public static void playAnimation(Piece selectedPiece, Move m, bool capture)
    {
        bm = BoardManager.Instance;

        if (capture == false)
        {
            //If Movement is Left, plays left animation
            if (m.Direction == Move.Laterality.LEFT)
            {
                selectedPiece.GetComponent<Animation>().Play("Left");
            }

            //If Movement is Right, plays right animation
            else if (m.Direction == Move.Laterality.RIGHT)
            {
                selectedPiece.GetComponent<Animation>().Play("Right");
            }

            //If movement is Forward, plays forward animation
            else
            {
                selectedPiece.GetComponent<Animation>().Play("Forward");
            }
        }
        //else
        //{
        //    if (bm.isMyTurn == false)
        //    {
        //        //If capture is Left, plays left capture animation
        //        if (m.Direction == Move.Laterality.LEFT)
        //        {
        //            selectedPiece.GetComponent<Animation>().Play("LeftBreak");
        //            bm.SpawnPiece(2, m.To);
        //            Destroy(bm.Pieces[m.To], 3f);
        //        }

        //        //If capture is Right, plays right capture animation
        //        else
        //        {
        //            selectedPiece.GetComponent<Animation>().Play("RightBreak");
        //            bm.SpawnPiece(3, m.To);
        //            Destroy(bm.Pieces[m.To], 3f);
        //        }
        //    }
        //    else
        //    {
        //        //If capture is Left, plays left capture animation
        //        if (m.Direction == Move.Laterality.LEFT)
        //        {
        //            selectedPiece.GetComponent<Animation>().Play("LeftBreak");
        //            bm.SpawnPiece(5, m.To);
        //            Destroy(bm.Pieces[m.To], 3f);
        //        }

        //        //If capture is Right, plays right capture animation
        //        else
        //        {
        //            selectedPiece.GetComponent<Animation>().Play("RightBreak");
        //            bm.SpawnPiece(4, m.To);
        //            Destroy(bm.Pieces[m.To], 3f);
        //        }
        //    }
        //}
    }
}
