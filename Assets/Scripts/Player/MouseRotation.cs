using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Configuration;

namespace Assets.Scripts.Player
{
    public class MouseRotation : MonoBehaviour
    {
        public Transform viewportSphere;
        private Vector2 mouseInput;
        public float MinPitchAngle = -85.0f;
        public float MaxPitchAngle = 85.0f;
        private float PitchAngle = 0.0f;
        public Camera PlayerCam;

        void Update()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //Rotate camera as mouse moves horizontally
            var horizontalTurn = Input.GetAxis("Mouse X") * MouseConfiguration.MouseSensetivity_XAxis * Time.deltaTime;
            transform.Rotate(0, horizontalTurn, 0);

            //Rotate camera as mouse moves vertically
            PitchAngle += Input.GetAxis("Mouse Y") * MouseConfiguration.MouseSensetivity_YAxis * Time.deltaTime;
            PitchAngle = Mathf.Clamp(PitchAngle, MinPitchAngle, MaxPitchAngle);
            PlayerCam.transform.localRotation = Quaternion.Euler(-PitchAngle, 0.0f, 0.0f);
        }
    }
}
