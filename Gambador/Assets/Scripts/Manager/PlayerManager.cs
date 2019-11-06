using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseGameObjectManager
{
    public static GameObject playerGameObject;
    public PlayerManager(string gameObjectName) : base(gameObjectName)
    {
        playerGameObject = mainGameObject;
    }
}
