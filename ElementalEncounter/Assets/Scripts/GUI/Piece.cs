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

    public bool Undone = false;

    public AudioClip captureSound;

    public void SetPosition(int x, int y)
    {
        Position = new Coordinate(x, y);
    }
    public void SetPosition(Coordinate c)
    {
        Position = c;
    }

    public IEnumerator PlayMoveSound()
    {
        GetComponent<AudioSource>().Play();                 //This starts the sound
        for (int i = 0; i < 140; i++) yield return null;    //This waits for a number of frames (animations go for 100)
        GetComponent<AudioSource>().Stop();                 //This stops the sound
    }

    public static void playAnimation(Piece selectedPiece, Move m, bool capture)
    {
        bm = BoardManager.Instance;

        if (capture == false)
        {
            //If Movement is Left, plays left animation
            if (m.Direction == Move.Laterality.LEFT)
            {
                selectedPiece.GetComponent<Animation>()["Left"].speed = 1;
                selectedPiece.GetComponent<Animation>().Play("Left");
            }

            //If Movement is Right, plays right animation
            else if (m.Direction == Move.Laterality.RIGHT)
            {
                selectedPiece.GetComponent<Animation>()["Right"].speed = 1;
                selectedPiece.GetComponent<Animation>().Play("Right");
            }

            //If movement is Forward, plays forward animation
            else
            {
                selectedPiece.GetComponent<Animation>()["Forward"].speed = 1;
                selectedPiece.GetComponent<Animation>().Play("Forward");
            }
        }
        else
        {
            System.Random rnd = new System.Random();
            int captureChoice;

            if (selectedPiece.isIce == false)
            {
                //If capture is Left, plays left capture animation
                if (m.Direction != Move.Laterality.LEFT)
                {
                    captureChoice = rnd.Next(1, 5);

                    if (captureChoice == 2)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Bounce - Left");
                        bm.SpawnPiece(2, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else if (captureChoice == 3)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Roll - Left");
                        bm.SpawnPiece(3, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else
                    {
                        selectedPiece.GetComponent<Animation>().Play("Smash - Left");
                        bm.SpawnPiece(4, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }

                }

                //If capture is Right, plays right capture animation
                else
                {
                    captureChoice = 6;

                    if (captureChoice == 5)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Bounce - Right");
                        bm.SpawnPiece(5, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else if (captureChoice == 6)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Roll - Right");
                        bm.SpawnPiece(6, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else
                    {
                        selectedPiece.GetComponent<Animation>().Play("Smash - Right");
                        bm.SpawnPiece(7, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                }
            }
            else
            {
                //If capture is Left, plays left capture animation
                if (m.Direction == Move.Laterality.LEFT)
                {
                    captureChoice = rnd.Next(7, 11);

                    if (captureChoice == 8)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Diagonal - Left");
                        bm.SpawnPiece(8, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else if (captureChoice == 9)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Dig - Left");
                        bm.SpawnPiece(9, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else
                    {
                        selectedPiece.GetComponent<Animation>().Play("Top - Left");
                        bm.SpawnPiece(10, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                }

                //If capture is Right, plays right capture animation
                else
                {
                    captureChoice = rnd.Next(10, 14);

                    if (captureChoice == 11)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Diagonal - Right");
                        bm.SpawnPiece(11, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else if (captureChoice == 12)
                    {
                        selectedPiece.GetComponent<Animation>().Play("Dig - Right");
                        bm.SpawnPiece(12, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                    else
                    {
                        selectedPiece.GetComponent<Animation>().Play("Top - Right");
                        bm.SpawnPiece(13, m.To);
                        Destroy(bm.Pieces[m.To].gameObject, 3f);
                    }
                }
            }
        }
    }
}