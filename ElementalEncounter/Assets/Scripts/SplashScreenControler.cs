using System.Collections;
using System.Collections.Generic;
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

        FadeIn();
        yield return new WaitForSeconds(2f);
        FadeOut();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(loadLevel);
    }

    private void FadeIn()
    {
        SplashImage.CrossFadeAlpha(1.0f, 1.0f, false);
    }

    private void FadeOut()
    {
        SplashImage.CrossFadeAlpha(0.0f, 1.0f, false);
    }
}
