﻿using System.Collections;
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

    private void Awake()
    {
        GetComponent<AudioSource>().Pause();
    }

    public void SetPosition(int x, int y)
    {
        Position = new Coordinate(x, y);
    }
    public void SetPosition(Coordinate c)
    {
        Position = c;
    }

    private float moveSoundTime = 0f;
    public IEnumerator PlayMoveSound(bool forward = true)
    {
        //GetComponent<AudioSource>().time = moveSoundTime;

        GetComponent<AudioSource>().UnPause();                 //This starts the sound
        GetComponent<AudioSource>().pitch = forward ? 1 : -1;
        for (int i = 0; i < 140; i++) {
            if (this == null) yield break;
            yield return null;    //This waits for a number of frames (animations go for 100)
        }
        GetComponent<AudioSource>().Pause();                 //This stops the sound
        //moveSoundTime = GetComponent<AudioSource>().time;
    }

    public static int playAnimation(Piece selectedPiece, Move m, bool capture)
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
                string[] animationList = { "Bounce - Left", "Roll - Left", "Smash - Left", "Bounce - Right", "Roll - Right", "Smash - Right" };
                for (int i = 0; i < animationList.Length; i++){
                    selectedPiece.GetComponent<Animation>()[animationList[i]].time = 0;
                    selectedPiece.GetComponent<Animation>()[animationList[i]].speed = 1;
                }

                //If capture is Left, plays left capture animation
                if (m.Direction != Move.Laterality.LEFT)
                {
                    captureChoice = rnd.Next(2, 5);

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
                    captureChoice = rnd.Next(5, 8);

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
                string[] animationList = { "Diagonal - Left", "Dig - Left", "Top - Left", "Diagonal - Right", "Dig - Right", "Top - Right" };
                for (int i = 0; i < animationList.Length; i++){
                    selectedPiece.GetComponent<Animation>()[animationList[i]].time = 0;
                    selectedPiece.GetComponent<Animation>()[animationList[i]].speed = 1;
                }

                //If capture is Left, plays left capture animation
                if (m.Direction == Move.Laterality.LEFT)
                {
                    captureChoice = rnd.Next(8, 11);

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
                    captureChoice = rnd.Next(11, 14);

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
            return captureChoice;
        }
        return -1;
    }
}