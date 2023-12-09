using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Compilation;
using UnityEngine;

public class GunRealization : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private _gunData _gunData;
    [SerializeField] private new Transform camera;
    float timeSinceLastShoot;

    private void Start()
    {
        PlayerWeaponControll.ShootInput += Shoot;
        PlayerWeaponControll.ReloadInput += StartReload;
    }

    private IEnumerator Reload()
    {
        _gunData.reloading = true;

        yield return new WaitForSeconds(_gunData.reloadTime);
        _gunData.currentSpareAmmo = _gunData.currentSpareAmmo - (_gunData.magazinSize-_gunData.currentAmmo);
        _gunData.currentAmmo = _gunData.magazinSize;
        _gunData.reloading = false;
    }

    private bool canShoot() => !_gunData.reloading && timeSinceLastShoot > 1f / (_gunData.fireRate / 60f);

    public void Shoot()
    {
        if (_gunData.currentAmmo > 0)
        {
            if (canShoot())
            {
                if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hitInfo, _gunData.maxDistance))
                {
                    Debug.Log(hitInfo.transform.name);
                }

                _gunData.currentAmmo--;
                timeSinceLastShoot = 0;
                OnGunShoot();
            }
        }
    }

    private void OnDisable()
    {
        _gunData.reloading = false;
    }

    public void StartReload()
    {
        if (!_gunData.reloading && this.gameObject.activeSelf)
        {
            StartCoroutine(Reload());
            //reload
        }
    }

    private void Update()
    {
        timeSinceLastShoot += Time.deltaTime;
        Debug.DrawRay(camera.position, camera.forward*_gunData.maxDistance, Color.green);
    }

    private void OnGunShoot()
    {
    }
}

