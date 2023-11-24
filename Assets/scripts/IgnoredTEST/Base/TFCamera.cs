using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitanfallPlayerMovement
{
    public class CameraController : MonoBehaviour
    {
        public Camera mainCamera;
        float sensX = 1f;
        float sensY = 1f;
        float baseFov = 90f;
        float maxFov = 140f;
        float wallRunTilt = 15f;

        float wishTilt = 0;
        float curTilt = 0;
        Vector2 currentLook;
        Vector2 sway = Vector3.zero;
        float fov;
        Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            curTilt = transform.localEulerAngles.z;
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            float addedFov = rb.velocity.magnitude - 3.44f;
            fov = Mathf.Lerp(fov, baseFov + addedFov, 0.5f);
            fov = Mathf.Clamp(fov, baseFov, maxFov);
            mainCamera.fieldOfView = fov;

            currentLook = Vector2.Lerp(currentLook, currentLook + sway, 0.8f);
            curTilt = Mathf.LerpAngle(curTilt, wishTilt * wallRunTilt, 0.05f);

            sway = Vector2.Lerp(sway, Vector2.zero, 0.2f);
        }

        public void Punch(Vector2 dir)
        {
            sway += dir;
        }

        #region Setters
        public void SetTilt(float newVal)
        {
            wishTilt = newVal;
        }

        public void SetXSens(float newVal)
        {
            sensX = newVal;
        }

        public void SetYSens(float newVal)
        {
            sensY = newVal;
        }

        public void SetFov(float newVal)
        {
            baseFov = newVal;
        }
        #endregion
    }
}

