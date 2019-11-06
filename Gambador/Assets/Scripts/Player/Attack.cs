using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Damage damage;
    private Properties properties;

    // Start is called before the first frame update
    void Start()
    {
        damage = this.gameObject.GetComponent<Damage>();
        properties = this.gameObject.GetComponent<Properties>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(damage && properties)
                damage.MakeCircular(properties.damage);
        }
    }
}
