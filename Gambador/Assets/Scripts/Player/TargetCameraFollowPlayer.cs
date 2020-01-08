using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCameraFollowPlayer : MonoBehaviour
{
    private GameObject player;
    private Vector3 Distance;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        Distance = player.transform.position + transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + Distance.x, player.transform.position.y + Distance.y, player.transform.position.z + Distance.z);
    }
}
