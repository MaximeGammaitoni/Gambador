using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AttackManager
{
    private EnemyManager enemyManager;
    private GameObject player;
    private Properties playerProperties;
    private GameObject pulseParticleGO;
    private bool EnnemyInCircle= false;
    private ParticleSystem pulseParticle;
    private Color pulseParticleStartColor;

    public AttackManager()
    {
        enemyManager = GameManager.singleton.EnemyManager;
        player = GameObject.FindGameObjectWithTag("Player");
        pulseParticleGO = GameObject.Find("PulseParticle");
        playerProperties = player.GetComponent<Properties>();
        GameManager.GameUpdate += AttackCheckerUpdate;
        pulseParticleGO.GetComponent<ParticleSystem>().Play();
    }

    void AttackCheckerUpdate()
    {
        Vector3 mousePos = GameManager.singleton.RaycastManager.GetMousePosition(Vector3.zero);
        Collider[] hitColliders = Physics.OverlapSphere(mousePos, Config.AttackRange);
        int i = 0;
        if (hitColliders.Length > 0)
        {
            EnnemyInCircle = false;
            while (i < hitColliders.Length)
            {
                Collider currentCollider = hitColliders[i];

                if (currentCollider.tag == "Enemy")
                {

                    EnnemyInCircle = true;
                }
                i++;
            }
            if (EnnemyInCircle)
            {

                pulseParticleGO.transform.position = mousePos;
            }
            else
            {
                pulseParticleGO.transform.position = Vector3.zero;
            }
        }


    }
    public void AttackEnemy(Vector3 position, string method = "Circular")
    {

        MethodInfo thisMethod = this.GetType().GetMethod("Make" + method, BindingFlags.NonPublic | BindingFlags.Instance);
        thisMethod?.Invoke(this, new object[] { position, playerProperties.damage, Config.AttackRange });
        
    }

    #region Make
    private void MakeCircular(Vector3 position, int damage, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, range);
        int i = 0;
        var songIsPlay = false;
        while (i < hitColliders.Length)
        {
            Collider currentCollider = hitColliders[i];

            if (currentCollider.tag == "Enemy")
            {
                if (!songIsPlay)
                {
                    SFXManager.PlayRandomSlash(SFXManager.PlayerAudioSource);
                    songIsPlay = true;
                }
                    
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

        if (properties.hp <= 0 && !PlayerDeathManager.PlayerIsDead)
        {
            enemyManager.EnemyDeathTrigger(target.GetComponent<Enemy>());
        }
    }

    private void PlayerTake(int damage, Properties properties, Collider target)
    {

    }
    #endregion 
}
