using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CharacterMovementSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera setup")]
        [Range(-90f, 90f)] public float MinVerticalAngle = -90f;
        [Range(-90f, 90f)] public float MaxVerticalAngle = 90f;

        [SerializeField] public Transform Camera;
        [SerializeField] public Transform orientation;
        private Quaternion camCenter;

        [Header("CameraController Misc")]
        public bool InvertX = false;
        public bool yxSens = true;

        [Header("Camera rotatoin")]
        [Range(0,10)]public float xSensetivity = 1;
        [Range(0,10)]public float ySensetivity = 1;
        public float SensetivityMultiplayer;
        public Rigidbody rb;

        private float xMouseAxis;
        private float yMouseAxis;
        private float camerarotationX;
        private float camerarotationY;


        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            camCenter = Camera.localRotation;
        }

        // Update is called once per frame
        private void Update()
        {
            InputSetup();

            //Max X angle rotation
            camerarotationX = Mathf.Clamp(camerarotationX, MinVerticalAngle, MaxVerticalAngle);
            Camera.transform.rotation = Quaternion.Euler(camerarotationX, camerarotationY, 0);
            orientation.transform.rotation = Quaternion.Euler(0, camerarotationY, 0);
            rb.transform.rotation = orientation.transform.rotation;


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
                                                #pragma warning disable CS1717 // Ќазначение выполнено дл€ той же переменной
                ySensetivity = ySensetivity;
                                                #pragma warning restore CS1717 // Ќазначение выполнено дл€ той же переменной

                                                #pragma warning disable CS1717 // Ќазначение выполнено дл€ той же переменной
                xSensetivity = xSensetivity;
                                                #pragma warning restore CS1717 // Ќазначение выполнено дл€ той же переменной
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

