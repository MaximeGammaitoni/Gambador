using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "enemy")
            {
                child.transform.gameObject.SetActive(true);
            }
        }
    }
}
