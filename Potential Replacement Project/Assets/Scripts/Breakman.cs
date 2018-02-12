using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakman : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isIce;

    public void SetPosition(int x,int y)
    {
        CurrentX = x;
        CurrentY = y;
        
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}
