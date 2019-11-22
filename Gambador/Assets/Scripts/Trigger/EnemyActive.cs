using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActive : MonoBehaviour
{
    public Vector3 addDest;
    public GameObject Door;
    private bool roomIsClean = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Enemy")
            {
                child.transform.gameObject.SetActive(true); // Make sure enemy is active by default
            }
        }
        EnemyManager.EnemyDeathEvent += EnemyDeath;
        PlayerDeathManager.OnPlayerDeath += PlayerDeath;
    }

    // Update is called once per frame
    private void EnemyDeath()
    {
        if (CheckIfEnemiesLeft())
            return;
        roomIsClean = true;
        StartCoroutine(OpenDoor());
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Enemy")
            {
                child.transform.GetComponent<Enemy>().canAttack = true;//Todo replace by fx
            }
        }
    }

    private void PlayerDeath()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Enemy")
            {
                Enemy enemy = child.GetComponent<Enemy>();
                if (! roomIsClean)
                {
                    child.transform.gameObject.SetActive(true);
                    enemy.canAttack = false;
                }

            }
        }
    }

    private bool CheckIfEnemiesLeft()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Enemy" && child.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator OpenDoor()
    {
        var dest = addDest + Door.transform.position;
        float ratioSpeed = 0;
        while (Door.transform.position != dest)
        {
            Door.transform.position = Vector3.Lerp(Door.transform.position, dest, ratioSpeed);
            ratioSpeed += Time.deltaTime * Config.TimeScale;
            yield return 0;
        }
    }
}
