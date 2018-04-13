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
        BoardManager.Instance.panelContainer.SetActive(true);
        BoardManager.Instance.mapPanel.SetActive(true);
        quitPanel.SetActive(true);
    }

    public void ShowQuitGamePanel()
    {
        quitPanel.SetActive(true);
    }

    public void PlayerConfirmedQuit()
    {
        GameObject mcg = GameObject.Find("MusicController");
        if (mcg != null)
        {
            MusicControler mc = mcg.GetComponent<MusicControler>();
            mc.PlayDefaultMusic();
        }
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
        BoardManager.Instance.panelContainer.SetActive(false);
        BoardManager.Instance.mapPanel.SetActive(false);
        quitPanel.SetActive(false);
    }

    public void RestartGame()
    {
        BoardManager.Instance.ResetBoard();
        BoardManager.Instance.winMenu.SetActive(false);
        BoardManager.Instance.loseMenu.SetActive(false);
        BoardManager.Instance.panelContainer.SetActive(false);
        BoardManager.Instance.mapPanel.SetActive(false);
    }
}
