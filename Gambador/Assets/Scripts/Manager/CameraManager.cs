using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : BaseGameObjectManager
{
    public CameraManager(string gameObjectName) : base(gameObjectName)
    {
        if (mainGameObject.GetComponent<Camera>() == null)
        {
            Debug.LogError(gameObjectName + " GameObject instantiante in CameraManager is'nt a camera ");
        }
    }

    public Ray RaycastToMousePosition()
    {
        Ray ray = mainGameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        return ray;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
}
