using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    private float playerHeight;
    private float playerWidth;
    // Start is called before the first frame update
    void Start()
    {
       
        playerHeight = gameObject.GetComponent<MeshRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 50000000, Color.red);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Ground")
                {
                    Vector3 objectHit = new Vector3(hit.point.x, hit.transform.position.y + playerHeight, hit.point.z);
                    transform.position = objectHit; // todoo make a class for camera raycast
                }
            }
        }

    }


}
