/******************************************************************************
   File Name: ThirdPersonUserControl.cs
   Author(s): Brendon Banville (brendonbanville@gmail.com)
     Project: BBTools

 Description: This file tracks the control input of the player and translates
              it into player action.
******************************************************************************/

using BrendonBanville.Tools;
using System;
using UnityEngine;

namespace BrendonBanville.Controllers
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        public bool readPlayerInput = true;
        public bool allowJump = false;

        /// <summary>
        /// The inputs for player movement
        /// </summary>
        [Header("Default Inputs")]
        public string horizontalInput = "Horizontal";
        public string verticalInput = "Vertical";

        [Condition("allowJump", true)]
        public KeyCode jumpInput = KeyCode.Space;

        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        /// <summary>
        /// The settings for camera control
        /// </summary>
        [Header("Camera Settings")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        // access camera info    
        [HideInInspector]
        public ThirdPersonCamera tpCamera;
        // generic string to change the CameraState 
        [HideInInspector]
        public string customCameraState;
        // generic string to change the CameraPoint of the Fixed Point Mode 
        [HideInInspector]
        public string customlookAtPoint;
        // generic bool to change the CameraState
        [HideInInspector]
        public bool changeCameraState;
        // generic bool to know if the state will change with or without lerp
        [HideInInspector]
        public bool smoothCameraState;
        // keep the current direction in case you change the cameraState
        [HideInInspector]
        public bool keepDirection;

        // access the ThirdPersonCharacter component
        protected ThirdPersonCharacter cc;

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        {
            CharacterInit();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void CharacterInit()
        {
            cc = GetComponent<ThirdPersonCharacter>();
            if (cc != null)
            {
                cc.Init();
            }

            tpCamera = FindObjectOfType<ThirdPersonCamera>();
            if (tpCamera)
            {
                tpCamera.SetMainTarget(this.transform);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void LateUpdate()
        {
            // returns if didn't find the controller
            if (cc == null) { return; }

            if (readPlayerInput)
            {
                // update input methods
                InputHandle();
                // update camera states
                UpdateCameraStates();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void FixedUpdate()
        {
            cc.AirControl();
            CameraInput();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        {
            // call ThirdPersonMotor methods  
            cc.UpdateMotor();
            // call ThirdPersonAnimator methods
            cc.UpdateAnimator();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InputHandle()
        {
            CameraInput();

            if (!cc.lockMovement)
            {
                MoveCharacter();
                SprintInput();
                StrafeInput();

                if (allowJump)
                {
                    JumpInput();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void MoveCharacter()
        {
            cc.input.x = Input.GetAxis(horizontalInput);
            cc.input.y = Input.GetAxis(verticalInput);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
            {
                cc.Strafe();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
            {
                cc.Sprint(true);
            }
            else if (Input.GetKeyUp(sprintInput))
            {
                cc.Sprint(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput))
            {
                cc.Jump();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void CameraInput()
        {
            if (tpCamera == null) { return; }
            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);

            // tranform Character direction from camera if not KeepDirection
            if (!keepDirection)
            {
                cc.UpdateTargetDirection(tpCamera != null ? tpCamera.transform : null);
            }

            // rotate the character with the camera while strafing        
            RotateWithCamera(tpCamera != null ? tpCamera.transform : null);

            // rotate the character with the camera while strafing
            RotateWithPlayer(this.transform);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdateCameraStates()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<ThirdPersonCamera>();
                if (tpCamera == null)
                {
                    return;
                }
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameraTransform"></param>
        protected virtual void RotateWithCamera(Transform cameraTransform)
        {
            if (cc.isStrafing && !cc.lockMovement && !cc.lockMovement)
            {
                cc.RotatePlayerWithAnotherTransform(cameraTransform);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerModel"></param>
        protected virtual void RotateWithPlayer(Transform playerModel)
        {
            if (cc.isStrafing)
            {
                tpCamera.RotateCameraWithAnotherTransform(cc.transform);
            }
        }
    }
}
