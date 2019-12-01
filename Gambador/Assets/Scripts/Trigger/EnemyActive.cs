using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActive : MonoBehaviour
{
    public Vector3 addDest;
    public GameObject Door;
    public bool roomIsClean = false;

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
       
        EnemyManager.EnemyDeathEvent -= EnemyDeath;
        roomIsClean = true;
        StartCoroutine(OpenDoor());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Enemy")
                {
                    child.transform.GetComponent<Enemy>().canAttack = true;
                    if (child.GetComponent<BulletAssaultLauncher>() != null)
                    {
                        child.GetComponent<BulletAssaultLauncher>().rafaleLaunched = false;
                    }
                }
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Enemy")
                {
                    child.transform.GetComponent<Enemy>().canAttack = false;
                }
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
                enemy.canAttack = false;
                if (! roomIsClean)
                {
                    child.transform.gameObject.SetActive(true);
                    child.GetComponentInChildren<Animator>().enabled = true;
                }

            }
        }
        foreach (Bullet bullet in GameObject.FindObjectsOfType<Bullet>())
        {
            Destroy(bullet.transform.gameObject);
        }
    }

    private bool CheckIfEnemiesLeft()
    {
        foreach (Transform child in transform)
        {
            var test = transform.name;
            if (child.tag == "Enemy" && child.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        
        return false;
    }

    IEnumerator OpenDoor()
    {
        if(Door != null)
        {
            SFXManager.PlaySFX(SFXManager.OpenDoor, SFXManager.PlayerAudioSource);
            var dest = addDest + Door.transform.position;
            float ratioSpeed = 0;

            while (Door.transform.position != dest)
            {
                Door.transform.position = Vector3.Lerp(Door.transform.position, dest, ratioSpeed);
                ratioSpeed += Time.deltaTime * Config.TimeScale;
                yield return 0;
            }
            GameManager.singleton.CameraManager.ResetTargetCameraOnPlayer();
        }  
    }
}
