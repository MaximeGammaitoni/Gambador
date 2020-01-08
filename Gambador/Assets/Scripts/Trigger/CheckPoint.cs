using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool is_win = false;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            PlayerManager.respawnArea = other.transform.position;
            if (this.is_win)
                Debug.Log("Win gg");
        }
            
    }

}
