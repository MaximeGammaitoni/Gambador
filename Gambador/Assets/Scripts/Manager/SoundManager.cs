using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager 
{
    public AudioClip corridorBGM;
    public AudioClip level1;
    public AudioClip level2;
    public AudioClip level3;
    public AudioClip level4;
    public AudioClip level5;
    public AudioSource AudioSource;
    public SoundManager()
    {
        AudioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        corridorBGM = Resources.Load("Sounds/BGM/corridor") as AudioClip;
        level1 = Resources.Load("Sounds/BGM/Level1") as AudioClip;
        level2 = Resources.Load("Sounds/BGM/Level2") as AudioClip;
        level3 = Resources.Load("Sounds/BGM/Level3") as AudioClip;

        this.AudioSource.clip = corridorBGM;
        this.AudioSource.time = 10;
        this.AudioSource.Play();
    }
    public void ChangeAudioClip(AudioClip ac, float time)
    {
        GameManager.singleton.StartCoroutine(AudioController.FadeOutFadeIn(AudioSource,ac,0.2f, time));
    }


}
