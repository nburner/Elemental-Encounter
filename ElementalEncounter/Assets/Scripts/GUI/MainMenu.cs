using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject quitPanel;

    AsyncOperation loadingScene;

    public GameObject mainMenu;
    public GameObject gameOptions;
    public GameObject optionsPanel;
    public GameObject instructionsPanel;
    public GameObject generalDescription;
    public GameObject volumeDescription;
    public GameObject mapControllsDescription;
    public GameObject gamePlayDescription;
    public GameObject nextButton;
    public GameObject previousButon;
    public AudioListener audioListen;
    public ToggleGroup aiToggleGroup;
    public ToggleGroup mapToggleGroup;
    public ToggleGroup turnToggleGroup;
    public ToggleGroup soundToggleGroup;
    public ToggleGroup animationToggleGroup;
    public Toggle animationToggle;

    //private bool startGame;

    private GameCore gameCore;
    void Start()
    {
        mainMenu.SetActive(true);
        gameOptions.SetActive(false);
        quitPanel.SetActive(false);
        //startGame = false;
        //loadingScene = SceneManager.LoadSceneAsync("BreakGame", LoadSceneMode.Additive);
        //StartCoroutine(PlaySinglePlayerGame());
        //loadingScene.allowSceneActivation = false;
    }

    void Awake()
    {
        GameObject core = GameObject.Find("GameCore");
        if (core == null)
        {
            gameCore = new GameObject("GameCore").AddComponent<GameCore>();
        }
        else gameCore = core.GetComponent<GameCore>();

        gameCore.animations = true;
    }

    public void singlePlayerButtonClick()
    {
        //startGame = true;
        //gameCore.isSinglePlayer = true;
        //gameCore.aILevel = GameCore.AILevel.Intermediate;
        //gameCore.MySide = GameCore.Turn.ICE;
        //SceneManager.LoadScene("BreakGame");

        mainMenu.SetActive(false);
        gameOptions.SetActive(true);
    }

    public void StartGame()
    {
        IEnumerable<Toggle> aiToggles = aiToggleGroup.ActiveToggles();
        IEnumerable<Toggle> mapToggles = mapToggleGroup.ActiveToggles();
        IEnumerable<Toggle> turnToggles = turnToggleGroup.ActiveToggles();
        string aiText = "";
        string mapText = "";
        string turnText = "";
        foreach (var toggle in aiToggles)
        {
            if (toggle.enabled)
            {
                aiText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
            }
        }
        foreach (var toggle in mapToggles)
        {
            if (toggle.enabled)
            {
                mapText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
            }
        }
        foreach (var toggle in turnToggles)
        {
            if (toggle.enabled)
            {
                turnText = toggle.ToString().Replace(" (UnityEngine.UI.Toggle)", "");
            }
        }

        switch (aiText)
        {
            case "Easy":
                gameCore.aILevel = GameCore.AILevel.Beginner;
                break;
			case "Novice":
				gameCore.aILevel = GameCore.AILevel.Novice;
				break;
			case "Intermediate":
                gameCore.aILevel = GameCore.AILevel.Intermediate;
                break;
            case "Expert":
                gameCore.aILevel = GameCore.AILevel.Expert;
                break;
        }

        switch (mapText)
        {
            case "Ice":
                gameCore.Map = GameCore.MapChoice.ICE;
                break;
            case "Fire":
                gameCore.Map = GameCore.MapChoice.FIRE;
                break;
            case "Clash":
                gameCore.Map = GameCore.MapChoice.CLASH;
                break;
        }

        switch (turnText)
        {
            case "Ice":
                gameCore.MySide = GameCore.Turn.ICE;
                break;
            case "Fire":
                gameCore.MySide = GameCore.Turn.FIRE;
                break;
        }
        gameCore.isSinglePlayer = true;

        SceneManager.LoadScene("BreakGame");
    }

    //public IEnumerator PlaySinglePlayerGame()
    //{
    //   // string time = DateTime.Now.ToString("h:mm:ss tt");

    //    //Debug.Log("started loading GameScene at" + time);


    //    //gameCore.MySide = GameCore.Turn.ICE;
    //    //gameCore.aILevel = GameCore.AILevel.Intermediate;
    //    gameCore.isSinglePlayer = true;

    //    while (!loadingScene.isDone)
    //    {
    //        Debug.Log(loadingScene.progress);

    //        if (startGame == true) loadingScene.allowSceneActivation = true;

    //        yield return null;
    //    }

    //    SceneManager.SetActiveScene(SceneManager.GetSceneByName("BreakGame"));
    //    SceneManager.UnloadSceneAsync("MainMenu");
    //}
    public void PlayMultiplayerGame()
    {
        SceneManager.LoadScene("Game_Lobby");
    }

    public void ShowQuitGamePanel()
    {
        quitPanel.SetActive(true);
    }

    public void PlayerConfirmedQuit()
    {
        Application.Quit();
    }

    public void PlayerDeniedQuit()
    {
        quitPanel.SetActive(false);
    }
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        gameOptions.SetActive(false);
        quitPanel.SetActive(false);
        optionsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
    }
    public void ShowOptions()
    {
        optionsPanel.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ShowInstructions()
    {
        mainMenu.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void ShowNextInstruction()
    {
        previousButon.GetComponent<Button>().interactable = true;
        if (mapControllsDescription.activeSelf) nextButton.GetComponent<Button>().interactable = false;

        if (generalDescription.activeSelf)
        {
            generalDescription.SetActive(false);
            volumeDescription.SetActive(true);
        }
        else if (volumeDescription.activeSelf)
        {
            volumeDescription.SetActive(false);
            mapControllsDescription.SetActive(true);
        }
        else if (mapControllsDescription.activeSelf)
        {
            mapControllsDescription.SetActive(false);
            gamePlayDescription.SetActive(true);
        }
    }

    public void ShowPreviousInstruction()
    {
        nextButton.GetComponent<Button>().interactable = true;
        if (volumeDescription.activeSelf) previousButon.GetComponent<Button>().interactable = false;

        if (gamePlayDescription.activeSelf)
        {
            gamePlayDescription.SetActive(false);
            mapControllsDescription.SetActive(true);
        }
        else if (mapControllsDescription.activeSelf)
        {
            mapControllsDescription.SetActive(false);
            volumeDescription.SetActive(true);
        }
        else if (volumeDescription.activeSelf)
        {
            volumeDescription.SetActive(false);
            generalDescription.SetActive(true);
        }
    }

    public void OnAnimationToggleChange()
    {
        gameCore.animations = !animationToggle.enabled;
    }
}