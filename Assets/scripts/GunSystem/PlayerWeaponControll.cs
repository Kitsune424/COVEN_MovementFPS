using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class PlayerWeaponControll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] weapons;

    [SerializeField] private float switchTime;
    private int selectedWeapon;
    private float timeSinceSwitch;


    [Header("Actions")]
    public static Action ShootInput;
    public static Action ReloadInput;


    [Header("Keys")]
    [SerializeField] private KeyCode reloadKey;
    [SerializeField] private KeyCode[] switching;


    private void Start()
    {
        SetWeapons();
        Select(selectedWeapon);

        timeSinceSwitch = 0f;
    }

    private void SetWeapons()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }

        if (switching == null) switching = new KeyCode[weapons.Length];
    }

    private void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }
        timeSinceSwitch = 0f;
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ShootInput?.Invoke();
        }

        //Reload
        if (Input.GetKeyDown(reloadKey))
        {
            ReloadInput?.Invoke();
        }

        //Switch
        int perviousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < switching.Length; i++)
        {
            if (Input.GetKeyDown(switching[i]) && timeSinceSwitch >= switchTime)
            {
                selectedWeapon = i;
            }
        }

        if (perviousSelectedWeapon != selectedWeapon)
        {
            Select(selectedWeapon);
        }

        timeSinceSwitch += Time.deltaTime;
    }
}
