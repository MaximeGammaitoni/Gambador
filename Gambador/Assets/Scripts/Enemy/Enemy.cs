using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager enemyManager;
    [HideInInspector] public bool canAttack = false;
    [HideInInspector] public bool canSpawn = true;

    void Start()
    {
        enemyManager = GameManager.singleton.EnemyManager;
    }

    void OnEnable()
    {
        enemyManager?.PopTrigger(this);
    }

    private void OnDisable()
    {
        Debug.Log("Restart position ?");
    }
}
