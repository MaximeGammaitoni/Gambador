using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseGameObjectManager
{
    public static GameObject playerGameObject;
    [HideInInspector] public static Vector3 respawnArea = Vector3.zero;

    public PlayerManager(string gameObjectName) : base(gameObjectName)
    {
        playerGameObject = mainGameObject;
    }
}
