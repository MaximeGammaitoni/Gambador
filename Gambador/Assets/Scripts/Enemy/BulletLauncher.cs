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
    void Start()
    {
        player = GameObject.Find("Player");
        if(Bullet == null)
        {
            Bullet = Resources.Load("GameObject/Bullet") as GameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= RateOfFire)
        {
            timer += Time.deltaTime * Config.TimeScale;
        }
        else 
        {
            GameObject go = Instantiate(Bullet, transform.position, Quaternion.identity);
            Bullet bulletScript = go.AddComponent<Bullet>();
            bulletScript.speed = speed;
            bulletScript.LifeTime = LifeTime;
            bulletScript.Direction = new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z) - transform.position;
            timer = 0;
        }
    }
}
