/******************************************************************************
   File Name: ThirdPersonCamera.cs
   Author(s): Brendon Banville (brendonbanville@gmail.com)
     Project: BBTools

 Description: This file contains the functionality of the player camera.
******************************************************************************/

using BrendonBanville.Tools;
using BrendonBanville.Tools.CameraExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendonBanville.Controllers
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Header("Settings")]
        public Transform target;
        [Tooltip("Lerp speed between Camera States")]
        public float smoothCameraRotation = 12.0f;
        [Tooltip("What layer(s) will be culled")]
        public LayerMask cullingLayer = ~(1 << 8);
        [Tooltip("Debug purposes, lock the camera behind the character to better align the camera states")]
        public bool lockCamera;
        [Tooltip("Automatically rotates the camera in the direction the player is moving")]
        public bool autoCamRotate = false;
        [Tooltip("Lerp speed while camera is turning to follow the player")]
        public float autoCamRotationSpeed = 8.0f;
        public bool occlusionOn = true;

        [Tooltip("The cameras right offset position")]
        public float rightOffset = 0.0f;
        [Tooltip("The distance the camera should be set to when not handling occlusion")]
        public float defaultDistance = 2.5f;
        [Tooltip("The cameras vertical offset position")]
        public float height = 1.4f;
        [Tooltip("Lerp speed while camera is following the player")]
        public float smoothFollow = 10.0f;
        [Tooltip("Sensitivity on the X axis")]
        public float xMouseSensitivity = 3.0f;
        [Tooltip("Sensitivity on the Y axis")]
        public float yMouseSensitivity = 3.0f;
        [Tooltip("Minimum rotation value along the Y axis")]
        public float yMinLimit = -40.0f;
        [Tooltip("Maximum rotation value along the Y axis")]
        public float yMaxLimit = 80.0f;

        /// <summary>
        /// Component Dependencies
        /// </summary>
        [Header("Dependencies")]
        [HideInInspector] public int indexList, indexLookPoint;
        [HideInInspector] public float offSetPlayerPivot;
        [HideInInspector] public string currentStateName;
        [HideInInspector] public Transform currentTarget;
        [HideInInspector] public Vector2 movementSpeed;

        private Transform targetLookAt;
        private Vector3 currentTargetPos;
        private Vector3 lookPoint;
        private Vector3 current_cPos;
        private Vector3 desired_cPos;
        private Camera _camera;
        private float distance = 5f;
        private float mouseY = 0f;
        private float mouseX = 0f;
        private float currentHeight;
        private float cullingDistance;
        private float checkHeightRadius = 0.4f;
        private float clipPlaneMargin = 0f;
        private float forward = -1f;
        private float xMinLimit = -360f;
        private float xMaxLimit = 360f;
        private float cullingHeight = 0.2f;
        private float cullingMinDist = 0.1f;

        // A reference to the ThirdPersonCharacter on the object
        ThirdPersonCharacter characterController;

        /// <summary>
        /// Called on first frame
        /// </summary>
        void Start()
        {
            Init();
        }

        /// <summary>
        /// Initializes the camera controller variables
        /// </summary>
        public void Init()
        {
            if (target == null) { return; }
            _camera = GetComponentInChildren<Camera>();
            currentTarget = target;
            currentTargetPos = new Vector3(currentTarget.position.x, currentTarget.position.y + offSetPlayerPivot, currentTarget.position.z);

            targetLookAt = new GameObject("targetLookAt").transform;
            targetLookAt.position = currentTarget.position;
            targetLookAt.hideFlags = HideFlags.HideInHierarchy;
            targetLookAt.rotation = currentTarget.rotation;

            mouseY = currentTarget.eulerAngles.x;
            mouseX = currentTarget.eulerAngles.y;

            distance = defaultDistance;
            currentHeight = height;
        }

        /// <summary>
        /// Calls in sync with physics
        /// </summary>
        void FixedUpdate()
        {
            if (target == null || targetLookAt == null) return;

            CameraMovement();

            if (autoCamRotate)
            {
                RotateCameraFromInput();
            }
        }


        /// <summary>
        /// Set the target for the camera
        /// </summary>
        /// <param name="New cursorObject"></param>
        public void SetTarget(Transform newTarget)
        {
            currentTarget = newTarget ? newTarget : target;
        }

        /// <summary>
        /// Set the main target for the camera
        /// </summary>
        /// <param name="newTarget"></param>
        public void SetMainTarget(Transform newTarget)
        {
            target = newTarget;
            currentTarget = newTarget;
            mouseY = currentTarget.rotation.eulerAngles.x;
            mouseX = currentTarget.rotation.eulerAngles.y;
            Init();
        }

        /// <summary>    
        /// Convert a point in the screen in a Ray for the world
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public Ray ScreenPointToRay(Vector3 Point)
        {
            return this.GetComponentInChildren<Camera>().ScreenPointToRay(Point);
        }

        /// <summary>
        /// Camera Rotation behaviour
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RotateCamera(float x, float y)
        {
            // free rotation 
            mouseX += x * xMouseSensitivity;
            mouseY -= y * yMouseSensitivity;

            movementSpeed.x = x;
            movementSpeed.y = -y;

            if (!lockCamera)
            {
                mouseY = ExtensionMethods.ClampAngle(mouseY, yMinLimit, yMaxLimit);
                mouseX = ExtensionMethods.ClampAngle(mouseX, xMinLimit, xMaxLimit);
            }
            else
            {
                mouseY = currentTarget.root.localEulerAngles.x;
                mouseX = currentTarget.root.localEulerAngles.y;
            }
        }

        /// <summary>
        /// Camera behaviour
        /// </summary>    
        void CameraMovement()
        {
            if (currentTarget == null) { return; }

            distance = Mathf.Lerp(distance, defaultDistance, smoothFollow * Time.deltaTime);
            cullingDistance = Mathf.Lerp(cullingDistance, distance, Time.deltaTime);
            var camDir = (forward * targetLookAt.forward) + (rightOffset * targetLookAt.right);
            camDir = camDir.normalized;

            var targetPos = new Vector3(currentTarget.position.x, currentTarget.position.y + offSetPlayerPivot, currentTarget.position.z);
            currentTargetPos = targetPos;
            desired_cPos = targetPos + new Vector3(0, height, 0);
            current_cPos = currentTargetPos + new Vector3(0, currentHeight, 0);

            if (occlusionOn)
            {
                OcclusionCulling(targetPos, camDir);
            }

            var lookPoint = current_cPos + targetLookAt.forward * 2f;
            lookPoint += (targetLookAt.right * Vector3.Dot(camDir * (distance), targetLookAt.right));
            targetLookAt.position = current_cPos;

            Quaternion newRot = Quaternion.Euler(mouseY, mouseX, 0);
            targetLookAt.rotation = Quaternion.Slerp(targetLookAt.rotation, newRot, smoothCameraRotation * Time.deltaTime);
            transform.position = current_cPos + (camDir * (distance));
            var rotation = Quaternion.LookRotation((lookPoint) - transform.position);

            transform.rotation = rotation;
            movementSpeed = Vector2.zero;
        }

        /// <summary>
        /// Handles the calculations for camera occlusion and culling
        /// </summary>
        /// <param name="TargetPosition" = targetPos></param>
        /// <param name="CameraDirection" = camDir></param>
        public void OcclusionCulling(Vector3 TargetPosition, Vector3 CameraDirection)
        {
            RaycastHit hitInfo;

            ClipPlanePoints planePoints = _camera.NearClipPlanePoints(current_cPos + (CameraDirection * (distance)), clipPlaneMargin);
            ClipPlanePoints oldPoints = _camera.NearClipPlanePoints(desired_cPos + (CameraDirection * distance), clipPlaneMargin);

            //Check if Height is not blocked 
            if (Physics.SphereCast(TargetPosition, checkHeightRadius, Vector3.up, out hitInfo, cullingHeight + 0.2f, cullingLayer))
            {
                var t = hitInfo.distance - 0.2f;
                t -= height;
                t /= (cullingHeight - height);
                cullingHeight = Mathf.Lerp(height, cullingHeight, Mathf.Clamp(t, 0.0f, 1.0f));
            }

            //Check if desired target position is not blocked       
            if (CullingRayCast(desired_cPos, oldPoints, out hitInfo, distance + 0.2f, cullingLayer, Color.blue))
            {
                distance = hitInfo.distance - 0.2f;
                if (distance < defaultDistance)
                {
                    var t = hitInfo.distance;
                    t -= cullingMinDist;
                    t /= cullingMinDist;
                    currentHeight = Mathf.Lerp(cullingHeight, height, Mathf.Clamp(t, 0.0f, 1.0f));
                    current_cPos = currentTargetPos + new Vector3(0, currentHeight, 0);
                }
            }
            else
            {
                currentHeight = height;
            }

            //Check if target position with culling height applied is not blocked
            if (CullingRayCast(current_cPos, planePoints, out hitInfo, distance, cullingLayer, Color.cyan))
            {
                distance = Mathf.Clamp(cullingDistance, 0.0f, defaultDistance);
            }
        }

        /// <summary>
        /// Custom Raycast using NearClipPlanesPoints
        /// </summary>
        /// <param name="_to"></param>
        /// <param name="from"></param>
        /// <param name="hitInfo"></param>
        /// <param name="distance"></param>
        /// <param name="cullingLayer"></param>
        /// <returns></returns>
        bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance, LayerMask cullingLayer, Color color)
        {
            bool value = false;

            if (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, cullingLayer))
            {
                Tools.CameraExtensions.Transparent transparentObject = hitInfo.transform.GetComponent<Transparent>();

                if (transparentObject != null)
                {
                    value = false;
                    transparentObject.SetAlpha();
                }
                else
                {
                    value = true;
                    cullingDistance = hitInfo.distance;
                }
            }

            if (Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, cullingLayer))
            {
                Transparent transparentObject = hitInfo.transform.GetComponent<Transparent>();

                if (transparentObject != null)
                {
                    value = false;
                    transparentObject.SetAlpha();
                }
                else
                {
                    value = true;
                    if (cullingDistance > hitInfo.distance)
                    {
                        cullingDistance = hitInfo.distance;
                    }
                }
            }

            if (Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, cullingLayer))
            {
                Transparent transparentObject = hitInfo.transform.GetComponent<Transparent>();

                if (transparentObject != null)
                {
                    value = false;
                    transparentObject.SetAlpha();
                }
                else
                {
                    value = true;
                    if (cullingDistance > hitInfo.distance)
                    {
                        cullingDistance = hitInfo.distance;
                    }
                }
            }

            if (Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, cullingLayer))
            {
                Transparent transparentObject = hitInfo.transform.GetComponent<Transparent>();

                if (transparentObject != null)
                {
                    value = false;
                    transparentObject.SetAlpha();
                }
                else
                {
                    value = true;
                    if (cullingDistance > hitInfo.distance)
                    {
                        cullingDistance = hitInfo.distance;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Rotates the camera character with a given transform
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void RotateCameraWithAnotherTransform(Transform referenceTransform)
        {
            RotateCamera(-referenceTransform.eulerAngles.x, 0.0f);
        }

        public virtual void RotateCameraFromInput()
        {
            ThirdPersonUserControl userControl = target.GetComponent<ThirdPersonUserControl>();
            float userInput = Input.GetAxis(userControl.horizontalInput);

            if (Input.GetAxis(userControl.verticalInput) <= -0.05 || Input.GetAxis(userControl.verticalInput) >= 0.05)
            {
                if (Input.GetAxis(userControl.horizontalInput) <= -0.05 || Input.GetAxis(userControl.horizontalInput) >= 0.05)
                {
                    RotateCamera(userInput, 0.0f);
                }
            }
            else
            {
                //transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(0.0f, 1.0f, -3.0f), Time.deltaTime * autoCamRotationSpeed);
                RotateCamera(userInput, 0.0f);
            }
        }
    }
}
