using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedradaLifetime : MonoBehaviour
{

    public int damage; // Amount of damage the fireball does
    private float age;
    public float maxLifeTime;
    public string owner = null;

    void Start()
    {
        age = 0f;
    }

    void Update()
    {
        age += Time.deltaTime;
        if (age > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }
}
