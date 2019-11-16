using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform player;
    void Start()
    {
        player = GameObject.Find("Player").transform;

    }

    // Update is called once per frame
    void Update()
    {
       transform.LookAt(new Vector3 (player.position.x, transform.position.y, player.position.z));
    }
}
