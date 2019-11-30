using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLauncher : MonoBehaviour
{
    private GameObject player;
    public float RateOfFire;
    private float timer;
    public float speed;
    public GameObject Bullet;
    public float LifeTime;
    private Enemy enemy;
    public bool InPlayerDir;
    public float DelayBeforeShoot;
    private float delayTimer = 0;
    public bool ShootIstant;
    void Start()
    {
        enemy = this.GetComponent<Enemy>();
        player = GameObject.Find("Player");
        if (Bullet == null)
        {
            Bullet = Resources.Load("GameObject/Bullet") as GameObject;
        }
        if (ShootIstant)
            timer = RateOfFire;

        PlayerDeathManager.OnPlayerDeath += PlayerDeath;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.canAttack)
        {
            delayTimer += Time.deltaTime * Config.TimeScale;
            if (delayTimer > DelayBeforeShoot)
            {

                if (timer <= RateOfFire)
                {
                    timer += Time.deltaTime * Config.TimeScale;
                }
                else
                {
                    GameObject go = Instantiate(Bullet, transform);
                    go.transform.parent = null;
                    //go.transform.rotation.SetEulerAngles(transform.rotation.eulerAngles * -1);
                    Bullet bulletScript = go.AddComponent<Bullet>();
                    bulletScript.speed = speed;
                    bulletScript.LifeTime = LifeTime;
                    if (InPlayerDir)
                        bulletScript.Direction = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position;
                    else
                        bulletScript.Direction = transform.forward;
                    timer = 0;
                }
            }
        }


    }
    private void PlayerDeath()
    {
        timer = 0;
        delayTimer = 0;
    }
}
