using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Vector3 Direction;
    public float LifeTime;
    private float timer = 0;
    void Update()
    {
        timer += Time.deltaTime * Config.TimeScale;
        transform.Translate(-transform.forward * (speed * Time.deltaTime) * Config.TimeScale);
        //transform.rotation.SetEulerAngles(Direction.normalized * (speed * Time.deltaTime) * Config.TimeScale);
        if (timer>= LifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == Config.PlayerTag)
        {
            GameManager.singleton.PlayerDeathManager.PlayerDeathTrigger();
        }
        else if (GameManager.singleton.RaycastManager.Obstaclestags.Contains(col.tag) || GameManager.singleton.RaycastManager.WalkablesTags.Contains(col.tag))
        {
            Destroy(gameObject);
        }
    }
}
