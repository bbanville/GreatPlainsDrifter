using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrendonBanville.Tools.CameraExtensions
{
    public class Transparent : MonoBehaviour
    {
        // References the object's color and mesh renderer
        Color startColor;
        MeshRenderer _meshRenderer;

        // Determines object's transparency
        // (0 = transparent, 1 = opaque)
        float alphaValue = 1.0f;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            startColor = _meshRenderer.material.color;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            // Fades the alpha value back to 1 over time
            alphaValue += 0.01f;
            // Clamps the alpha value so it does not exceed 1
            alphaValue = Mathf.Clamp(alphaValue, 0.0f, 1.0f);

            // Sets the object's color back to normal
            _meshRenderer.material.color = new Color(startColor.r, startColor.g, startColor.b, alphaValue);
        }

        public void SetAlpha()
        {
            // Makes the object semi-transparent
            alphaValue = 0.5f;
        }
    }
}
