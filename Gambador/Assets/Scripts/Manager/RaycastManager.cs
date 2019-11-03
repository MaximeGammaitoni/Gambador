using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager
{
    public List<string> WalkablesTags;
    public List<string> Obstaclestags;
    public RaycastManager()
    {
        WalkablesTags = new List<string>();
        Obstaclestags = new List<string>();
        WalkablesTags.Add("Ground");
        Obstaclestags.Add("Obstacle");
    }
    public Vector3 GetMousePosition(Vector3 initialPosition)
    {
        RaycastHit hit;
        Ray ray = GameManager.singleton.CameraManager.RaycastToMousePosition();
        if (Physics.Raycast(ray, out hit))
        {
            if ( WalkablesTags.Contains(hit.transform.tag))//can move
            {
                Vector3 objectHit = new Vector3(hit.point.x, hit.point.y + 0.3f, hit.point.z);
                return objectHit;
            }

        }

        return initialPosition;
    }

    public void MovingRaycastWithLineRenderer(Transform objectToMoveTransform, LineRenderer lr, float objectToMoveHeight)
    {
        Vector3 mousePos = GameManager.singleton.RaycastManager.GetMousePosition(objectToMoveTransform.position);
        RaycastHit hit;
        Vector3 fromPosition = objectToMoveTransform.position;
        Vector3 toPosition = mousePos;
        Vector3 direction = toPosition - fromPosition;
        float distance = Vector3.Distance(toPosition, fromPosition);
        if (Physics.Raycast(fromPosition, direction, out hit, distance))
        {
            if (Obstaclestags.Contains(hit.transform.tag)) // there is obstacles in distance beetwen player and mouse pos
            {
                lr.startColor =Color.red;
                lr.endColor = Color.red;
            }
            else{
                if (Input.GetMouseButtonDown(0)) // not an obstacle in trajectory, player can move
                {
                    Vector3 objectHit = new Vector3(mousePos.x, mousePos.y , mousePos.z);
                    objectToMoveTransform.position = objectHit;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))//nothing in trajectory, player can move
            {
                Vector3 objectHit = new Vector3(mousePos.x, mousePos.y , mousePos.z);
                objectToMoveTransform.position = objectHit;
            }

            lr.startColor = Color.green;
            lr.endColor = Color.green;
        }
        lr.SetPosition(0, objectToMoveTransform.position);
        lr.SetPosition(1, mousePos);
    }
}
