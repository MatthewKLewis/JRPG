using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sLifeTimer : MonoBehaviour
{
    private float timeInstantiated;
    private float lifespan = 1f;

    void Start()
    {
        timeInstantiated = Time.time;
    }

    void Update()
    {
        if (Time.time > timeInstantiated + lifespan)
        {
            Destroy(this.gameObject);
        }
    }
}
