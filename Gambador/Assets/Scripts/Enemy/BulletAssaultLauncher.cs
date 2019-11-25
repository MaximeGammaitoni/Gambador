using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAssaultLauncher : MonoBehaviour
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
    public int RafaleLength;
    private bool rafaleLaunched = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.canAttack && !rafaleLaunched)
        {
            delayTimer += Time.deltaTime * Config.TimeScale;
            if (delayTimer > DelayBeforeShoot)
            {

                if (timer <= RateOfFire)
                {
                    Debug.Log("Fire");
                    timer += Time.deltaTime * Config.TimeScale;
                }
                else
                {
                    StartCoroutine(StartRafale());
                    timer = 0;
                }
            }
        }
    }
    IEnumerator StartRafale()
    {
        rafaleLaunched = true;
         int rafaleNumber = 0;
        while (rafaleNumber <= RafaleLength)
        {
            rafaleNumber++;
            GameObject go = Instantiate(Bullet, transform.position, Quaternion.identity);
            Bullet bulletScript = go.AddComponent<Bullet>();
            bulletScript.speed = speed;
            bulletScript.LifeTime = LifeTime;
            if (InPlayerDir)
                bulletScript.Direction = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position;
            else
                bulletScript.Direction = transform.forward;
            yield return new WaitForSeconds(0.2f);
        }
        rafaleLaunched = false;

    }
}
