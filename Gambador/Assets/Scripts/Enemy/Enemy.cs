using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager enemeyManager;
    void Start()
    {
        enemeyManager = new EnemyManager();
    }

    void OnEnable()
    {

        enemeyManager?.OnPop(this);
    }
}
