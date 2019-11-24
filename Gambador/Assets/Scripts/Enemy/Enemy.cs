using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Vector3 initialPosition;
    [HideInInspector] public bool canAttack = false;
    [HideInInspector] public bool isDead = false;

    void Start()
    {
        enemyManager = GameManager.singleton.EnemyManager;
        initialPosition = gameObject.transform.position;
    }

    public void OnDeath()
    {
        transform.position = initialPosition;
        canAttack = false;
    }

    private void OnEnable()
    {
        isDead = false;
    }
}
