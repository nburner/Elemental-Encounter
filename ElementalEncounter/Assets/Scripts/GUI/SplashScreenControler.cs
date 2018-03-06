using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SplashScreenControler : MonoBehaviour
{
    public Image SplashImage;
    public string loadLevel;

    IEnumerator Start()
    {
        SplashImage.canvasRenderer.SetAlpha(0.0f);

        yield return new WaitForSeconds(2f);
        FadeIn();
        yield return new WaitForSeconds(2f);
        FadeOut();
        yield return new WaitForSeconds(2f);
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
