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

    public VideoClip[] videos;

    IEnumerator Start()
    {

        camera = GameObject.Find("Main Camera");
        SplashImage.canvasRenderer.SetAlpha(0.0f);

        yield return new WaitForSeconds(2f);
        FadeIn();
        yield return new WaitForSeconds(2f);
        FadeOut();
        yield return new WaitForSeconds(2f);

        var videoPlayer1 = camera.GetComponent<UnityEngine.Video.VideoPlayer>();
        var videoPlayer2 = camera.GetComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer1.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        videoPlayer1.Play();

        yield return new WaitForSeconds(15f);

        videoPlayer1.Stop();
        videoPlayer1 = null;

        videoPlayer2.renderMode = VideoRenderMode.CameraNearPlane;

        videoPlayer2.Play();

        

        

 

    }

    private void Update()
    {
        if (Input.anyKey)
        {
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

