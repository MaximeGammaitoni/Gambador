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
    }
    IEnumerator DeathAnimCoroutine(Enemy enemy)
    {
        enemy.GetComponent<LookAtPlayer>().canLookAt = false;
        var playerPos = GameObject.Find("Player").transform.position;
        var distance = new Vector3(playerPos.x, enemy.gameObject.transform.position.y, playerPos.z) - enemy.gameObject.transform.position;
        Debug.Log(distance);
        Vector3 direction = - distance.normalized*3;
        var origin = enemy.gameObject.transform.position;
        var destination = origin + direction;
        var speedRatio = 10;
        while (enemy.gameObject.transform.position != destination)
        {
            Debug.Log(enemy.transform.position); 
            enemy.transform.position = Vector3.Lerp(enemy.gameObject.transform.position, destination,speedRatio * Time.deltaTime * Config.TimeScale);
            speedRatio++;
            yield return 0;
        }
        yield return new WaitForSeconds(0.2f);
        enemy.gameObject.SetActive(false);

        EnemyDeathEvent?.Invoke();
        var go = GameObject.Instantiate(enemyParticle, enemy.gameObject.transform.position, Quaternion.identity);
        go.AddComponent<ParticleStoper>();
        enemy.OnDeath();

    }

}
