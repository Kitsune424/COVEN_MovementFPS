using CharacterMovementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitanfallPlayerMovement
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float groundSpeed = 4f;
        public float runSpeed = 6f;
        public float grAccel = 20f;
        //Jump
        public float jumpUpSpeed = 9.2f;
        float dashSpeed = 6f;
        //crouch
        private float crounchYscale = 0.5f;
        private float startcrouchYscale;

        [Header("Air")]
        public float airSpeed = 3f;
        public float airAccel = 20f;

        [Header("Wallrunning")]
        public float wallSpeed = 10f;
        public float wallClimbSpeed = 4f;
        public float wallAccel = 20f;
        public float wallRunTime = 3f;
        public float wallStickiness = 20f;
        public float wallStickDistance = 1f;
        public float wallFloorBarrier = 40f;
        public float wallBanTime = 4f;
        Vector3 bannedGroundNormal;

        private bool canJump = true;
        private bool canDJump = true;
        private float wallBan = 0f;
        private float wrTimer = 0f;
        private float wallStickTimer = 0f;

        
        [Header("Misc")]
        private bool running;
        private bool jump;
        private bool crouched;
        private bool grounded;

        public Collider ground;
        public Vector3 groundNormal = Vector3.up;
        public CapsuleCollider col;

        public Rigidbody rb;
        public Vector3 dir = Vector3.zero;

        enum Mode
        {
            Walking,
            Flying,
            Wallruning
        }
        Mode mode = Mode.Flying;


        void Start()
        {
            rb = GetComponent<Rigidbody>();
            col = rb.GetComponent<CapsuleCollider>();
            rb.freezeRotation = true;

            startcrouchYscale = transform.localScale.y;
        }

        void OnGUI()
        {
            GUILayout.Label("Spid: " + new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
            GUILayout.Label("SpidUp: " + rb.velocity.y);
        }

        void Update()
        {
            col.material.dynamicFriction = 0f;
            dir = Direction();

            running = (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.9);
            crouched = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jump = true;
            }

            //Special use
            //if (Input.GetKeyDown(KeyCode.T)) transform.position = new Vector3(0f, 30f, 0f);
            //if (Input.GetKeyDown(KeyCode.X)) rb.velocity = new Vector3(rb.velocity.x, 40f, rb.velocity.z);
            //if (Input.GetKeyDown(KeyCode.V)) rb.AddForce(dir * 20f, ForceMode.VelocityChange);
        }

        void FixedUpdate()
        {
            if (crouched)
            {
                col.height = Mathf.Max(0.5f, col.height - Time.deltaTime * 10f);
                transform.localScale = new Vector3(transform.localScale.x, crounchYscale, transform.localScale.z);
            }
            else
            {
                col.height = Mathf.Min(2f, col.height + Time.deltaTime * 10f);
                transform.localScale = new Vector3(transform.localScale.x, startcrouchYscale, transform.localScale.z);
            }

            if (wallStickTimer == 0f && wallBan > 0f)
            {
                bannedGroundNormal = groundNormal;
            }
            else
            {
                bannedGroundNormal = Vector3.zero;
            }

            wallStickTimer = Mathf.Max(wallStickTimer - Time.deltaTime, 0f);
            wallBan = Mathf.Max(wallBan - Time.deltaTime, 0f);

            switch (mode)
            {
                case Mode.Wallruning:
                    Wallrun(dir, wallSpeed, wallClimbSpeed, wallAccel);
                    if (ground.tag != "InfiniteWallrun") wrTimer = Mathf.Max(wrTimer - Time.deltaTime, 0f);
                    break;

                case Mode.Walking:
                    Walk(dir, running ? runSpeed : groundSpeed, grAccel);
                    break;

                case Mode.Flying:
                    AirMove(dir, airSpeed, airAccel);
                    break;
            }

            jump = false;
        }



        private Vector3 Direction()
        {
            float hAxis = Input.GetAxisRaw("Horizontal");
            float vAxis = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(hAxis, 0, vAxis);
            return rb.transform.TransformDirection(direction);
        }



        #region Collisions
        void OnCollisionStay(Collision collision)
        {
            if (collision.contactCount > 0)
            {
                float angle;

                foreach (ContactPoint contact in collision.contacts)
                {
                    angle = Vector3.Angle(contact.normal, Vector3.up);
                    if (angle < wallFloorBarrier)
                    {
                        EnterWalking();
                        grounded = true;
                        groundNormal = contact.normal;
                        ground = contact.otherCollider;
                        return;
                    }
                }

                if (VectorToGround().magnitude > 0.2f)
                {
                    grounded = false;
                }

                if (grounded == false)
                {
                    foreach (ContactPoint contact in collision.contacts)
                    {
                        if (contact.otherCollider.tag != "NoWallrun" && contact.otherCollider.tag != "Player" && mode != Mode.Walking)
                        {
                            angle = Vector3.Angle(contact.normal, Vector3.up);
                            if (angle > wallFloorBarrier && angle < 120f)
                            {
                                grounded = true;
                                groundNormal = contact.normal;
                                ground = contact.otherCollider;
                                EnterWallrun();
                                return;
                            }
                        }
                    }
                }
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.contactCount == 0)
            {
                EnterFlying();
            }
        }
        #endregion



        #region Entering States
        void EnterWalking()
        {
            if (mode != Mode.Walking && canJump)
            {
                if (mode == Mode.Flying && crouched)
                {
                    rb.AddForce(rb.velocity.normalized, ForceMode.VelocityChange);
                }
                if (rb.velocity.y < -1.2f)
                {
                }
                //StartCoroutine(bHopCoroutine(bhopLeniency));
                mode = Mode.Walking;
            }
        }

        void EnterFlying(bool wishFly = false)
        {
            grounded = false;
            if (mode == Mode.Wallruning && VectorToWall().magnitude < wallStickDistance && !wishFly)
            {
                return;
            }
            else if (mode != Mode.Flying)
            {

                wallBan = wallBanTime;
                canDJump = true;
                mode = Mode.Flying;
            }
        }

        void EnterWallrun()
        {
            if (mode != Mode.Wallruning)
            {
                if (VectorToGround().magnitude > 0.2f && CanRunOnThisWall(bannedGroundNormal) && wallStickTimer == 0f)
                {
                    wrTimer = wallRunTime;
                    canDJump = true;
                    mode = Mode.Wallruning;
                }
                else
                {
                    EnterFlying(true);
                }
            }
        }
        #endregion



        #region Movement Types
        void Walk(Vector3 wishDir, float maxSpeed, float acceleration)
        {
            if (jump && canJump)
            {
                Jump();
            }
            else
            {
                if (crouched)
                    acceleration = 10f;
                wishDir = wishDir.normalized;
                Vector3 spid = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                if (spid.magnitude > maxSpeed) acceleration *= spid.magnitude / maxSpeed;
                Vector3 direction = wishDir * maxSpeed - spid;

                if (direction.magnitude < 0.5f)
                {
                    acceleration *= direction.magnitude / 0.5f;
                }

                direction = direction.normalized * acceleration;
                float magn = direction.magnitude;
                direction = direction.normalized;
                direction *= magn;

                Vector3 slopeCorrection = groundNormal * Physics.gravity.y / groundNormal.y;
                slopeCorrection.y = 0f;
                if (!crouched)
                    direction += slopeCorrection;


                rb.AddForce(direction, ForceMode.Acceleration);
            }
        }

        void AirMove(Vector3 wishDir, float maxSpeed, float acceleration)
        {
            if (jump && !crouched)
            {
                DoubleJump(wishDir);
            }

            if (crouched && rb.velocity.y > -10 && Input.GetKey(KeyCode.Space))
            {
                rb.AddForce(Vector3.down * 1f, ForceMode.Acceleration);
            }

            float projVel = Vector3.Dot(new Vector3(rb.velocity.x, 0f, rb.velocity.z), wishDir); // Vector projection of Current velocity onto accelDir.
            float accelVel = acceleration * Time.deltaTime; // Accelerated velocity in direction of movment

            // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
            if (projVel + accelVel > maxSpeed)
                accelVel = Mathf.Max(0f, maxSpeed - projVel);

            rb.AddForce(wishDir.normalized * accelVel, ForceMode.VelocityChange);
        }

        void Wallrun(Vector3 wishDir, float maxSpeed, float climbSpeed, float acceleration)
        {
            if (jump)
            {
                //Vertical
                float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
                rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

                //Horizontal
                Vector3 jumpOffWall = groundNormal.normalized;
                jumpOffWall *= dashSpeed;
                jumpOffWall.y = 0;
                rb.AddForce(jumpOffWall, ForceMode.VelocityChange);
                wrTimer = 0f;
                EnterFlying(true);
            }
            else if (wrTimer == 0f || crouched)
            {
                rb.AddForce(groundNormal * 3f, ForceMode.VelocityChange);
                EnterFlying(true);
            }
            else
            {
                //Horizontal
                Vector3 distance = VectorToWall();
                wishDir = RotateToPlane(wishDir, -distance.normalized);
                wishDir *= maxSpeed;
                wishDir.y = Mathf.Clamp(wishDir.y, -climbSpeed, climbSpeed);
                Vector3 wallrunForce = wishDir - rb.velocity;
                if (wallrunForce.magnitude > 0.2f) wallrunForce = wallrunForce.normalized * acceleration;

                //Vertical
                if (rb.velocity.y < 0f && wishDir.y > 0f) wallrunForce.y = 2f * acceleration;

                //Anti-gravity force
                Vector3 antiGravityForce = -Physics.gravity;
                if (wrTimer < 0.33 * wallRunTime)
                {
                    antiGravityForce *= wrTimer / wallRunTime;
                    wallrunForce += (Physics.gravity + antiGravityForce);
                }

                //Forces
                rb.AddForce(wallrunForce, ForceMode.Acceleration);
                rb.AddForce(antiGravityForce, ForceMode.Acceleration);
                if (distance.magnitude > wallStickDistance) distance = Vector3.zero;
                rb.AddForce(distance * wallStickiness, ForceMode.Acceleration);
            }
            if (!grounded)
            {
                wallStickTimer = 0.2f;
                EnterFlying();
            }
        }

        void Jump()
        {
            if (mode == Mode.Walking && canJump)
            {
                float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
                rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);
                StartCoroutine(jumpCooldownCoroutine(0.2f));
                EnterFlying(true);
            }
        }

        void DoubleJump(Vector3 wishDir)
        {
            if (canDJump)
            {
                //Vertical
                float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);

                rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

                //Horizontal
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

                canDJump = false;
            }
        }
        #endregion



        #region MathGenious
        Vector2 ClampedAdditionVector(Vector2 a, Vector2 b)
        {
            float k, x, y;
            k = Mathf.Sqrt(Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2)) / Mathf.Sqrt(Mathf.Pow(a.x + b.x, 2) + Mathf.Pow(a.y + b.y, 2));
            x = k * (a.x + b.x) - a.x;
            y = k * (a.y + b.y) - a.y;
            return new Vector2(x, y);
        }

        Vector3 RotateToPlane(Vector3 vect, Vector3 normal)
        {
            Vector3 rotDir = Vector3.ProjectOnPlane(normal, Vector3.up);
            Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
            rotDir = rotation * rotDir;
            float angle = -Vector3.Angle(Vector3.up, normal);
            rotation = Quaternion.AngleAxis(angle, rotDir);
            vect = rotation * vect;
            return vect;
        }

        float WallrunCameraAngle()
        {
            Vector3 rotDir = Vector3.ProjectOnPlane(groundNormal, Vector3.up);
            Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
            rotDir = rotation * rotDir;
            float angle = Vector3.SignedAngle(Vector3.up, groundNormal, Quaternion.AngleAxis(90f, rotDir) * groundNormal);
            angle -= 90;
            angle /= 180;
            Vector3 playerDir = transform.forward;
            Vector3 normal = new Vector3(groundNormal.x, 0, groundNormal.z);

            return Vector3.Cross(playerDir, normal).y * angle;
        }

        bool CanRunOnThisWall(Vector3 normal)
        {
            if (Vector3.Angle(normal, groundNormal) > 10 || wallBan == 0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Vector3 VectorToWall()
        {
            Vector3 direction;
            Vector3 position = transform.position + Vector3.up * col.height / 2f;
            RaycastHit hit;
            if (Physics.Raycast(position, -groundNormal, out hit, wallStickDistance) && Vector3.Angle(groundNormal, hit.normal) < 70)
            {
                groundNormal = hit.normal;
                direction = hit.point - position;
                return direction;
            }
            else
            {
                return Vector3.positiveInfinity;
            }
        }

        Vector3 VectorToGround()
        {
            Vector3 position = transform.position;
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, wallStickDistance))
            {
                return hit.point - position;
            }
            else
            {
                return Vector3.positiveInfinity;
            }
        }
        #endregion



        #region Coroutines
        IEnumerator jumpCooldownCoroutine(float time)
        {
            canJump = false;
            yield return new WaitForSeconds(time);
            canJump = true;
        }
        #endregion
    }
}
