using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private Properties properties;
    private EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        properties = this.gameObject.GetComponent<Properties>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Take(int damage = 1)
    {
        if (properties.hp > 0)
        {
            properties.hp -= damage;
        }else{
            enemyManager.EnemyDeath();
         }
    }

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

    
}
