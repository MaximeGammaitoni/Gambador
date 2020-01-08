using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class CameraManager : BaseGameObjectManager
{
    private Transform playerTargetCamera;
    public CameraManager(string gameObjectName) : base(gameObjectName)
    {
        if (mainGameObject.GetComponent<Camera>() == null)
        {
            Debug.LogError(gameObjectName + " GameObject instantiante in CameraManager is'nt a camera ");
        }
        playerTargetCamera = GameObject.Find("TargetPlayerCamera").transform;
        PlayerDeathManager.OnPlayerDeath += ResetTargetCameraOnPlayer;
    }

    public Ray RaycastToMousePosition()
    {
        Ray ray = mainGameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        return ray;
    }
    
    public void ChangeTargetCamera(Transform target)
    {
        mainGameObject.GetComponent<Camera2DFollow>().target = target;
    }

    public Transform GetTargetCamera()
    {
        return mainGameObject.GetComponent<Camera2DFollow>().target;
    }

    public void ResetTargetCameraOnPlayer()
    {
        ChangeTargetCamera(playerTargetCamera);
    }
}
