using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public Vector3 addDest;
    public GameObject Door;
    private bool isOpen = false;
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !isOpen)
        {
            StartCoroutine(OpenDoor());
            
        }
        isOpen = true;
    
    }

    IEnumerator OpenDoor()
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
}
