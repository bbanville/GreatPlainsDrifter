/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Game;

namespace Opsive.UltimateCharacterController.Objects.ItemAssist
{
    /// <summary>
    /// The tracer will show a Line Renderer from the hitscan fire point to the hit point
    /// Updated by Nick F., MostWanted Game Development LLC
    /// </summary>
    public class Tracer : MonoBehaviour
    {
        [Tooltip("How long until the tracer is destroyed.")]
        public float m_VisibleTime;
        [Tooltip("distance per frame tracer effect moves.")]
        public float speed = 100f;
        [Tooltip("Length of tracer.")]
        public float length = .3f;
        [Tooltip("Max distance tracer will go if no hits.")]
        public float maxDistance = 100f;
        [Tooltip("Scale tracer from start to Max Distance. Use 0 to 1 for Time.")]
        public AnimationCurve scaleOverDistance;
        [Tooltip("Optional trail effect behind the tracer.")]
        public Tracer trail;
        [Tooltip("Flip tracer direction")]
        public bool flip;

        // Component references
        private Transform m_Transform;
        private LineRenderer m_LineRenderer;
        private Vector3 startPoint;
        private Vector3 endPoint;
        private float visibleTime;
        private float widthMultiplier;
        private float currentPosition;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_Transform = transform;
            m_LineRenderer = GetComponent<LineRenderer>();
            widthMultiplier = m_LineRenderer.widthMultiplier;
        }

        public void Initialize(Vector3 hitPoint)
        {
            startPoint = m_Transform.position;
            endPoint = hitPoint;
            currentPosition = 0f;
            m_LineRenderer.widthMultiplier = widthMultiplier;
            Scheduler.Schedule(m_VisibleTime, DestroyObject);
            if (trail)
            {
                Tracer newTrail = ObjectPool.Instantiate(trail.gameObject, transform.position, transform.rotation).GetComponent<Tracer>();
                newTrail.Initialize(hitPoint);
            }
        }

        private void Update()
        {
            // Move tracer forward
            currentPosition += speed * Time.deltaTime;
            float distance = Vector3.Distance(m_Transform.position, endPoint);
            // Calculate Head position
            m_LineRenderer.SetPosition((flip) ? 0 : 2, Vector3.Lerp(m_Transform.position, endPoint, currentPosition / distance));
            // Calculate Tail position
            Vector3 tail = (endPoint - m_Transform.position).normalized * -length;
            m_LineRenderer.SetPosition((flip) ? 2 : 0, (currentPosition - length > length) ? Vector3.Lerp(m_Transform.position + tail, endPoint, (currentPosition - length) / distance) : m_Transform.position);
            // Middle point at 25% to give a raindrop shape to it
            m_LineRenderer.SetPosition(1, Vector3.Lerp(m_LineRenderer.GetPosition(0), m_LineRenderer.GetPosition(2), (flip) ? .25f : .75f));
            // Adjust line width as it gets farther from view
            m_LineRenderer.widthMultiplier = widthMultiplier * scaleOverDistance.Evaluate(Vector3.Distance(m_Transform.position, m_LineRenderer.GetPosition(0)) / maxDistance);
        }

        private void DestroyObject()
        {
            ObjectPool.Destroy(gameObject);
        }
    }
}