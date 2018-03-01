﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //List of Gameobject for spawning pieces on the board
    public List<GameObject> piecePrefabs;
    public List<GameObject> boardPrefabs;
    public List<GameObject> activePieces;

    public bool isMyTurn = true;

    public static BoardManager Instance { set; get; }
    public Piece[,] Pieces { set; get; }

    private Piece selectedPiece;
    private bool isClicked = false;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private int selectionX = -1;
    private int selectionY = -1;
    private GameCore gameCore;


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        gameCore = GameCore.Instance;
        gameCore.boardManager = this;

        SpawnAllBoardSpaces();
        SpawnAllPieces();
    }

    #region Spawners
    private void SpawnAllPieces()
    {
        activePieces = new List<GameObject>();
        Pieces = new Piece[8, 8];
        //spawn Ice team
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(0, i, 0);
            SpawnPiece(0, i, 1);
        }
        //spawn Fire team
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(1, i, 6);
            SpawnPiece(1, i, 7);
        }
    }
    public void SpawnPiece(int index, int x, int y)
    {
        GameObject go = Instantiate(piecePrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Pieces[x, y] = go.GetComponent<Piece>();
        Pieces[x, y].SetPosition(x, y);
        activePieces.Add(go);
    }

    private void SpawnAllBoardSpaces()
    {
        bool isBlackPiece = false;
        bool[,] Spacesboard = new bool[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Spacesboard[i, j] = isBlackPiece;
                if (!isBlackPiece)
                {
                    SpawnBoardSpace(1, i, j);
                }
                else
                {
                    SpawnBoardSpace(0, i, j);
                }
                isBlackPiece = !isBlackPiece;
            }
            isBlackPiece = !isBlackPiece;
        }
    }
    private void SpawnBoardSpace(int index, int x, int y)
    {
        GameObject another = Instantiate(boardPrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        another.transform.SetParent(transform);
    }

    #endregion

    internal void GetLocalMove()
    {
        isMyTurn = true;
    }
    private void Update()
    {
        UpdateSelection();
        
        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedPiece == null)
                {
                    // select the Piece
                    SelectPiece(selectionX, selectionY);
                    Debug.Log("Selected");
                }
                else
                {
                    // Move the Piece
                    //MovePiece(selectionX, selectionY);
                    MakeLocalMove(selectionX, selectionY);
                }
            }
        }
    }

    private void SelectPiece(int x, int y)
    {

        if (Pieces[x, y] == null)
            return;

        if (Pieces[x, y].isIce != isMyTurn)
            return;
        bool hasAtleastOneMove = false;
        var moves = gameCore.PossibleMove(Pieces[x, y].isIce, x, y);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j] != default(char))
                {
                    hasAtleastOneMove = true;
                    break;
                }
            }
            if (hasAtleastOneMove) break;
        }

        if (!hasAtleastOneMove) return;

        selectedPiece = Pieces[x, y];
        BoardHighlights.Instance.HighlightAllowedMoves(moves);

        if (!isClicked)
        {
            isClicked = !isClicked;
        }

    }

    private void MovePiece(int x, int y)
    {
        // TODO: Add winning conditions. Make in different functions.
        char temp = gameCore.PossibleMove(selectedPiece.isIce, selectedPiece.CurrentX, selectedPiece.CurrentY)[x, y];
        if (temp != default(char))
        {
            Piece b = Pieces[x, y];
            if (b != null && b.isIce != isMyTurn)
            {
                // Capture a piece
                activePieces.Remove(b.gameObject);
                Destroy(b.gameObject);
                Piece.playAnimation(selectedPiece, temp, x, y, true);
            }
            else
            {
                Piece.playAnimation(selectedPiece, temp, x, y, false);
            }

            if (isMyTurn)
            {
                if (selectedPiece.CurrentY + 1 == 7)
                {
                    EndGame();
                    return;
                }
            }
            else
            {
                if (selectedPiece.CurrentY - 1 == 0)
                {
                    EndGame();
                    return;
                }
            }

            Pieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;

            selectedPiece.transform.position = GetTileCenter(x, y);
            selectedPiece.SetPosition(x, y);
            Pieces[x, y] = selectedPiece;

            isMyTurn = !isMyTurn;
            //ai.GetMove(Pieces, FinishAIMove);
        }

        if (isClicked)
        {
            isClicked = !isClicked;
        }
        BoardHighlights.Instance.HideHighlights();

        selectedPiece = null;

    }

    private void MakeLocalMove(int x, int y)
    {
        char moveDirection = gameCore.PossibleMove(selectedPiece.isIce, selectedPiece.CurrentX, selectedPiece.CurrentY)[x, y];
        if (moveDirection != default(char) && isMyTurn) //If Valid Move
        {
            gameCore.UpdateBoard(selectedPiece.CurrentX, selectedPiece.CurrentY, x, y);                
        }

        if (isClicked)
        {
            isClicked = !isClicked;
        }
        BoardHighlights.Instance.HideHighlights();

        selectedPiece = null;
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("BreakPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }



    public Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
    
    public void EndGame()
    {
        if (isMyTurn)
        {
            Debug.Log("Ice Wins!");

        }
        else
        {
            Debug.Log("Fire Wins!");
        }

        foreach (GameObject go in activePieces)
        {
            Destroy(go);
        }


        isMyTurn = true; //This is provoking an error Needs to have another winning conditions separetely
        BoardHighlights.Instance.HideHighlights();
        SpawnAllPieces();
    }

    public void FinishAIMove(int fromX, int fromY, int toX, int toY)
    {
        char moveDirection = gameCore.PossibleMove(Pieces[fromX, fromY].isIce, fromX, fromY)[toX, toY];

        bool takingPiece = Pieces[toX, toY] != null;
        if (takingPiece)
        {
            activePieces.Remove(Pieces[toX, toY].gameObject);
            Destroy(Pieces[toX, toY].gameObject, .5f);
        }
        Piece.playAnimation(Pieces[fromX, fromY], moveDirection, toX, toY, takingPiece);

        Pieces[fromX, fromY].transform.position = GetTileCenter(toX, toY);
        Pieces[fromX, fromY].SetPosition(toX, toY);
        Pieces[toX, toY] = Pieces[fromX, fromY];
        Pieces[fromX, fromY] = null;

        if (toY == 0 || toY == 7) EndGame();

        isMyTurn = !isMyTurn;
    }
}
