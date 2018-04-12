using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicControler : MonoBehaviour
{
    [Header("Set in Inspector")]
    public AudioClip mainMenuMusic;
    public AudioClip iceMapMusic;
    public AudioClip fireMapMusic;
    public AudioClip clashMapMusic;
    public Slider slider;

    private AudioSource audioSource;

    // Use this for initialization
    private void Awake()
    {
        DontDestroyOnLoad(this);

        audioSource = GameObject.Find("MusicController").GetComponent<AudioSource>();
        if (!audioSource.isPlaying)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.Play();
        }
    }

    public void OnVolumeSliderChanged()
    {
        audioSource.volume = slider.value;
    }

    public void PlayMapMusic(GameCore.MapChoice map)
    {
        audioSource.Stop();

        if (map == GameCore.MapChoice.CLASH)
        {
            audioSource.clip = clashMapMusic;
        }
        else if (map == GameCore.MapChoice.FIRE)
        {
            audioSource.clip = fireMapMusic;
        }
        else
        {
            audioSource.clip = iceMapMusic;
        }

        audioSource.Play();
    }

    public void PlayDefaultMusic()
    {
        audioSource = GameObject.Find("MusicController").GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = mainMenuMusic;
        audioSource.Play();
    }
}