using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager
{
    public List<string> WalkablesTags;
    public List<string> Obstaclestags;
    private GameObject player;
    private LineRenderer lr;
    private float playerHeight;
    private float lastYFromPos;
    public RaycastManager()
    {
        WalkablesTags = new List<string>();
        Obstaclestags = new List<string>();
        WalkablesTags.Add("Ground");
        Obstaclestags.Add("Obstacle");
        player = GameObject.Find("Player").gameObject;
        lr = player.transform.Find("Laser").GetComponent<LineRenderer>();
        playerHeight = player.GetComponent<MeshRenderer>().bounds.size.y;
        GameManager.GameUpdate += MovingRaycastWithLineRenderer;

    }
    public Vector3 GetMousePosition(Vector3 initialPosition)
    {
        RaycastHit hit;
        Ray ray = GameManager.singleton.CameraManager.RaycastToMousePosition();
        if (Physics.Raycast(ray, out hit))
        {
            if (WalkablesTags.Contains(hit.transform.tag))//can move
            {
                Vector3 objectHit = new Vector3(hit.point.x, hit.point.y + 0.3f, hit.point.z);
                return objectHit;
            }
        }
        return initialPosition;
    }

    public void MovingRaycastWithLineRenderer()
    {
        Vector3 mousePos = GameManager.singleton.RaycastManager.GetMousePosition(player.transform.position);
        RaycastHit hit;
        Vector3 fromPosition = player.transform.position;
        Vector3 toPosition = mousePos;
        Vector3 direction = toPosition - fromPosition;
        float distance = Vector3.Distance(toPosition, fromPosition);
        float radius = 7.45f;
        if (distance > radius) //If the distance is less than the radius, it is already within the circle.
        {

            Vector3 fromOriginToObject = toPosition - fromPosition; //~GreenPosition~ - *BlackCenter*
            fromOriginToObject *= radius / distance; //Multiply by radius //Divide by Distance
            mousePos = fromPosition + fromOriginToObject; //*BlackCenter* + all that Math
            if (Physics.Raycast(mousePos, Vector3.down, out hit, 1000))
                mousePos.y = hit.transform.position.y+playerHeight;
        }


        lr.SetPosition(0, player.transform.position);
        lr.SetPosition(1, mousePos);
        if (Physics.Raycast(fromPosition, direction, out hit, distance))
        {
            if (Obstaclestags.Contains(hit.transform.tag)) // there is obstacles in distance beetwen player and mouse pos
            {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
            }
            else
            {
                if (Input.GetMouseButtonDown(0)) // not an obstacle in trajectory, player can move
                {
                    Vector3 objectHit = new Vector3(mousePos.x, mousePos.y, mousePos.z);
                    GameManager.singleton.MovingPlayerManager.StartMovingPlayer(objectHit);
                }
            }
        }
        else
        {

            lr.startColor = Color.green;
            lr.endColor = Color.green;

            if (Input.GetMouseButtonDown(0)) // not an obstacle in trajectory, player can move
            {

                Vector3 objectHit = new Vector3(mousePos.x, mousePos.y, mousePos.z);
                GameManager.singleton.MovingPlayerManager.StartMovingPlayer(objectHit);



            }

        }

    }

    private bool PointInsideSphere(Vector3 point, Vector3 center, float radius)
    {
        return Vector3.Distance(point, center) < radius;
    }
    public bool checkPoint(int radius, int x, int y,
                  float percent, float startAngle)
    {

        // calculate endAngle 
        float endAngle = 360 / percent + startAngle;

        // Calculate polar co-ordinates 
        float polarradius =
                    (float)Math.Sqrt(x * x + y * y);

        float Angle = (float)Math.Atan(y / x);

        // Check whether polarradius is less then  
        // radius of circle or not and Angle is  
        // between startAngle and endAngle or not 
        if (Angle >= startAngle && Angle <= endAngle
                            && polarradius < radius)
            return true;
        else
            return false;
    }

}
