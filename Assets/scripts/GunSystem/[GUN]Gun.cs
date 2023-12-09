using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TitanfallPlayerMovement;
using Unity.EditorCoroutines.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using InfimaGames.LowPolyShooterPack.Legacy;


public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Target target;
    [SerializeField] PlayerMovement PM;
    [SerializeField] _WeaponConfiguration gunData;
    private float timeSinceLastShoot;
    public Transform shootCamera;
    public Animator animator;
    private bool isADS;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip shotSound;

    
    

    private void Start()
    {
        target = GetComponent<Target>();
        PlayerWeaponControll.shootInput += Shoot;
        PlayerWeaponControll.reloadingInput += StartReload;
    }

    private void Update()
    {
        timeSinceLastShoot += Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("ADSin");
            isADS = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetTrigger("ADSout");
            isADS = false;
        }
    }

    #region Logic Bool
    private bool CanShoot() => !gunData.reloading && timeSinceLastShoot > 1f / (gunData.fireRate / 60f);
    private IEnumerator Reload()
    {
        gunData.reloading = true;

        if (gunData.currentAmmo == 0)
        {
            yield return new EditorWaitForSeconds(gunData.EmptyReloadTime);

        }
        else
        {
            yield return new EditorWaitForSeconds(gunData.NoEmptyReloadTime);
        }

        gunData.currentSize = (gunData.magSize - gunData.currentAmmo);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
    }
    #endregion

    #region Shoot Logic
    private void StartReload()
    {
        if (!gunData.reloading)
        {
            StartCoroutine(Reload());
            //reload
        }
    }

    private void Shoot()
    {
        float currentIDLESpread = (gunData.adsSpread * (1+PM.rb.velocity.magnitude)*2.5f);
        Vector3 shootDirection = shootCamera.transform.forward;
        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                audioSource.GetComponent<AudioSource>().clip = shotSound;
                audioSource.Play();
                //spreadSystem
                if (isADS == true)
                {
                    shootDirection = shootDirection + shootCamera.TransformDirection
                        (new Vector3(Random.Range(-gunData.adsSpread, gunData.adsSpread), Random.Range(-gunData.adsSpread, -gunData.adsSpread)));
                }
                else
                {
                    shootDirection = shootDirection + shootCamera.TransformDirection
                        (new Vector3(Random.Range(-currentIDLESpread, currentIDLESpread), Random.Range(-currentIDLESpread, currentIDLESpread)));
                }
                Debug.DrawRay(shootCamera.position, shootDirection * gunData.fireDistance, Color.green);

                RaycastHit hit;
                if (Physics.Raycast(shootCamera.position, shootDirection * gunData.fireDistance, out hit, gunData.fireDistance))
                {
                    TargetScript target = hit.transform.GetComponent<TargetScript>();

                    if (target != null)
                    {
                        target.isHit = true;
                    }
                }

                gunData.currentAmmo--;
                timeSinceLastShoot = 0;
            }
        }
    }

    #endregion
}
