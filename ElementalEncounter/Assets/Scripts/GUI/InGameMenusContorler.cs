using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenusContorler : MonoBehaviour {
    
    
    public void StartNewGame()
    {
        BoardManager.Instance.ResetBoard();
        
    }

    public void quitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
