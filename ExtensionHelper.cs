using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TopeBox
{
    public static class ExtensionHelper
    {
        private static Vector3 _TempVector3;
        private static Color _Color;

        public static Transform FindChildTransformByName(this Transform rootTransform, string nameFind)
        {
            Transform result = null;
            Transform[] childs = rootTransform.GetComponentsInChildren<Transform>();
            int length = childs.Length;
            for (int i = 0; i < length; i++)
            {
                Transform child = childs[i];
                if (child.name.Contains(nameFind))
                {
                    result = child;
                    break;
                }
            }

            return result;
        }

        public static Transform[] FindChildsTransformByName(this Transform rootTransform, string nameFind)
        {
            List<Transform> transforms = new List<Transform>();
            Transform[] childs = rootTransform.GetComponentsInChildren<Transform>();
            int length = childs.Length;
            for (int i = 0; i < length; i++)
            {
                Transform child = childs[i];
                if (child.name.Contains(nameFind))
                {
                    transforms.Add(child);
                }
            }

            return transforms.ToArray();
        }

        public static void SetPositionX(this Transform transform, float x)
        {
            _TempVector3 = transform.position;
            _TempVector3.x = x;
            transform.position = _TempVector3;
            //transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            _TempVector3 = transform.position;
            _TempVector3.y = y;
            transform.position = _TempVector3;
            //transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public static void SetPositionZ(this Transform transform, float z)
        {
            _TempVector3 = transform.position;
            _TempVector3.z = z;
            transform.position = _TempVector3;
            //transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        public static void SetLocalPositionX(this Transform transform, float x)
        {
            _TempVector3 = transform.localPosition;
            _TempVector3.x = x;
            transform.localPosition = _TempVector3;
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            _TempVector3 = transform.localPosition;
            _TempVector3.y = y;
            transform.localPosition = _TempVector3;
        }

        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            _TempVector3 = transform.localPosition;
            _TempVector3.z = z;
            transform.localPosition = _TempVector3;
        }

        public static void SetLocalScaleX(this Transform transform, float x)
        {
            _TempVector3 = transform.localScale;
            _TempVector3.x = x;
            transform.localScale = _TempVector3;
            //transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        }

        public static void SetLocalScaleY(this Transform transform, float y)
        {
            _TempVector3 = transform.localScale;
            _TempVector3.y = y;
            transform.localScale = _TempVector3;
            //transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        }

        public static void SetLocalScaleZ(this Transform transform, float z)
        {
            _TempVector3 = transform.localScale;
            _TempVector3.z = z;
            transform.localScale = _TempVector3;
            //transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
        }

        public static void SetEulerX(this Transform transform, float x)
        {
            _TempVector3 = transform.eulerAngles;
            _TempVector3.x = x;
            transform.eulerAngles = _TempVector3;
        }

        public static void SetEulerY(this Transform transform, float y)
        {
            _TempVector3 = transform.eulerAngles;
            _TempVector3.y = y;
            transform.eulerAngles = _TempVector3;
        }

        public static void SetEulerZ(this Transform transform, float z)
        {
            _TempVector3 = transform.eulerAngles;
            _TempVector3.z = z;
            transform.eulerAngles = _TempVector3;
        }

        public static void SetLocalEulerX(this Transform transform, float x)
        {
            _TempVector3 = transform.localEulerAngles;
            _TempVector3.x = x;
            transform.localEulerAngles = _TempVector3;
        }

        public static void SetLocalEulerY(this Transform transform, float y)
        {
            _TempVector3 = transform.localEulerAngles;
            _TempVector3.y = y;
            transform.localEulerAngles = _TempVector3;
        }

        public static void SetLocalEulerZ(this Transform transform, float z)
        {
            _TempVector3 = transform.localEulerAngles;
            _TempVector3.z = z;
            transform.localEulerAngles = _TempVector3;
        }

        public static void FlipFollowClientDirection(this Transform transform, float directionX, Vector3 worldScale)
        {
            int unitScale = directionX < 0 ? -1 : 1;
            transform.SetLocalScaleX(Mathf.Abs(transform.localScale.x) * unitScale * worldScale.x);
        }

        public static void SetGUIImageRedChannel(this Image image, float value)
        {
            _Color = image.color;
            _Color.r = value;
            image.color = _Color;
        }

        public static void SetGUIImageGreenChannel(this Image image, float value)
        {
            _Color = image.color;
            _Color.g = value;
            image.color = _Color;
        }

        public static void SetGUIImageBlueChannel(this Image image, float value)
        {
            _Color = image.color;
            _Color.b = value;
            image.color = _Color;
        }

        public static void SetGUIImageAlphaChannel(this Image image, float value)
        {
            _Color = image.color;
            _Color.a = value;
            image.color = _Color;
        }

        public static bool IsVisibleFromCamera(this Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }

        public static void SetSafeEnable(this Renderer renderer, bool enable)
        {
            if (renderer.enabled != enable)
            {
                renderer.enabled = enable;
            }
        }

        public static void SetSafeActive(this GameObject gameobject, bool enable)
        {
            if (gameobject.activeSelf != enable)
            {
                gameobject.SetActive(enable);
            }
        }

        /// <summary>
        /// Extension method to check if a layer is in a layermask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}