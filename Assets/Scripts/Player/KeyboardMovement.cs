/*******************************************************************************
File: KeyboardMovement.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Contains the methods for player movement and jumping.
*******************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Managers;
using BrendonBanville.Tools;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class KeyboardMovement : MonoBehaviour
    {
        enum ControllerType { Rigidbody, Transform }
        [SerializeField] ControllerType controllerType = ControllerType.Transform;

        [Header("Movement Settings")]
        public float MoveSpeed = 3.5f;
        public float maxVelocityChange = 10.0f;
        public float Acceleration = 1f;
        [Range(1.0f, 2.0f)] public float sprintMultiplier = 1.7f;

        [Header("Jump Settings")]
        public float JumpStrength = 8.0f;
        public float MaxGroundedJumpTime = .25f;
        private float MaxJumpDelay = .25f;

        private Rigidbody _rigidbody;
        private float _currentMoveSpeed;
        private Vector3 _lastDirection;

        private bool _isJumping;

        public GameObject groundDetector;
        public bool _wasGrounded;
        private float _lastGroundedTime;
        private bool _justLanded = false;

        private float _lastJumpTime;

        [Header("Feedback Settings")]
        public List<AudioClip> MovementSounds;
        [Range(0.0f, 2.0f)]public float footStepDelay = 0.88f;
        float movementTimer = 0.0f;

        SoundManager SM;
        AudioSource _audioSource;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();
        }

        void FixedUpdate()
        {
            var horizontalMovement = Input.GetAxisRaw("Horizontal");
            var verticalMovement = Input.GetAxisRaw("Vertical");

            var moveVector = new Vector3(horizontalMovement, 0, verticalMovement);

            var magnitude = moveVector.magnitude > 1 ? 1 : moveVector.magnitude;
            var speed = magnitude * MoveSpeed;
            _currentMoveSpeed = _currentMoveSpeed.MoveTowards(speed, Acceleration);

            if (speed > 0)
            {
                _lastDirection = moveVector;
            }

            /// </summary>
            /// Directs to the player controller that is specified
            /// </summary>
            if (controllerType == ControllerType.Rigidbody)
            {
                var direction = (speed > 0 ? moveVector : _lastDirection) * _currentMoveSpeed * Time.deltaTime;
                var movement = Vector3.zero;
                
                var horz = Input.GetAxisRaw("Horizontal");
                var vert = Input.GetAxisRaw("Vertical");

                var rayDistance = .4f;
                var offset = new Vector3(0, -.2f, 0);

                if (Input.GetKey(KeyCode.W))
                {
                    movement += transform.forward;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    movement -= transform.right;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    movement -= transform.forward;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    movement += transform.right;
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speed = speed * sprintMultiplier;
                }

                movement = new Vector3(movement.x * speed, _rigidbody.velocity.y, movement.z * speed);

                _rigidbody.velocity = movement;
            }
            else if (controllerType == ControllerType.Transform)
            {
                /// </summary>
                /// Sets a movement direction
                /// </summary>
                
                _currentMoveSpeed = _currentMoveSpeed.MoveTowards(speed, Acceleration);

                if (speed > 0)
                {
                    _lastDirection = moveVector;
                }

                var movement = (speed > 0 ? moveVector : _lastDirection) * _currentMoveSpeed * Time.deltaTime;
                var rayDistance = .4f;
                var offset = new Vector3(0, -.2f, 0);

                if (movement.z > 0 && Physics.Raycast(transform.position + offset, transform.forward, rayDistance, 1 << 8))
                {
                    movement = new Vector3(movement.x, movement.y, 0);
                }
                else if (movement.z < 0 && Physics.Raycast(transform.position + offset, -transform.forward, rayDistance, 1 << 8))
                {
                    movement = new Vector3(movement.x, movement.y, 0);
                }

                if (movement.x > 0 && Physics.Raycast(transform.position + offset, transform.right, rayDistance, 1 << 8))
                {
                    movement = new Vector3(0, movement.y, movement.z);
                }
                else if (movement.x < 0 && Physics.Raycast(transform.position + offset, -transform.right, rayDistance, 1 << 8))
                {
                    movement = new Vector3(0, movement.y, movement.z);
                }

                transform.Translate(movement);
            }
        }

        void Update()
        {
            movementTimer += Time.deltaTime;

            /// </summary>
            /// Detects if the player is touching the ground and sets variables accordingly
            /// </summary>
            if (groundDetector.GetComponent<GroundDetector>().IsGrounded())
            {
                _isJumping = false;
                _wasGrounded = true;
                _lastGroundedTime = Time.time;
                
                if (!_justLanded)
                {
                    SM.PlaySoundFromDict("SandLand");
                    _justLanded = true;
                }

                if ((_rigidbody.velocity.x > 0 || _rigidbody.velocity.z > 0) && movementTimer >= footStepDelay)
                {
                    _audioSource.PlayOneShot(MovementSounds.Random());
                    movementTimer = 0;
                }
            }
            else if (Time.time - _lastGroundedTime > MaxGroundedJumpTime)
            {
                _wasGrounded = false;
                _isJumping = false;
            }

            /// </summary>
            /// Checks if the player can jump and then applies a vertical force if true
            /// </summary>
            var canJump = Time.time - _lastJumpTime > MaxJumpDelay;

            if (Input.GetKeyDown(KeyCode.Space) && _wasGrounded && !_isJumping && canJump)
            {
                _isJumping = true;
                _lastJumpTime = Time.time;
                SM.PlaySoundFromDict("SandJump");
                _justLanded = false;

                var rigidbody = GetComponent<Rigidbody>();
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, JumpStrength, rigidbody.velocity.z);
            }
        }

        void PlayRandomMovementSound()
        {
            if (MovementSounds.Count > 0)
            {
                _audioSource.PlayOneShot(MovementSounds.Random());
            }
        }
    }
}