using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunData : MonoBehaviour
{
    [Header("Gun characteristic")]
    public float damage = 43f;
    public float range = 100f;

    [Header("Misc")]
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            TARGET target = hit.transform.GetComponent<TARGET>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
