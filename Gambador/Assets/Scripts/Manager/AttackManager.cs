using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AttackManager
{
    private EnemyManager enemyManager;
    private GameObject player;
    private Properties playerProperties;

    public AttackManager()
    {
        enemyManager = GameManager.singleton.EnemyManager;
        player = GameObject.FindGameObjectWithTag("Player");
        playerProperties = player.GetComponent<Properties>();
    }

    public void AttackEnemy(Vector3 position, string method = "Circular")
    {

        MethodInfo thisMethod = this.GetType().GetMethod("Make" + method, BindingFlags.NonPublic | BindingFlags.Instance);
        thisMethod?.Invoke(this, new object[] { position, playerProperties.damage, playerProperties.attackRange });
        
    }

    #region Make
    private void MakeCircular(Vector3 position, int damage, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, range);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Collider currentCollider = hitColliders[i];

            if (currentCollider.tag == "Enemy")
            {
                this.TakeAttack(currentCollider, damage);
            }

            i++;
        }
    }
    #endregion 

    #region Take damage 
    public void TakeAttack(Collider target,  int damage = 1)
    {
        Properties properties = target.GetComponent<Properties>();
        properties.hp -= damage;

        switch (target.tag)
        {
            case "Enemy":
                EnemyTake(damage, properties, target);
                break;
            case Config.PlayerTag:
                PlayerTake(damage, properties, target);
                break;
        }
    }

    private void EnemyTake(int damage, Properties properties, Collider target)
    {

        if (properties.hp <= 0)
        {
            enemyManager.EnemyDeathTrigger(target.GetComponent<Enemy>());
        }
    }

    private void PlayerTake(int damage, Properties properties, Collider target)
    {

    }
    #endregion 
}
