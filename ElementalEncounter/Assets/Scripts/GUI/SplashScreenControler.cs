using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Video;

public class SplashScreenControler : MonoBehaviour
{
    public Image SplashImage;
    public string loadLevel;
    public new GameObject camera;
    private GameCore gameCore;

    public void Awake()
    {
        GameObject core = GameObject.Find("GameCore");
        if (core == null)
        {
            gameCore = new GameObject("GameCore").AddComponent<GameCore>();
        }
        else gameCore = core.GetComponent<GameCore>();
    }

    IEnumerator Start()
    {

        camera = GameObject.Find("Main Camera");
        SplashImage.canvasRenderer.SetAlpha(0.0f);

        yield return new WaitForSeconds(2f);
        FadeIn();
        yield return new WaitForSeconds(2f);
        FadeOut();
        yield return new WaitForSeconds(2f);
        camera.GetComponent<AudioSource>().Play();
        var videoPlayer1 = camera.GetComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer1.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        videoPlayer1.Play();

        yield return new WaitForSeconds(14f);

        gameCore.MainMenuAudioStartTime = camera.GetComponent<AudioSource>().time;
        //camera.GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene(loadLevel);
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            camera = GameObject.Find("Main Camera");
            gameCore.MainMenuAudioStartTime = camera.GetComponent<AudioSource>().time;
            //camera.GetComponent<AudioSource>().Stop();
            SceneManager.LoadScene(loadLevel);
        }
    }
    private void FadeIn()
    {
        SplashImage.CrossFadeAlpha(1.0f, 2.0f, false);
    }

    private void FadeOut()
    {
        SplashImage.CrossFadeAlpha(0.0f, 2.0f, false);
    }
}

