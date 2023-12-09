using System;
using UnityEditor;
using UnityEngine;

public class PlayerWeaponControll : MonoBehaviour
{
    [Header("Actions")]
    public static Action shootInput;
    public static Action reloadingInput;

    [Header("keys")]
    [SerializeField] private KeyCode reloadKey;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            shootInput?.Invoke();
        }

        if (Input.GetKeyDown(reloadKey))
        {
            reloadingInput?.Invoke();
        }
    }
}
