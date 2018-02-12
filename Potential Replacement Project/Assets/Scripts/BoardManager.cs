using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    public bool[,] AllowedMoves { set; get; }

    public Breakman[,] Breakmans { set; get; }
    private Breakman selectedBreakman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    private int selectionX = -1;
    private int selectionY = -1;

    //List of Gameobject for spawning pieces on the board
    public List<GameObject> breakmanPrefabs;
    private List<GameObject> activeBreakman;

    public bool isIceTurn = true;

    private void Start()
    {
        Instance = this;
        SpawnAllBreakPieces();
    }
    private void Update()
    {
        UpdateSelection();
        DrawBoard();

        if (Input.GetMouseButtonDown(0))
        {
            if(selectionX>=0 && selectionY >= 0)
            {
                if (selectedBreakman == null)
                {
                    // select the Breakman
                    SelectBreakMan(selectionX, selectionY);
                }
                else
                {
                    // Move the Breakman
                    MoveBreakman(selectionX, selectionY);
                }
            }
        }
    }

    private void SelectBreakMan(int x, int y)
    {
        if (Breakmans[x, y] == null)
            return;

        if (Breakmans[x, y].isIce != isIceTurn)
            return;
        AllowedMoves = Breakmans[x, y].PossibleMove();
        selectedBreakman = Breakmans[x, y];
        BoardHighlights.Instance.HighlightAllowedMoves(AllowedMoves);
    }

    private void MoveBreakman(int x, int y)
    {
        // TODO: Add winning conditions. Make in different functions.
        if (AllowedMoves[x, y])
        {
            Breakman b = Breakmans[x, y];
            if (b != null && b.isIce && !isIceTurn)
            {
                // Capture a piece
                activeBreakman.Remove(b.gameObject);
                Destroy(b.gameObject);
            }

            if (isIceTurn)
            {
                if (selectedBreakman.CurrentY + 1 == 7)
                {
                    EndGame();
                }
            }
            else
            {
                if (selectedBreakman.CurrentY - 1 == 0)
                {
                    EndGame();
                }
            }

            Breakmans[selectedBreakman.CurrentX, selectedBreakman.CurrentY] = null;
            selectedBreakman.transform.position = GetTileCenter(x, y);
            selectedBreakman.SetPosition(x, y);
            Breakmans[x, y] = selectedBreakman;
            isIceTurn = !isIceTurn;
        }

        BoardHighlights.Instance.HideHighlights();

        selectedBreakman = null;
        
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

    private void DrawBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        //Draw the Selection

        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(
                Vector3.forward * (selectionY+1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    private void SpawnBreakman(int index, int x , int y)
    {
        string numberOfPrefab = "Number" + breakmanPrefabs.Capacity;
        Debug.Log(numberOfPrefab);
        GameObject go = Instantiate(breakmanPrefabs[index], GetTileCenter(x,y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Breakmans[x, y] = go.GetComponent<Breakman>();
        Breakmans[x, y].SetPosition(x, y);
        activeBreakman.Add(go);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void SpawnAllBreakPieces()
    {
        activeBreakman = new List<GameObject>();
        Breakmans = new Breakman[8, 8];
        //spawn Ice team
        for (int i = 0; i < 8; i++)
        {
            SpawnBreakman(0, i, 0);
            SpawnBreakman(0, i, 1);
        }
        //spawn Fire team
        for (int i = 0; i < 8; i++)
        {
            SpawnBreakman(1, i, 6);
            SpawnBreakman(1, i, 7);
        }
    }

    private void EndGame()
    {
        if (isIceTurn)
        {
            Debug.Log("Fire Wins!");
        }
        else
        {
            Debug.Log("Ice Wins!");
        }

        foreach (GameObject go in activeBreakman)
        {
            Destroy(go);
        }

        isIceTurn = !isIceTurn;
        BoardHighlights.Instance.HideHighlights();
        SpawnAllBreakPieces();
    }
}
