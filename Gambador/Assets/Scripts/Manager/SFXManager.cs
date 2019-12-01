using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SFXManager
{

    public static AudioClip EnterTP;
    public static AudioClip ExitTP;
    public static AudioClip Slash1;
    public static AudioClip Slash2;
    public static AudioClip Slash3;

    public static AudioClip Explosion;
    public static AudioClip Death;
    public static AudioClip OpenDoor;

    public static AudioSource PlayerAudioSource;



    public static void SetSfx()
    {
        EnterTP = Resources.Load("Sounds/SFX/EnterTP") as AudioClip;
        ExitTP =  Resources.Load("Sounds/SFX/ExitTP") as AudioClip; 
        Slash1 = Resources.Load("Sounds/SFX/Slash1") as AudioClip; 
        Slash2 = Resources.Load("Sounds/SFX/Slash2") as AudioClip;
        Slash3 = Resources.Load("Sounds/SFX/Slash3") as AudioClip;
        Explosion = Resources.Load("Sounds/SFX/Explosion") as AudioClip;
        Death = Resources.Load("Sounds/SFX/Death") as AudioClip;
        OpenDoor = Resources.Load("Sounds/SFX/OpenDoor") as AudioClip;
        PlayerAudioSource = GameObject.Find("Player").GetComponent<AudioSource>();
    }

    public static void PlaySFX(AudioClip clip, AudioSource audioSource)
    {
        audioSource.PlayOneShot(clip);
    }

    public static void PlayRandomSlash(AudioSource audioSource)
    {
        var rand = Random.Range(1000, 3999);
        var floorRand = (int) Mathf.Floor(rand / 1000);
        switch (floorRand) {
            case 1 :
                audioSource.PlayOneShot(Slash1);
                break;
            case 2 :
                audioSource.PlayOneShot(Slash2);
                break;
            case 3 :
                audioSource.PlayOneShot(Slash3);
                break;
            default:
                throw new System.ArgumentException("Wrong argument in switch slash play");
        }


        
    }
}
