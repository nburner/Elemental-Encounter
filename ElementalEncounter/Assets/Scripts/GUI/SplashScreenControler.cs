using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SplashScreenControler : MonoBehaviour
{
    public Image SplashImage;
    public string loadLevel;
    public new GameObject camera;

    IEnumerator Start()
    {

        camera = GameObject.Find("Main Camera");
        SplashImage.canvasRenderer.SetAlpha(0.0f);

        yield return new WaitForSeconds(2f);
        FadeIn();
        yield return new WaitForSeconds(2f);
        FadeOut();
        yield return new WaitForSeconds(2f);

        var videoPlayer = camera.GetComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        videoPlayer.Play();

        yield return new WaitForSeconds(15f);

        SceneManager.LoadScene(loadLevel);

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

