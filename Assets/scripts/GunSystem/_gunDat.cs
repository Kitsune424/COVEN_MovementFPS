using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(fileName = "Guns", menuName = "Weapon/Gun")]
public class _gunData : ScriptableObject
{
    [Header("info")]
    public string WeaponName;

    [Header("ShootCfg")]
    public float damage;
    public float maxDistance;
    public float fireRate;

    [Header("spread")]
    public LayerMask HitMask;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);

    public Vector3 GetSpread(float ShootTime = 0)
    {
        Vector3 ShootDirection = new Vector3(
            Random.Range(-Spread.x, Spread.x),
            Random.Range(-Spread.y, Spread.y),
            Random.Range(-Spread.z, Spread.z));

        ShootDirection.Normalize();
        return ShootDirection;
    }

    [Header("Reloading")]
    public string bulletType;
    public float currentAmmo;
    public float magazinSize;
    public float currentSpareAmmo;
    public float maxSpareAmmo;
    public float reloadTime;
    [HideInInspector]
    public bool reloading;
}
