using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapons/Gun")]
public class _WeaponConfiguration : ScriptableObject
{
    [Header("info")]
    public string weaponName;

    [Header("bool's")]
    public bool automaticShoot;
    public bool boltAction;
    public bool canReloadAimed;
    public bool reloading;

    [Header("Firing")]
    public float fireDistance;
    public float idleSpread;
    public float adsSpread;
    public float fireRate;
    public float damage;

    [Header("Magazine")]
    public float currentAmmo;
    public float magSize;
    public float currentSize;
    public float maxAmmo;

    [Header("Reload time")]
    public float EmptyReloadTime;
    public float NoEmptyReloadTime;
}
