using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == Config.PlayerTag)
        {
            GameManager.singleton.PlayerDeathManager.PlayerDeathTrigger();
        }
    }
}
