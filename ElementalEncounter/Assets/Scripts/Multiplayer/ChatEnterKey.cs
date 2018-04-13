using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatEnterKey : MonoBehaviour
{
    public InputField input;

	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) BoardManager.Instance.SendMessageChat();
	}
}