using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitanfallPlayerMovement
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Camera setup")]
        [Range(-90f, 90f)] public float MinVerticalAngle = -90f;
        [Range(-90f, 90f)] public float MaxVerticalAngle = 90f;
        [Range(60f, 110f)] public float baseFov = 90f;
        private Quaternion camCenter;

        [SerializeField] public Transform Camera;
        [SerializeField] public Transform orientation;
        [SerializeField] public Rigidbody rb;


        [Header("Camera rotation")]
        [Range(0.01f, 10f)] public float xSensetivity = 1;
        [Range(0.01f, 10f)] public float ySensetivity = 1;
        public float SensetivityMultiplayer;
        public bool InvertX = false;
        public bool yxSens = true;

        private float xMouseAxis;
        private float yMouseAxis;
        private float camerarotationX;
        private float camerarotationY;





        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
        }

        private void Update()
        {
            InputSetup();

            //Max X angle rotation
            camerarotationX = Mathf.Clamp(camerarotationX, MinVerticalAngle, MaxVerticalAngle);
            Camera.transform.rotation = Quaternion.Euler(camerarotationX, camerarotationY, 0);
            orientation.transform.rotation = Quaternion.Euler(0, camerarotationY, 0);
            rb.transform.rotation = orientation.transform.rotation;
        }

        private void FixedUpdate()
        {
            
        }
        private void InputSetup()
        {
            xMouseAxis = Input.GetAxisRaw("Mouse X");
            yMouseAxis = Input.GetAxisRaw("Mouse Y");

            //sesetivity setup
            if (yxSens)
            {
                ySensetivity = xSensetivity;
            }
            else if (!yxSens)
            {
                                                            #pragma warning disable CS1717 // Назначение выполнено для той же переменной
                ySensetivity = ySensetivity;
                                                            #pragma warning restore CS1717 // Назначение выполнено для той же переменной

                                                            #pragma warning disable CS1717 // Назначение выполнено для той же переменной
                xSensetivity = xSensetivity;
                                                            #pragma warning restore CS1717 // Назначение выполнено для той же переменной
            }


            //Camera rotatoin and invert asxes system
            if (!InvertX)
            {
                camerarotationX -= yMouseAxis * xSensetivity * SensetivityMultiplayer;
                camerarotationY += xMouseAxis * ySensetivity * SensetivityMultiplayer;
            }
            else if (InvertX)
            {
                camerarotationX += yMouseAxis * xSensetivity * SensetivityMultiplayer;
                camerarotationY += xMouseAxis * ySensetivity * SensetivityMultiplayer;
            }
        }
    }
}

