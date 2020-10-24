using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BrendonBanville.Tools
{
    /// <summary>
    /// Contains all BBTools extension methods 
    /// </summary>
    public static class ExtensionMethods 
	{
		/// <summary>
		/// Determines if an animator contains a certain parameter, based on a type and a name
		/// </summary>
		/// <returns><c>true</c> if has parameter of type the specified self name type; otherwise, <c>false</c>.</returns>
		/// <param name="self">Self.</param>
		/// <param name="name">Name.</param>
		/// <param name="type">Type.</param>
		public static bool HasParameterOfType (this Animator self, string name, AnimatorControllerParameterType type) 
		{
			if (name == null || name == "") { return false; }
			AnimatorControllerParameter[] parameters = self.parameters;
			foreach (AnimatorControllerParameter currParam in parameters) 
			{
				if (currParam.type == type && currParam.name == name) 
				{
					return true;
				}
			}
			return false;
        }

        /// <summary>
        /// Returns true if a renderer is visible from a camera
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
        }

        /// <summary>
        /// Returns true if this rectangle intersects the other specified rectangle
        /// </summary>
        /// <param name="thisRectangle"></param>
        /// <param name="otherRectangle"></param>
        /// <returns></returns>
        public static bool Intersects(this Rect thisRectangle, Rect otherRectangle)
        {
            return !((thisRectangle.x > otherRectangle.xMax) || (thisRectangle.xMax < otherRectangle.x) || (thisRectangle.y > otherRectangle.yMax) || (thisRectangle.yMax < otherRectangle.y));
        }

        /// <summary>
        /// Returns bool if layer is within layermask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool Contains(this LayerMask mask, int layer) 
        {
             return ((mask.value & (1 << layer)) > 0);
        }
         
        /// <summary>
        /// Returns true if gameObject is within layermask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="gameobject"></param>
        /// <returns></returns>
		public static bool Contains(this LayerMask mask, GameObject gameobject) 
        {
             return ((mask.value & (1 << gameobject.layer)) > 0);
        }

		static List<Component> m_ComponentCache = new List<Component>();

        /// <summary>
        /// Grabs a component without allocating memory uselessly
        /// </summary>
        /// <param name="this"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
		public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType) 
		{ 
			@this.GetComponents(componentType, m_ComponentCache); 
			var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null; 
			m_ComponentCache.Clear(); 
			return component; 
		} 

        /// <summary>
        /// Grabs a component without allocating memory uselessly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
		public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
		{
			@this.GetComponents(typeof(T), m_ComponentCache);
			var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
			m_ComponentCache.Clear();
			return component as T;
		}

        /// <summary>
        /// Rotates a vector2 by angleInDegrees
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vector, float angleInDegrees)
        {
            float sin = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            return vector;
        }


        /// <summary>
        /// Normalizes an angle in degrees
        /// </summary>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static float NormalizeAngle(this float angleInDegrees)
        {
            angleInDegrees = angleInDegrees % 360f;
            if (angleInDegrees < 0)
            {
                angleInDegrees += 360f;
            }                
            return angleInDegrees;
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            for (int t = transform.childCount - 1; t >= 0; t--)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(t).gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(t).gameObject);
                }
            }
        }

        public static T[] Append<T>(this T[] arrayInitial, T[] arrayToAppend)
        {
            if (arrayToAppend == null)
            {
                throw new ArgumentNullException("The appended object cannot be null");
            }
            if ((arrayInitial is string) || (arrayToAppend is string))
            {
                throw new ArgumentException("The argument must be an enumerable");
            }
            T[] ret = new T[arrayInitial.Length + arrayToAppend.Length];
            arrayInitial.CopyTo(ret, 0);
            arrayToAppend.CopyTo(ret, arrayInitial.Length);

            return ret;
        }

        /// <summary>
        /// Normalized the angle. between -180 and 180 degrees
        /// </summary>
        /// <param Name="eulerAngle">Euler angle.</param>
        public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
        {
            var delta = eulerAngle;

            if (delta.x > 180) delta.x -= 360;
            else if (delta.x < -180) delta.x += 360;

            if (delta.y > 180) delta.y -= 360;
            else if (delta.y < -180) delta.y += 360;

            if (delta.z > 180) delta.z -= 360;
            else if (delta.z < -180) delta.z += 360;

            return new Vector3(delta.x, delta.y, delta.z);//round values to angle;
        }

        public static Vector3 Difference(this Vector3 vector, Vector3 otherVector)
        {
            return otherVector - vector;
        }
        public static void SetActiveChildren(this GameObject gameObjet, bool value)
        {
            foreach (Transform child in gameObjet.transform)
                child.gameObject.SetActive(value);
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360)
                    angle += 360;
                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }

        public static ClipPlanePoints NearClipPlanePoints(this Camera camera, Vector3 pos, float clipPlaneMargin)
        {
            var clipPlanePoints = new ClipPlanePoints();

            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;
            height *= 1 + clipPlaneMargin;
            width *= 1 + clipPlaneMargin;
            clipPlanePoints.LowerRight = pos + transform.right * width;
            clipPlanePoints.LowerRight -= transform.up * height;
            clipPlanePoints.LowerRight += transform.forward * distance;

            clipPlanePoints.LowerLeft = pos - transform.right * width;
            clipPlanePoints.LowerLeft -= transform.up * height;
            clipPlanePoints.LowerLeft += transform.forward * distance;

            clipPlanePoints.UpperRight = pos + transform.right * width;
            clipPlanePoints.UpperRight += transform.up * height;
            clipPlanePoints.UpperRight += transform.forward * distance;

            clipPlanePoints.UpperLeft = pos - transform.right * width;
            clipPlanePoints.UpperLeft += transform.up * height;
            clipPlanePoints.UpperLeft += transform.forward * distance;

            return clipPlanePoints;
        }
        public static HitBarPoints GetBoundPoint(this BoxCollider boxCollider, Transform torso, LayerMask mask)
        {
            HitBarPoints bp = new HitBarPoints();
            var boxPoint = boxCollider.GetBoxPoint();
            Ray toTop = new Ray(boxPoint.top, boxPoint.top - torso.position);
            Ray toCenter = new Ray(torso.position, boxPoint.center - torso.position);
            Ray toBottom = new Ray(torso.position, boxPoint.bottom - torso.position);
            Debug.DrawRay(toTop.origin, toTop.direction, Color.red, 2);
            Debug.DrawRay(toCenter.origin, toCenter.direction, Color.green, 2);
            Debug.DrawRay(toBottom.origin, toBottom.direction, Color.blue, 2);
            RaycastHit hit;
            var dist = Vector3.Distance(torso.position, boxPoint.top);
            if (Physics.Raycast(toTop, out hit, dist, mask))
            {
                bp |= HitBarPoints.Top;
                Debug.Log(hit.transform.name);
            }
            dist = Vector3.Distance(torso.position, boxPoint.center);
            if (Physics.Raycast(toCenter, out hit, dist, mask))
            {
                bp |= HitBarPoints.Center;
                Debug.Log(hit.transform.name);
            }
            dist = Vector3.Distance(torso.position, boxPoint.bottom);
            if (Physics.Raycast(toBottom, out hit, dist, mask))
            {
                bp |= HitBarPoints.Bottom;
                Debug.Log(hit.transform.name);
            }

            return bp;
        }
        public static BoxPoint GetBoxPoint(this BoxCollider boxCollider)
        {
            BoxPoint bp = new BoxPoint();
            bp.center = boxCollider.transform.TransformPoint(boxCollider.center);
            var height = boxCollider.transform.lossyScale.y * boxCollider.size.y;
            var ray = new Ray(bp.center, boxCollider.transform.up);

            bp.top = ray.GetPoint((height * 0.5f));
            bp.bottom = ray.GetPoint(-(height * 0.5f));

            return bp;
        }
        public static Vector3 BoxSize(this BoxCollider boxCollider)
        {
            var length = boxCollider.transform.lossyScale.x * boxCollider.size.x;
            var width = boxCollider.transform.lossyScale.z * boxCollider.size.z;
            var height = boxCollider.transform.lossyScale.y * boxCollider.size.y;
            return new Vector3(length, height, width);
        }
        public static bool Contains(this Enum keys, Enum flag)
        {
            if (keys.GetType() != flag.GetType())
            {
                throw new ArgumentException("Type Mismatch");
            }
            return (Convert.ToUInt64(keys) & Convert.ToUInt64(flag)) != 0;
        }

    }
    public struct BoxPoint
    {
        public Vector3 top;
        public Vector3 center;
        public Vector3 bottom;

    }
    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }
    [Flags]
    public enum HitBarPoints
    {
        None = 0,
        Top = 1,
        Center = 2,
        Bottom = 4
    }
}