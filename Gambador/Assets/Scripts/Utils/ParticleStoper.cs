using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStoper : MonoBehaviour
{
    ParticleSystem particleSystem;
    float timeToStop = 0.5f;
    float timer = 0;
    float timeToDestroy = 2;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * Config.TimeScale;
        if(timer >= timeToDestroy)
        {
            Destroy(gameObject);
        }
        else if (timer >= timeToStop)
        {
            particleSystem.Stop();
        }
    }
}
