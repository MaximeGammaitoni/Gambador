using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 mousePos = GetMousePosition();
        RaycastHit hit;
        Vector3 fromPosition = transform.position;
        Vector3 toPosition = mousePos;
        Vector3 direction = toPosition - fromPosition;
        Debug.DrawRay(fromPosition, direction, Color.red);
        if (Physics.Raycast(fromPosition, direction, out hit))
        {
            if(hit.transform.name== "Obstacle")
            {
               
                lr.SetColors(Color.red, Color.red);
                Debug.Log(hit.transform.name);
            }

        }
        else
        {
            lr.SetColors(Color.green, Color.green);
        }
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, mousePos);

    }

    Vector3 GetMousePosition()
    {
        RaycastHit hit;
        Ray ray = GameManager.singleton.cameraManager.RaycastToMousePosition();
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Ground")
            {
                Vector3 objectHit = new Vector3(hit.point.x, hit.point.y+0.3f, hit.point.z);
                return  objectHit;
            }
        }

        return transform.position;
    }
}
