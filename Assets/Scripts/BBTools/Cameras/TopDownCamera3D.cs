using UnityEngine;
using System.Collections;

namespace BrendonBanville.Controllers
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("BBTools/3D Controllers/TopDownCamera3D")]
    public class TopDownCamera3D : MonoBehaviour
    {
        #if UNITY_EDITOR
        public int lastTab = 0;

        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool targetingSettingsFoldout;
        public bool inputSettingsFoldout;
        #endif

        private Transform m_Transform; //camera tranform
        public bool useFixedUpdate = false; //use FixedUpdate() or Update()

        /// [Header("Movement")]
        public float keyboardMovementSpeed = 5f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSped = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;

        /// [Header("Height")]
        public bool autoHeight = true;
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 15f; //minimnal height
        public float heightDampening = 5f; 
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 25f;

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        /// [Header("Map Limits")]
        public bool limitMap = true;
        public float limitX = 50f; //x limit of map
        public float limitY = 50f; //z limit of map

        /// [Header("Targeting")]
        public Transform targetFollow; //target to follow
        public Vector3 targetOffset;

        private TopDownUserControl3D userControl { get { return this.gameObject.GetComponent<TopDownUserControl3D>(); } }

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        private void Start()
        {
            m_Transform = transform;
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {
            if (FollowingTarget)
                FollowTarget();
            else
                Move();

            HeightCalculation();
            Rotation();
            LimitPosition();
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (userControl.useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(userControl.KeyboardInput.x, 0, userControl.KeyboardInput.y);

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (userControl.useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, userControl.screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - userControl.screenEdgeBorder, 0, userControl.screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - userControl.screenEdgeBorder, Screen.width, userControl.screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, userControl.screenEdgeBorder);

                desiredMove.x = leftRect.Contains(userControl.MouseInput) ? -1 : rightRect.Contains(userControl.MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(userControl.MouseInput) ? 1 : downRect.Contains(userControl.MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }       
        
            if(userControl.usePanning && Input.GetKey(userControl.panningKey) && userControl.MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-userControl.MouseAxis.x, 0, -userControl.MouseAxis.y);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            float distanceToGround = DistanceToGround();
            if (userControl.useScrollwheelZooming)
            {
                zoomPos += userControl.ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            }
            if (userControl.useKeyboardZooming)
            {
                zoomPos += userControl.ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;
            }

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
            float difference = 0;

            if (distanceToGround != targetHeight)
            {
                difference = targetHeight - distanceToGround;
            }

            m_Transform.position = Vector3.Lerp(m_Transform.position, new Vector3(m_Transform.position.x, targetHeight + difference, m_Transform.position.z), Time.deltaTime * heightDampening);
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            if (userControl.useKeyboardRotation)
            {
                transform.Rotate(Vector3.up, userControl.RotationDirection * Time.deltaTime * rotationSped, Space.World);
            }

            if (userControl.useMouseRotation && Input.GetKey(userControl.mouseRotationKey))
            {
                m_Transform.Rotate(Vector3.up, -userControl.MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.World);
            }
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;
                
            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, -limitY, limitY));
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(m_Transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundMask.value))
                return (hit.point - m_Transform.position).magnitude;

            return 0f;
        }
    }
}