using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform player;
    public bool canLookAt =true;
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(canLookAt)
            transform.LookAt(new Vector3 (player.position.x, transform.position.y, player.position.z));
    }
    private void OnEnable()
    {
        canLookAt = canLookAt;
    }
}
