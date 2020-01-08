using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class rotateCube : MonoBehaviour
{
    public Vector3 speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed);
    }
}