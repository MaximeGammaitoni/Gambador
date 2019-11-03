using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCameraChanger : MonoBehaviour
{
    private Transform previousCameraTarget;

    // Update is called once per frame
    void OnTriggerEnter()
    {
        previousCameraTarget = GameManager.singleton.CameraManager.GetTargetCamera();
        GameManager.singleton.CameraManager.ChangeTargetCamera(gameObject.transform.Find("TargetCamera"));
    }

    private void OnTriggerExit()
    {
        GameManager.singleton.CameraManager.ChangeTargetCamera(previousCameraTarget);
    }
}
