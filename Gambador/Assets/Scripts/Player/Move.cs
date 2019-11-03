using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private RaycastManager raycastManager;
    private LineRenderer lr;
    private float playerHeight;
    private float playerWidth;
    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GameManager.singleton.RaycastManager;
        lr = gameObject.transform.Find("Laser").GetComponent<LineRenderer>();
        playerHeight = gameObject.GetComponent<MeshRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        raycastManager.MovingRaycastWithLineRenderer(transform, lr, playerHeight);

    }


}
