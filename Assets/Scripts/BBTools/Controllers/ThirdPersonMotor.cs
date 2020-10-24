/******************************************************************************
   File Name: ThirdPersonMotor.cs
   Author(s): Brendon Banville (brendonbanville@gmail.com)
     Project: BBTools

 Description: This file contains the functionality of a player movement motor
              as well as all of the base variables for the other aspects of
              the ThirdPerson Controller.
******************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using BrendonBanville.Tools;
using BrendonBanville.Tools.CameraExtensions;

namespace BrendonBanville.Controllers
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public abstract class ThirdPersonMotor : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Header("Layers")]

        // Layers that the character can walk on
        public LayerMask groundLayer = 1 << 0;

        // Distance to became not grounded
        [SerializeField] protected float groundMinDistance = 0.2f;
        [SerializeField] protected float groundMaxDistance = 0.5f;

        public enum MovementType
        {
            FreeWithStrafe,
            OnlyStrafe,
            OnlyFree
        }

        /// <summary>
        /// 
        /// </summary>
        [Header("Setup")]

        public MovementType movementType = MovementType.FreeWithStrafe;

        // Lock the player movement
        public bool lockMovement;

        // Speed of the rotation on free directional movement

        [SerializeField] public float freeRotationSpeed = 10f;

        // Speed of the rotation while strafe movement
        public float strafeRotationSpeed = 10f;

        /// <summary>
        /// 
        /// </summary>
        [Header("Jump Options")]

        // Check to control the character while jumping
        public bool jumpAirControl = true;
        // How much time the character will be jumping
        public float jumpTimer = 0.3f;
        [HideInInspector]
        public float jumpCounter;
        // Add Extra jump speed, based on your speed input the character will move forward
        public float jumpForward = 3f;
        // Add Extra jump height, if you want to jump only with Root Motion leave the value with 0
        public float jumpHeight = 4f;

        public Feedback JumpFeedback;
        public float bigAirTime = 0.8f;
        float airTimeTimer = 0;
        float airTime = 0;
        bool airTimeClocked = false;

        /// <summary>
        /// 
        /// </summary>
        [Header("Movement Speed")]

        public bool useRootMotion = false;
        public float freeWalkSpeed = 2.5f;
        public float freeRunningSpeed = 3f;
        public float freeSprintSpeed = 4f;
        public float strafeWalkSpeed = 2.5f;
        public float strafeRunningSpeed = 3f;
        public float strafeSprintSpeed = 4f;

        public Feedback MovementFeedback;

        /// <summary>
        /// 
        /// </summary>
        [Header("Grounded Setup")]

        public float stepOffsetEnd = 0.45f;
        public float stepOffsetStart = 0.05f;
        public float stepSmooth = 4f;
        [SerializeField] protected float slopeLimit = 45f;
        [SerializeField] protected float extraGravity = -10f;
        protected float groundDistance;
        public RaycastHit groundHit;

        // movement bools
        [HideInInspector]
        public bool isGrounded, isStrafing, isSprinting, isSliding;

        // action bools
        [HideInInspector]
        public bool isJumping;

        [HideInInspector]
        public Vector3 targetDirection;
        protected Quaternion targetRotation;
        [HideInInspector]
        public Quaternion freeRotation;
        [HideInInspector]
        public bool keepDirection;

        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public Rigidbody _rigidbody;
        [HideInInspector]
        public PhysicMaterial maxFrictionPhysics, frictionPhysics, slippyPhysics;
        [HideInInspector]
        public CapsuleCollider _capsuleCollider;

        [HideInInspector]
        public float colliderHeight;             
        [HideInInspector]
        public Vector2 input;      
        [HideInInspector]
        public float speed, direction, verticalVelocity;

        // velocity to apply to rigidbody
        [HideInInspector]
        public float velocity;

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (!isGrounded)
            {
                airTimeTimer += Time.deltaTime;
            }
            
            /// Temporary Solution
            if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("LandLow") && !airTimeClocked)
            {
                airTime = airTimeTimer;
                airTimeClocked = true;
                airTimeTimer = 0;

                if (airTime >= bigAirTime)
                {
                    JumpFeedback.ShakeCamera(CameraShakePresets.IntenseBump);
                }
                else
                {
                    JumpFeedback.ShakeCamera(CameraShakePresets.MediumBump);
                }
            }
            else if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("LandLow") && airTimeClocked)
            {
                airTimeClocked = false;
                airTimeTimer = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            // this method is called on the Start of the ThirdPersonController

            // access components
            animator = GetComponent<Animator>();

            // slides the character through walls and edges
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            // rigidbody info
            _rigidbody = GetComponent<Rigidbody>();

            // capsule collider info
            _capsuleCollider = GetComponent<CapsuleCollider>();
            colliderHeight = _capsuleCollider.height;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdateMotor()
        {
            CheckGround();
            ControlJumpBehaviour();
            ControlLocomotion();
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool freeLocomotionConditions
        {
            get
            {
                if (movementType.Equals(MovementType.OnlyStrafe)) { isStrafing = true; }
                return !isStrafing && !movementType.Equals(MovementType.OnlyStrafe) || movementType.Equals(MovementType.OnlyFree);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ControlLocomotion()
        {
            if (freeLocomotionConditions)
            {
                FreeMovement();     // free directional movement
            }
            else
            {
                StrafeMovement();   // move forward, backwards, strafe left and right
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void StrafeMovement()
        {
            var _speed = Mathf.Clamp(input.y, -1f, 1f);
            var _direction = Mathf.Clamp(input.x, -1f, 1f);
            speed = _speed;
            direction = _direction;
            if (isSprinting) speed += 0.5f;
            if (direction >= 0.7 || direction <= -0.7 || speed <= 0.1) isSprinting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void FreeMovement()
        {
            // Set speed to both vertical and horizontal inputs
            speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);            
            speed = Mathf.Clamp(speed, 0, 1f);
            // Add 0.5f on sprint to change the animation on animator
            if (isSprinting) speed += 0.5f;
                        
            if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
            {
                Vector3 lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                var eulerY = transform.eulerAngles.y;

                // Apply free directional rotation while not turning180 animations
                if (isGrounded || (!isGrounded && jumpAirControl))
                {
                    if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
                    var euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), freeRotationSpeed * Time.deltaTime);
                }               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="velocity"></param>
        protected void ControlSpeed(float velocity)
        {
            if (Time.deltaTime == 0) return;

            if (useRootMotion)
            {
                Vector3 v = (animator.deltaPosition * (velocity > 0 ? velocity : 1f)) / Time.deltaTime;
                v.y = _rigidbody.velocity.y;
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, v, 20f * Time.deltaTime);
            }
            else
            {
                var velY = transform.forward * velocity * speed;
                velY.y = _rigidbody.velocity.y;
                var velX = transform.right * velocity * direction;
                velX.x = _rigidbody.velocity.x;

                if (isStrafing)
                {
                    Vector3 v = (transform.TransformDirection(new Vector3(input.x, 0, input.y)) * (velocity > 0 ? velocity : 1f));
                    v.y = _rigidbody.velocity.y;
                    _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, v, 20f * Time.deltaTime);
                }
                else
                {
                    _rigidbody.velocity = velY;
                    _rigidbody.AddForce(transform.forward * (velocity * speed) * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ControlJumpBehaviour()
        {
            if (!isJumping) return;

            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                isJumping = false;
            }

            // apply extra force to the jump height   
            var vel = _rigidbody.velocity;
            vel.y = jumpHeight;
            _rigidbody.velocity = vel;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AirControl()
        {
            if (isGrounded) return;
            if (!jumpFwdCondition) return;

            var velY = transform.forward * jumpForward * speed;
            velY.y = _rigidbody.velocity.y;
            var velX = transform.right * jumpForward * direction;
            velX.x = _rigidbody.velocity.x;            

            if (jumpAirControl)
            {
                if (isStrafing)
                {
                    _rigidbody.velocity = new Vector3(velX.x, velY.y, _rigidbody.velocity.z);
                    var vel = transform.forward * (jumpForward * speed) + transform.right * (jumpForward * direction);
                    _rigidbody.velocity = new Vector3(vel.x, _rigidbody.velocity.y, vel.z);
                }
                else
                {
                    var vel = transform.forward * (jumpForward * speed);
                    _rigidbody.velocity = new Vector3(vel.x, _rigidbody.velocity.y, vel.z);
                }
            }
            else
            {
                var vel = transform.forward * (jumpForward);
                _rigidbody.velocity = new Vector3(vel.x, _rigidbody.velocity.y, vel.z);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool jumpFwdCondition
        {
            get
            {
                Vector3 p1 = transform.position + _capsuleCollider.center + Vector3.up * -_capsuleCollider.height * 0.5F;
                Vector3 p2 = p1 + Vector3.up * _capsuleCollider.height;
                return Physics.CapsuleCastAll(p1, p2, _capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void CheckGround()
        {
            CheckGroundDistance();

            // change the physics material to very slip when not grounded or maxFriction when is
            if (isGrounded && input == Vector2.zero)
            {
                _capsuleCollider.material = maxFrictionPhysics;
            }
            else if (isGrounded && input != Vector2.zero)
            {
                _capsuleCollider.material = frictionPhysics;
            }
            else
            {
                _capsuleCollider.material = slippyPhysics;
            }

            var magVel = (float)System.Math.Round(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z).magnitude, 2);
            magVel = Mathf.Clamp(magVel, 0, 1);

            var groundCheckDistance = groundMinDistance;
            if (magVel > 0.25f) groundCheckDistance = groundMaxDistance;

            // clear the checkground to free the character to attack on air                
            var onStep = StepOffset();

            if (groundDistance <= 0.05f)
            {
                isGrounded = true;
                Sliding();
            }
            else
            {
                if (groundDistance >= groundCheckDistance)
                {
                    isGrounded = false;
                    // check vertical velocity
                    verticalVelocity = _rigidbody.velocity.y;
                    // apply extra gravity when falling
                    if (!onStep && !isJumping)
                        _rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                }
                else if (!onStep && !isJumping)
                {
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void CheckGroundDistance()
        {
            if (_capsuleCollider != null)
            {
                // radius of the SphereCast
                float radius = _capsuleCollider.radius * 0.9f;
                var dist = 10f;

                // position of the SphereCast origin starting at the base of the capsule
                Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
                // ray for RayCast
                Ray ray1 = new Ray(transform.position, Vector3.down);
                // ray for SphereCast
                Ray ray2 = new Ray(pos, -Vector3.up);

                // raycast for check the ground distance
                if (Physics.Raycast(ray1, out groundHit, colliderHeight / 2 + 2f, groundLayer))
                {
                    Debug.DrawRay(transform.position, Vector3.down, Color.red);
                    dist = Math.Abs(transform.position.y - (colliderHeight / 2) - groundHit.point.y);
                }

                // sphere cast around the base of the capsule to check the ground distance
                if (Physics.SphereCast(ray2, radius, out groundHit, _capsuleCollider.radius + 2f, groundLayer))
                {
                    // check if sphereCast distance is small than the ray cast distance
                    if (dist > (groundHit.distance - _capsuleCollider.radius * 0.1f))
                    {
                        dist = (groundHit.distance - _capsuleCollider.radius * 0.1f);
                    }
                }

                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }      

        /// <summary>
        /// 
        /// </summary>
        void Sliding()
        {
            var onStep = StepOffset();
            var groundAngleTwo = 0f;
            RaycastHit hitinfo;
            Ray ray = new Ray(transform.position, -transform.up);

            if (Physics.Raycast(ray, out hitinfo, 1f, groundLayer))
            {
                groundAngleTwo = Vector3.Angle(Vector3.up, hitinfo.normal);
            }

            if (GroundAngle() > slopeLimit + 1f && GroundAngle() <= 85 &&
                groundAngleTwo > slopeLimit + 1f && groundAngleTwo <= 85 &&
                groundDistance <= 0.05f && !onStep)
            {
                isSliding = true;
                isGrounded = false;
                var slideVelocity = (GroundAngle() - slopeLimit) * 2f;
                slideVelocity = Mathf.Clamp(slideVelocity, 0, 10);
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -slideVelocity, _rigidbody.velocity.z);
            }
            else
            {
                isSliding = false;
                isGrounded = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool StepOffset()
        {
            if (input.sqrMagnitude < 0.1 || !isGrounded) return false;

            var _hit = new RaycastHit();
            var _movementDirection = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.y).normalized : transform.forward;
            Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + _movementDirection * ((_capsuleCollider).radius + 0.05f)), Vector3.down);

            if (Physics.Raycast(rayStep, out _hit, stepOffsetEnd - stepOffsetStart, groundLayer) && !_hit.collider.isTrigger)
            {
                if (_hit.point.y >= (transform.position.y) && _hit.point.y <= (transform.position.y + stepOffsetEnd))
                {
                    var _speed = isStrafing ? Mathf.Clamp(input.magnitude, 0, 1) : speed;
                    var velocityDirection = isStrafing ? (_hit.point - transform.position) : (_hit.point - transform.position).normalized;
                    _rigidbody.velocity = velocityDirection * stepSmooth * (_speed * (velocity > 1 ? velocity : 1));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public virtual void RotateToTarget(Transform target)
        {
            if (target)
            {
                Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
                targetRotation = Quaternion.Euler(newPos);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), strafeRotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Update the targetDirection variable using referenceTransform or just input.Rotate by word  the referenceDirection
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void UpdateTargetDirection(Transform referenceTransform = null)
        {
            if (referenceTransform)
            {
                var forward = keepDirection ? referenceTransform.forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0;

                forward = keepDirection ? forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0; //set to 0 because of referenceTransform rotation on the X axis

                //get the right-facing direction of the referenceTransform
                var right = keepDirection ? referenceTransform.right : referenceTransform.TransformDirection(Vector3.right);

                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                targetDirection = input.x * right + input.y * forward;
            }
            else
                targetDirection = keepDirection ? targetDirection : new Vector3(input.x, 0, input.y);
        }
    }
}