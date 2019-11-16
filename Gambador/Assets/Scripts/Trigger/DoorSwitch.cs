using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public Vector3 addDest;
    public GameObject Door;
    private Vector3 initialPos;
    private bool isOpen = false;
    void Start()
    {
        initialPos = transform.position;
        PlayerDeathManager.OnPlayerDeath += PlayerDeath;
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Config.PlayerTag && !isOpen)
        {
            StartCoroutine(OpenDoor(addDest));
            isOpen = true;
        }
       
    
    }

    IEnumerator OpenDoor(Vector3 addDest)
    {
        var dest = addDest + Door.transform.position;
        float ratioSpeed = 0;
        while (Door.transform.position != dest)
        {
            Door.transform.position = Vector3.Lerp(Door.transform.position, dest,  ratioSpeed);
            ratioSpeed += Time.deltaTime * Config.TimeScale;         
            yield return 0;
        }
    }
    void PlayerDeath()
    {
        isOpen = false;
        StartCoroutine(OpenDoor(initialPos - transform.position));
    }

}
