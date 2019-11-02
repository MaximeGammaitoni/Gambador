using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameObjectManager
{
    protected GameObject mainGameObject;
    public BaseGameObjectManager(string gameObjectName)
    {
        mainGameObject = GameObject.Find(gameObjectName);
    }
}
