using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject quitPanel;

    private GameCore gameCore;

    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        quitPanel.SetActive(false);
    }

    public void MainMenuClicked()
    {
        quitPanel.SetActive(true);
    }

    public void ShowQuitGamePanel()
    {
        quitPanel.SetActive(true);
    }

    public void PlayerConfirmedQuit()
    {
        if (gameCore.isSinglePlayer)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("Game_Lobby");
        }
    }

    public void PlayerDeniedQuit()
    {
        quitPanel.SetActive(false);
    }

    public void RestartGame()
    {
        BoardManager.Instance.ResetBoard();
        BoardManager.Instance.winMenu.SetActive(false);
        BoardManager.Instance.loseMenu.SetActive(false);
    }
}
