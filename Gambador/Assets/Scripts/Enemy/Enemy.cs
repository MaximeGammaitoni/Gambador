using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Vector3 initialPosition;
    [HideInInspector] public bool canAttack = false;
    private Vector3 initialPosition;
    void Start()
    {
        enemyManager = GameManager.singleton.EnemyManager;
        initialPosition = gameObject.transform.position;
    }

    public void OnDeath()
    {
        transform.position = initialPosition;
    }

    private void OnDisable()
    {
        
    }
}
