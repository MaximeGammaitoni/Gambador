using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager
{
    public delegate void EnemyEventManager();
    public static event EnemyEventManager EnemyDeathEvent;
    public static event EnemyEventManager PopEvent;
    private GameObject enemyParticle;

    public EnemyManager()
    {
        enemyParticle = Resources.Load("GameObject/EnemyParticle") as GameObject;
    }
    public void PopTrigger(Enemy enemy)
    {
        PopEvent?.Invoke();
    }

    public void EnemyDeathTrigger(Enemy enemy)
    {

        GameManager.singleton.StartCoroutine(DeathAnimCoroutine(enemy));
        enemy.GetComponentInChildren<Animator>().enabled = false;
    }
    IEnumerator DeathAnimCoroutine(Enemy enemy)
    {
        if (!enemy.isDead)
        {
            enemy.canAttack = false;
            enemy.isDead = true;
            if(enemy.GetComponent<LookAtPlayer>()!= null)
                enemy.GetComponent<LookAtPlayer>().canLookAt = false;
            var playerPos = GameObject.Find("Player").transform.position;
            var distance = new Vector3(playerPos.x, enemy.gameObject.transform.position.y, playerPos.z) - enemy.gameObject.transform.position;
            Debug.Log(distance);
            Vector3 direction = -distance.normalized * 2.5f;
            var origin = enemy.gameObject.transform.position;
            var destination = origin + direction;
            var speedRatio = 10;
            while (enemy.gameObject.transform.position != destination)
            {
                enemy.transform.position = Vector3.Lerp(enemy.gameObject.transform.position, destination,2f* speedRatio * (Time.deltaTime) * Config.TimeScale);
                speedRatio++;
                yield return 0;
            }
            yield return new WaitForSeconds(0.01f);
            enemy.gameObject.SetActive(false);

            
            var go = GameObject.Instantiate(enemyParticle, enemy.gameObject.transform.position, Quaternion.identity);
            SFXManager.PlaySFX(SFXManager.Explosion, go.GetComponent<AudioSource>());
            go.AddComponent<ParticleStoper>();
            EnemyDeathEvent?.Invoke();
            enemy.setPositionInitialPOsition();
        }
    }

}
