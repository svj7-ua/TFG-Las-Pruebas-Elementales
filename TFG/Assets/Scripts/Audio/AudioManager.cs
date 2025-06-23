using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Boss Audio Sources")]

    public AudioClip widowBossMusicSource;
    public AudioClip golemBossMusicSource;
    public AudioClip lordsBossMusicSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip fire;
    public AudioClip fireBall;
    public AudioClip wind;
    public AudioClip arcane;
    public AudioClip poison;
    public AudioClip lightning;
    public AudioClip heal;
    public AudioClip beam;
    public AudioClip beam_2;


    void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Attempted to play a null audio clip.");
        }
    }

    public void PlayBossBattleMusic(AudioClip bossMusicSource)
    {
        if (musicSource.clip != bossMusicSource)
        {
            musicSource.Stop(); // Stop the current music
            musicSource.clip = bossMusicSource; // Replace with the boss music clip
            musicSource.Play();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource.clip != backgroundMusic)
        {
            musicSource.Stop(); // Stop the current music
            musicSource.clip = backgroundMusic; // Replace with the normal background music clip
            musicSource.Play();
        }
    }

}
