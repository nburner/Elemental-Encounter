using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance { get; set; }

    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    private GameObject clickedSpaceHighlight;

    public Color highlightPrefabColor { get { return highlightPrefab.GetComponent<MeshRenderer>().sharedMaterial.color; } }

    public void Awake()
    {
        Instance = this;
        clickedSpaceHighlight = Instantiate(highlightPrefab);
        highlights = new List<GameObject>();
    }
    public void Start()
    {
    }

    private Color baseColor;
    public void Update()
    {
        clickedSpaceHighlight.GetComponent<MeshRenderer>().material.color = Color.Lerp(new Color(.9f, .9f, .9f, .5f), new Color(.5f,.5f,.5f,.5f), Mathf.PingPong(Time.time, 1));
        for (int i = 0; i < highlights.Count; i++)
        {
            highlights[i].GetComponent<MeshRenderer>().material.color = Color.Lerp(new Color(1,1,1, .5f), baseColor, Mathf.PingPong(Time.time, 1));
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
            go.transform.position = new Vector3(moves[i].To.X + 0.5f, 0.0001f, moves[i].To.Y + 0.5f);
        }
    }

    public void HighlightHint(Move move)
    {
        HideHighlights();
        GameObject go;
        baseColor = new Color((float)(0x35/255.0), 1, (float)(0x1A/255.0), (float).3);

        go = GetHighlightObject();
        go.SetActive(true);
        go.transform.position = new Vector3(move.To.X + 0.5f, 0.0001f, move.To.Y + 0.5f);

        //go = GetHighlightObject();
        //go.SetActive(true);
        //go.transform.position = new Vector3(move.From.X + 0.5f, 0.0001f, move.From.Y + 0.5f);
    }
    public void HighlightClickedSpace(Move move) { HighlightClickedSpace(move.From); }
    public void HighlightClickedSpace(Coordinate from)
    {
        clickedSpaceHighlight.SetActive(true);
        clickedSpaceHighlight.transform.position = new Vector3(from.X + 0.5f, 0.0001f, from.Y + 0.5f);
    }

	public void HighlightPreviousMove(Move move) {
		HideHighlights();
		baseColor = new Color(1f, 0f, 0f, .4f);
				
		GameObject go = GetHighlightObject();
		go.SetActive(true);
		go.transform.position = new Vector3(move.To.X + 0.5f, 0.0001f, move.To.Y + 0.5f);

		go = GetHighlightObject();
		go.SetActive(true);
		go.transform.position = new Vector3(move.From.X + 0.5f, 0.0001f, move.From.Y + 0.5f);
	}

	public void HideHighlights()
    {
        clickedSpaceHighlight.SetActive(false);
        foreach (GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }
}

