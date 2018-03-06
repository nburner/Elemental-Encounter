using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject quitPanel;

    void Start()
    {
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
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayerDeniedQuit()
    {
        quitPanel.SetActive(false);
    }
}
