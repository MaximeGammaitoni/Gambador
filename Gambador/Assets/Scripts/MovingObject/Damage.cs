using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private Properties properties;
    private EnemyManager enemyManager;
    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        properties = this.gameObject.GetComponent<Properties>();
        enemy = this.gameObject.GetComponent<Enemy>();
        enemyManager = new EnemyManager();
    }

    #region Make damage 
    public void MakeCircular(int damage = 1)
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, properties.attackRange);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Collider currentCollider = hitColliders[i];

            if (currentCollider.tag == "enemy")
            {
                
                currentCollider.GetComponent<Damage>().Take(damage);
            }
            
            i++;
        }
    }
    #endregion 

    #region Take damage 
    private void Take(int damage = 1)
    {
        properties.hp -= damage;

        switch (this.tag)
        {
            case "enemy":
                EnemyTake(damage);
                break;
            case "Player":
                PlayerTake(damage);
                break;
        }
    }

    private void EnemyTake(int damage)
    {
        if (properties.hp == 0)
        {
            enemyManager.EnemyDeath(enemy);
        }
    }

    private void PlayerTake(int damage)
    {

    }

    #endregion

}
