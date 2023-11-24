using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace CharacterMovementSystem
{
    public class CharacterMovement : MonoBehaviour
    {
        public enum CharacterState
        {
            walking,
            sprinting,
            crouching,
            air
        }

        [Header("Inputs")]
        private bool keyJumpDown, keyjumpUp; //Jump
        private bool keySprintDown; //Run
        private bool keyCrouchDown, keyCrouchUp; //Crouching
        private bool keySliding;

        [Header("Movement setup")]
        private float horizontalMovement;
        private float verticalMovement;
        public float CurrentVelocity;
        public Vector3 moveDirection;
        private bool exitingSlope;

        public float movementMultiplayer = 10f;
        public float airMultiplayer = 2f;
        public float airDrag = 1f;
        public float Drag = 5f;
        [SerializeField] public Transform orientation;
        [Range(1f, 89f)] public float maxslopeAngle = 55f;

        [Header("Speed's")]
        public float walkingSpeed = 10f;
        public float sprintingSpeed = 15f;
        public float crouchingSpeed = 3f;

        [Header("Jumping")]
        public float jumpForce = 5f;
        public float dashSpeed = 10f;

        private bool canDoubleJump = false;
        private bool canJump = false;

        [Header("Crouching")]
        private float crounchYscale = 0.5f;
        private float startcrouchYscale;
        private bool IsCrouching = false;

        [Header("Sliding")]
        public float maxSlideTime;
        public float slideForce;
        private float slideTimer;

        public float slideYscale;


        [Header("Wall running")]

        [Header("Ground and Wall Check")]
        public bool isGrounded;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;
        public LayerMask wallMask;
        private RaycastHit slopeHit;

        [Header("Misc")]
        private float playerHeigh = 2f;
        public CameraController CameraController;
        public Rigidbody rb;
        public CapsuleCollider CapsuleCollider;
        public CharacterState state;
        internal CollisionFlags collisionFlags;

        //base physic
        #region All about physic
        // Start is called before the first frame update
        private void Start()
        {
            canJump = true;

            rb = GetComponent<Rigidbody>();
            CameraController = rb.GetComponent<CameraController>();
            CapsuleCollider = rb.GetComponent<CapsuleCollider>();
            rb.freezeRotation = true;

            startcrouchYscale = transform.localScale.y;
        }

        // Update is called once per frame
        private void Update()
        {
            rb.useGravity = !OnSlope();
            isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundMask);
            CapsuleCollider.material.dynamicFriction = 0f;

            InputSetup();
            StateHandler();

            //jump
            if (keyJumpDown && isGrounded) { Jump(); }
            else if (keyJumpDown && !isGrounded) { DoubleJump(moveDirection); }

            //crouch
            if (keyCrouchDown) { Crouching(); }
            else if (keyCrouchUp) { StopCrouching(); }
        }

        //physic correct render
        private void FixedUpdate()
        {
            DragControl();
            Movement();
        }

        private void InputSetup()
        {
            //Move X Y
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");

            moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

            //jump
            keyJumpDown = Input.GetKeyDown(KeyCode.Space);
            keyjumpUp = Input.GetKeyUp(KeyCode.Space);

            //sprint
            keySprintDown = Input.GetKey(KeyCode.LeftShift);

            //crouching
            keyCrouchDown = Input.GetKeyDown(KeyCode.LeftControl);
            keyCrouchUp = Input.GetKeyUp(KeyCode.LeftControl);

            keySliding = Input.GetKey(KeyCode.C);
        }


        public bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeigh / 2 * 0.5f + 0.3f))
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return slopeAngle < maxslopeAngle && slopeAngle != 0;
            }
            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        }
        
        #endregion

        //Movement system
        #region Movement, Jump, Drag, Crouching, sliding

        /// <summary>
        /// основной компонент отвечающий за перемещение персонажа в пространстве за счет сложения сил
        /// </summary>
        private void Movement()
        {
            if (isGrounded && !OnSlope())
            {
                rb.AddForce(moveDirection.normalized * CurrentVelocity * movementMultiplayer, ForceMode.Acceleration);
            }
            else if (isGrounded && OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection(moveDirection.normalized) * CurrentVelocity * movementMultiplayer, ForceMode.Acceleration);
                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else if (!isGrounded)
            {
                rb.AddForce(moveDirection.normalized * CurrentVelocity * airMultiplayer, ForceMode.Acceleration);
            }
        }

        /// <summary>
        /// Контроль трения с поверхностью
        /// </summary>
        private void DragControl()
        {
            if (isGrounded) { rb.drag = Drag; }
            if (!isGrounded) { rb.drag = airDrag; }
        }

        /// <summary>
        /// прыжок
        /// </summary>
        private void Jump()
        {
            exitingSlope = true;
            if (canJump)
            {
                canDoubleJump = true;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Двойной прыжок 
        /// </summary>
        /// <param name="wishDir"></param>
        private void DoubleJump(Vector3 wishDir)
        {
            exitingSlope = true;
            //vertical
            if (canDoubleJump)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * (jumpForce * 0.7f), ForceMode.Impulse);
                canDoubleJump = false;
            }

            //horizontal
            if (wishDir != Vector3.zero)
            {
                Vector3 horSpid = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                Vector3 newSpid = wishDir.normalized;
                float newSpidMagnitude = dashSpeed;

                if (horSpid.magnitude > dashSpeed)
                {
                    float dot = Vector3.Dot(wishDir.normalized, horSpid.normalized);
                    if (dot > 0)
                    {
                        newSpidMagnitude = dashSpeed + (horSpid.magnitude - dashSpeed) * dot;
                    }
                    else
                    {
                        newSpidMagnitude = Mathf.Clamp(dashSpeed * (1 + dot), dashSpeed * (dashSpeed / horSpid.magnitude), dashSpeed);
                    }
                }

                newSpid *= newSpidMagnitude;

                rb.AddForce(newSpid - horSpid, ForceMode.VelocityChange);
            }
        }

        private void Crouching()
        {
            transform.localScale = new Vector3(transform.localScale.x, crounchYscale, transform.localScale.z);
            IsCrouching = true;
        }

        private void StopCrouching()
        {
            transform.localScale = new Vector3(transform.localScale.x, startcrouchYscale, transform.localScale.z);
            IsCrouching = false;
        }

        #endregion

        //Debug (Здесь будут все методы, отвечающие за вывод данных касательно движения)
        #region Debug system, if u need see something
        void OnGUI()
        {
            GUILayout.Label("Spid: " + Math.Round(rb.velocity.magnitude, 0));
        }
        #endregion

        //state handler
        #region State functions
        private void StateHandler()
        {
            if (isGrounded && keySprintDown)
            {
                state = CharacterState.sprinting;
                CurrentVelocity = sprintingSpeed;
            }
            else if (isGrounded && IsCrouching)
            {
                state = CharacterState.crouching;
                CurrentVelocity = crouchingSpeed;
            }
            else if (isGrounded)
            {
                state = CharacterState.walking;
                CurrentVelocity = walkingSpeed;
            }
           
            else if (!isGrounded)
            {
                state = CharacterState.air;
                CurrentVelocity = walkingSpeed;
            }
        }
        #endregion

        
        
    }
}

