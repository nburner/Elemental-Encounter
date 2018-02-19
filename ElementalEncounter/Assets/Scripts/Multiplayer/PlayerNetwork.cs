using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour
{
    public static PlayerNetwork Instance;
    public string PlayerName { get; set; }

    private void Awake()
    {
        Instance = this;
        PlayerName = "New#" + Random.Range(1000, 9999);
    }
}
