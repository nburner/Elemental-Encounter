using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatLogController : MonoBehaviour
{
    public GameObject moveLog;
    public GameObject chatBox;
    public GameObject rawImage;
    public GameObject chatButton;
    public GameObject logButton;
    public GameObject mapButton;

    private bool chatVisible = true;
    private bool logVisible = true;
    private bool mapVisible = true;

    public void OnChatButtonClicked()
    {
        if (chatVisible)
        {
            chatBox.transform.Translate(new Vector3(-195, 0));
            chatButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Show Chat";
            chatVisible = false;
        }
        else
        {
            chatBox.transform.Translate(new Vector3(195, 0));
            chatButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Hide Chat";
            chatVisible = true;
        }
    }

    public void OnLogButtonClicked()
    {
        if (logVisible)
        {
            moveLog.transform.Translate(new Vector3(195, 0));
            logButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Show Log";
            logVisible = false;
        }
        else
        {
            moveLog.transform.Translate(new Vector3(-195, 0));
            logButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Hide Log";
            logVisible = true;
        }
    }

    public void OnMapButtonClicked()
    {
        if (mapVisible)
        {
            rawImage.transform.Translate(new Vector3(265, 0));
            mapButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Show Map";
            mapVisible = false;
        }
        else
        {
            rawImage.transform.Translate(new Vector3(-265, 0));
            mapButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Hide Map";
            mapVisible = true;
        }
    }
}