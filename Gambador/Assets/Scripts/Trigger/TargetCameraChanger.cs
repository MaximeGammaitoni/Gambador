using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCameraChanger : MonoBehaviour
{
    private Transform previousCameraTarget;

    // Update is called once per frame
    void OnTriggerEnter()
    {
        previousCameraTarget = GameManager.singleton.cameraManager.GetTargetCamera();
        GameManager.singleton.cameraManager.ChangeTargetCamera(gameObject.transform.Find("TargetCamera"));
    }

    private void OnTriggerExit()
    {
        GameManager.singleton.cameraManager.ChangeTargetCamera(previousCameraTarget);
    }
}
