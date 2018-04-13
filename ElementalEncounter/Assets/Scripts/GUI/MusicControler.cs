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
    public Sprite[] image;
    public Button volumeButton;

    private AudioSource audioSource;

    // Use this for initialization
    private void Awake()
    {
        DontDestroyOnLoad(this);

        audioSource = GameObject.Find("MusicController").GetComponent<AudioSource>();
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", slider.value);
            PlayerPrefs.Save();
        }
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        slider.value = audioSource.volume;
        ChangeIcon();

        if (!audioSource.isPlaying)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.Play();
        }
    }

    public void OnVolumeSliderChanged()
    {
        audioSource.volume = slider.value;
        PlayerPrefs.SetFloat("MusicVolume", slider.value);
        PlayerPrefs.Save();
        ChangeIcon();
    }

    private void ChangeIcon()
    {
        if (slider.value == 0)
        {
            volumeButton.image.sprite = image[0];
        }
        else if (slider.value > 0 && slider.value < 0.3f)
        {
            volumeButton.image.sprite = image[1];
        }
        else if (slider.value > 0.3f && slider.value < 0.7f)
        {
            volumeButton.image.sprite = image[2];
        }
        else if (slider.value > 0.7)
        {
            volumeButton.image.sprite = image[3];
        }
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