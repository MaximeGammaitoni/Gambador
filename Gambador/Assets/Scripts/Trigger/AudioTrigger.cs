using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public enum AcEnum {level1,level2,level3,level4,level5, corridor };
    public AcEnum acEnum;
    public float time;
    public AudioClip ac;
    private void Start()
    {

        if(acEnum == AcEnum.level1)
        {
            ac = GameManager.singleton.SoundManager.level1;
        }
        else if (acEnum == AcEnum.level2)
        {
            ac = GameManager.singleton.SoundManager.level2;
        }
        else if (acEnum == AcEnum.level3)
        {
            ac = GameManager.singleton.SoundManager.level3;
        }
        else if (acEnum == AcEnum.level4)
        {
            ac = GameManager.singleton.SoundManager.level4;
        }
        else if (acEnum == AcEnum.level5)
        {
            ac = GameManager.singleton.SoundManager.level5;
        }
        else if(acEnum == AcEnum.corridor)
        {
            ac = GameManager.singleton.SoundManager.corridorBGM;
        }
    }
    void OnTriggerEnter(Collider col)
    {

        if(col.tag == Config.PlayerTag)
        {
            GameManager.singleton.SoundManager.ChangeAudioClip(ac, time);
            Destroy(gameObject);
        }
    }
}
