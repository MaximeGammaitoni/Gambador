using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBackGround : MonoBehaviour
{
    private Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
    }
}
