using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHover : MonoBehaviour
{
    public void OnMouseOver()
    {
        Debug.Log("over");
    }

    public void OnMouseExit()
    {
        Debug.Log("exit");
    }
}