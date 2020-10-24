using UnityEngine;

namespace BrendonBanville.Tools
{
    /// <summary>
    /// Class which contains many helpful methods which operates on Vectors
    /// </summary>
    public static class VectorMethods
    {
        /// <summary>
        /// Creating vector with random values in each axis
        /// </summary>
        public static Vector3 RandomVector(float rangeA, float rangeB)
        {
            return new Vector3(Random.Range(rangeA, rangeB), Random.Range(rangeA, rangeB), Random.Range(rangeA, rangeB));
        }

        /// <summary>
        /// Just summing all vector's axes values
        /// </summary>
        public static float VectorSum(Vector3 vector)
        {
            return vector.x + vector.y + vector.z;
        }

        /// <summary>
        /// Creating vector with random values in each axis but leaving y axis at 0f
        /// </summary>
        public static Vector3 RandomVectorNoY(float rangeA, float rangeB)
        {
            return new Vector3(Random.Range(rangeA, rangeB), 0f, Random.Range(rangeA, rangeB));
        }

        /// <summary>
        /// Creating vector with random values in each axis with min - max random ranges values
        /// </summary>
        public static Vector3 RandomVectorMinMax(float min, float max)
        {
            float mul1 = 1f;
            if (Random.Range(0, 2) == 1) mul1 = -1f;

            float mul2 = 1f;
            if (Random.Range(0, 2) == 1) mul2 = -1f;

            float mul3 = 1f;
            if (Random.Range(0, 2) == 1) mul3 = -1f;

            return new Vector3(Random.Range(min, max) * mul1, Random.Range(min, max) * mul2, Random.Range(min, max) * mul3);
        }

        /// <summary>
        /// Creating vector with random values in each axis with min - max random ranges values, but leaving y value to 0f
        /// </summary>
        public static Vector3 RandomVectorNoYMinMax(float min, float max)
        {
            float mul1 = 1f;
            if (Random.Range(0, 2) == 1) mul1 = -1f;

            float mul2 = 1f;
            if (Random.Range(0, 2) == 1) mul2 = -1f;

            return new Vector3(Random.Range(min, max) * mul1, 0f, Random.Range(min, max) * mul2);
        }

        /// <summary>
        /// Returning position on screen for UI element in reference to position in world 3D space, the 'z' will be negative if text is behind camera
        /// </summary>
        public static Vector3 GetUIPositionFromWorldPosition(Vector3 position, Camera camera, RectTransform canvas)
        {
            Vector3 uiPosition = camera.WorldToViewportPoint(position);

            uiPosition.x *= canvas.sizeDelta.x;
            uiPosition.y *= canvas.sizeDelta.y;

            uiPosition.x -= canvas.sizeDelta.x * canvas.pivot.x;
            uiPosition.y -= canvas.sizeDelta.y * canvas.pivot.y;

            return uiPosition;
        }

        /// <summary>
        /// Extracting position from Matrix
        /// </summary>
        public static Vector3 PosFromMatrix(Matrix4x4 m)
        {
            return m.GetColumn(3);
        }

        /// <summary>
        /// Extracting rotation from Matrix
        /// </summary>
        public static Quaternion RotFromMatrix(Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        /// <summary>
        /// Extracting scale from Matrix
        /// </summary>
        public static Vector3 ScaleFromMatrix(Matrix4x4 m)
        {
            return new Vector3
            (
                m.GetColumn(0).magnitude,
                m.GetColumn(1).magnitude,
                m.GetColumn(2).magnitude
            );
        }

        /// <summary>
        /// Smoothes a Vector3 that represents euler angles.
        /// </summary>
        /// <param name="current">The current Vector3 value.</param>
        /// <param name="target">The target Vector3 value.</param>
        /// <param name="velocity">A refernce Vector3 used internally.</param>
        /// <param name="smoothTime">The time to smooth, in seconds.</param>
        /// <returns>The smoothed Vector3 value.</returns>
        public static Vector3 SmoothDampEuler(Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
        {
            Vector3 v;

            v.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, smoothTime);
            v.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, smoothTime);
            v.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, smoothTime);

            return v;
        }

        /// <summary>
        /// Multiplies each element in Vector3 v by the corresponding element of w.
        /// </summary>
        public static Vector3 MultiplyVectors(Vector3 v, Vector3 w)
        {
            v.x *= w.x;
            v.y *= w.y;
            v.z *= w.z;

            return v;
        }
    }
}
