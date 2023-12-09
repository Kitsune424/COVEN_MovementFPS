using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public GameObject Damagetarget;
    
    public void TakeDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            Destroy(Damagetarget);
        }
    }
}

   
