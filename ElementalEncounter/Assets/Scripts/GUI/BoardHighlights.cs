using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance { get; set; }

    public GameObject highlightPrefab;
    private List<GameObject> highlights;

    public Color highlightPrefabColor { get { return highlightPrefab.GetComponent<MeshRenderer>().sharedMaterial.color; } }

    public void Awake()
    {
        Instance = this;
        highlights = new List<GameObject>();
    }
    public void Start()
    {
    }

    private Color baseColor;
    public void Update()
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            highlights[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, baseColor, Mathf.PingPong(Time.time, 1));
        }
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }
        return go;
    }
    public void HighlightAllowedMoves(List<Move> moves)
    {
        HideHighlights();
        baseColor = highlightPrefabColor;

        for (int i = 0; i < moves.Count; i++){
            GameObject go = GetHighlightObject();
            go.SetActive(true);
            go.transform.position = new Vector3(moves[i].To.X + 0.5f, 0.0501f, moves[i].To.Y + 0.5f);
        }
    }
    public void HighlightAllowedMoves(char[,] moves)
    {
        HideHighlights();
        baseColor = highlightPrefabColor;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j] != 0)
                {
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, 0.0501f, j + 0.5f);
                }
            }
        }
    }

    public void HighlightHint(Move move)
    {
        HideHighlights();
        GameObject go;
        baseColor = new Color((float)(0x35/255.0), 1, (float)(0x1A/255.0), (float).3);

        go = GetHighlightObject();
        go.SetActive(true);
        go.transform.position = new Vector3(move.To.X + 0.5f, 0.0501f, move.To.Y + 0.5f);

        go = GetHighlightObject();
        go.SetActive(true);
        go.transform.position = new Vector3(move.From.X + 0.5f, 0.0501f, move.From.Y + 0.5f);
    }

    public void HideHighlights()
    {
        foreach (GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }
}

