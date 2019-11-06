using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [HideInInspector] public delegate void EnemyEventManager();
    [HideInInspector] public static event EnemyEventManager EnemyDeathEvent;
    [HideInInspector] public static event EnemyEventManager PopEvent;

    private Properties properties;

    // Start is called before the first frame update
    void Start()
    {
        properties = this.gameObject.GetComponent<Properties>();
    }

    // Update is called once per frame
    void Update()
    {
        if (properties.hp <= 0)
            EnemyDeath();
    }

    void OnEnable()
    {
        OnPop();
    }

    private void OnPop()
    {
        Debug.Log("I'm pop" + this.name);
        PopEvent?.Invoke();
    }

    private void EnemyDeath()
    {
        Debug.Log(this.name + "is dead");
        EnemyDeathEvent?.Invoke();
    }

}
