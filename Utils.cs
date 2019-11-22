using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace TopeBox
{
    public static class Utils
    {
        public static readonly Vector3 VECTOR_3_ZERO = new Vector3(0, 0, 0);
        public static readonly Vector3 VECTOR_3_ONE = new Vector3(1f, 1f, 1f);
        public static readonly Vector4 COLOR_ZERO_ALPHA = new Vector4(1, 1, 1, 0);

        private static StringBuilder _StringBuilderTemp = new StringBuilder();

        /// <summary>
        /// Create chain string using StringBuilder
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string CreateStrings(params string[] strs)
        {
            _StringBuilderTemp.Length = 0;
            _StringBuilderTemp.Capacity = 0;

            for (int i = 0; i < strs.Length; i++)
            {
                _StringBuilderTemp.Append(strs[i]);
            }

            return _StringBuilderTemp.ToString();
        }

        /// <summary>
        /// Creates the rich text color string.
        /// </summary>
        /// <returns>The rich text color string.</returns>
        /// <param name="text">Text.</param>
        /// <param name="replaceStr">Replace string.</param>
        /// <param name="color">Color.</param>
        public static string CreateRichTextColorString(string text, string replaceStr, string replaceStrValue, Color32 color)
        {
            //Test <color=#0076FFFF>AA</color>
            string cssValue = CreateStrings("<color=#", ColorToHex(color), ">", replaceStrValue, "</color>");
            StringBuilder strBuilder = new StringBuilder(text);
            if (!string.IsNullOrEmpty(replaceStr))
            {
                strBuilder.Replace(replaceStr, cssValue);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Creates the rich text color string. Transform the text to css format text
        /// </summary>
        /// <returns>The rich text color string.</returns>
        /// <param name="text">Text.</param>
        /// <param name="color">Color.</param>
        public static string CreateRichTextColorString(string text, Color32 color)
        {
            return CreateStrings("<color=#", ColorToHex(color), ">", text, "</color>");
        }

        /// <summary>
        /// Creates the rich text color string.
        /// </summary>
        /// <returns>The rich text color string.</returns>
        /// <param name="text">Text.</param>
        /// <param name="hexColor">Hex color.</param>
        public static string CreateRichTextColorString(string text, string hexColor)
        {
            return CreateStrings("<color=#", hexColor, ">", text, "</color>");
        }

        /// <summary>
        /// Check two vector have same direction.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool VectorsSameDirection(Vector3 u, Vector3 v)
        {
            bool isSameDir = false;
            if (((u.x > 0 && v.x > 0) || (u.x < 0 && v.x < 0))
                && ((u.y > 0 && v.y > 0) || (u.y < 0 && v.y < 0)))
            {
                Vector3 perpendicularU = new Vector3(-u.y, u.x, u.z);
                isSameDir = Vector3.Dot(perpendicularU, v) == 0 ? true : false;
            }

            return isSameDir;
        }

        /// <summary>
        /// Convert from Pixel unity to Unity's unit (with 100 pixel == 1 Unity unit)
        /// </summary>
        /// <returns></returns>
        public static float ConvertPixelToUnit(int pixelUnit)
        {
            return (1f * pixelUnit / 100f);
        }

        public static float ConvertPixelToUnit(float pixelUnit)
        {
            return (1f * pixelUnit / 100f);
        }

        /// <summary>
        /// convert from time to frame according to FPS
        /// With value in ONE second
        /// </summary>
        /// <returns></returns>
        public static float ConvertValueOnTimeToValueOnFrames(float value, int FPS)
        {
            float result = value / FPS;
            result = (result * 100f) / 100f;

            return result;
        }

        public static int convertTimeToFrames(float timeValue, int FPS)
        {
            int result = Mathf.FloorToInt(timeValue / (1f / FPS));
            return result;
        }

        public static float ConvertPercentToUnit(float percent)
        {
            return (percent) / 100f;
        }

        /// <summary>
        /// Get the Top left of screen followed camera
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetTopRightScreenPosition()
        {
            Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
            position.z = 0;
            return position;
        }

        /// <summary>
        /// Get the bottom left of screen followed camera
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetBottomLeftScreenPosition()
        {
            Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
            position.z = 0;
            return position;
        }

        /// <summary>
        /// Return Position around circle
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Vector3[] GetArrangePositionCircle(int size, float radius)
        {
            Vector3[] positions = new Vector3[size];

            float spaceDegree = (Mathf.Deg2Rad * 360f) / size;
            for (int i = 0; i < size; i++)
            {
                float x = (radius * Mathf.Cos(spaceDegree * i));
                float y = (radius * Mathf.Sin(spaceDegree * i));
                positions[i] = new Vector3(x, y, 0);
            }

            return positions;
        }

        #region Surtherland check intersect between line and rectangle (NO ROTATING)
        /// <summary>///////////////////////////////////////////////////////////
        /// Check line interect with rectangle (NO ROTATING)
        /// </summary>
        /// <returns></returns>
        /// https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
        public static bool CheckLineIntersectRectangle(float x0, float y0, float x1, float y1, float xmin, float ymin, float xmax, float ymax)
        {
            const int LEFT = 1;   // 0001
            const int RIGHT = 2;  // 0010
            const int BOTTOM = 4; // 0100
            const int TOP = 8;    // 1000

            // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
            int outcode0 = ComputeOutCode(x0, y0, xmin, ymin, xmax, ymax);
            int outcode1 = ComputeOutCode(x1, y1, xmin, ymin, xmax, ymax);
            bool accept = false;
            while (true)
            {
                //if (!(outcode0 | outcode1))
                if ((outcode0 | outcode1) == 0)
                { // Bitwise OR is 0. Trivially accept and get out of loop
                    accept = true;
                    break;
                }
                //else if (outcode0 & outcode1)
                else if ((outcode0 & outcode1) > 0)
                { // Bitwise AND is not 0. Trivially reject and get out of loop
                    break;
                }
                else
                {
                    // failed both tests, so calculate the line segment to clip
                    // from an outside point to an intersection with clip edge
                    float x = 0;
                    float y = 0;

                    // At least one endpoint is outside the clip rectangle; pick it.
                    int outcodeOut = (outcode0 > 0) ? outcode0 : outcode1;

                    // Now find the intersection point;
                    // use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
                    if ((outcodeOut & TOP) > 0)
                    { // point is above the clip rectangle
                        x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
                        y = ymax;
                    }
                    else if ((outcodeOut & BOTTOM) > 0)
                    { // point is below the clip rectangle
                        x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
                        y = ymin;
                    }
                    else if ((outcodeOut & RIGHT) > 0)
                    {  // point is to the right of clip rectangle
                        y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
                        x = xmax;
                    }
                    else if ((outcodeOut & LEFT) > 0)
                    {   // point is to the left of clip rectangle
                        y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
                        x = xmin;
                    }

                    // Now we move outside point to intersection point to clip
                    // and get ready for next pass.
                    if (outcodeOut == outcode0)
                    {
                        x0 = x;
                        y0 = y;
                        outcode0 = ComputeOutCode(x0, y0, xmin, ymin, xmax, ymax);
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                        outcode1 = ComputeOutCode(x1, y1, xmin, ymin, xmax, ymax);
                    }
                }
            }
            return accept;
        }

        public static int ComputeOutCode(float x, float y, float xmin, float ymin, float xmax, float ymax)
        {
            const int INSIDE = 0; // 0000
            const int LEFT = 1;   // 0001
            const int RIGHT = 2;  // 0010
            const int BOTTOM = 4; // 0100
            const int TOP = 8;    // 1000

            int code;

            code = INSIDE;          // initialised as being inside of clip window

            if (x < xmin)         // to the left of clip window
                code |= LEFT;
            else if (x > xmax)      // to the right of clip window
                code |= RIGHT;
            if (y < ymin)           // below the clip window
                code |= BOTTOM;
            else if (y > ymax)      // above the clip window
                code |= TOP;

            return code;
        }
        ////////////////////////////////////////////////////////////////////////
        #endregion

        public static void SaveToDebugFile(string text)
        {
            string path = "Debug_FILE_MESSAGE.txt";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter swNew = File.CreateText(path))
                {
                    swNew.WriteLine(text);
                    swNew.WriteLine("");
                    swNew.Close();
                }
            }
            else
            {
                using (StreamWriter swAppend = File.AppendText(path))
                {
                    swAppend.WriteLine(text);
                    swAppend.WriteLine("");
                    swAppend.Close();
                }
            }
        }

        /// <summary>
        /// Get Third point of triangle
        /// </summary>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static Vector3 GetThirdPointOfTriangle(Vector3 firstPoint, Vector3 secondPoint, float strength)
        {
            int Clockwise = -1;// counterclockwise
            if (firstPoint.x > secondPoint.x)
            {
                Clockwise = 1;
            }

            Vector3 mid = (secondPoint + firstPoint) / 2f;

            Vector3 tangent = (mid - firstPoint);
            Vector3 normal = new Vector3(tangent.y * Clockwise, -tangent.x * Clockwise, 0);

            Vector3 resultMid = mid + normal * strength;

            return resultMid;
        }

        public static float RoundFloatTo2Digit(float value)
        {
            return ((int)(value * 100)) * 1f / 100f;
        }

        public static float GetRotationAnim(Vector3 position, Vector3 desPosition)
        {
            Vector3 distance = desPosition - position;
            return Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Colors to hex.
        /// </summary>
        /// <returns>The to hex.</returns>
        /// <param name="color">Color.</param>
        public static string ColorToHex(Color32 color)
        {
            return CreateStrings(color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
        }

        /// <summary>
        /// For Ipad (1024x768)
        /// </summary>
        /// <returns></returns>
        public static bool Is4x3Resolution()
        {
            return (Mathf.Abs((Screen.width * 1.0f / Screen.height * 1.0f) - (1024f / 768f)) <= 0.01f);
        }

        /// <summary>
        /// For 1920x1200
        /// </summary>
        /// <returns></returns>
        public static bool Is16x10Resolution()
        {
            return (Mathf.Abs((Screen.width * 1.0f / Screen.height * 1.0f) - (1920f / 1200f)) <= 0.01f);
        }

        /// <summary>
        /// For 1334x750
        /// </summary>
        /// <returns></returns>
        public static bool Is16x9Resolution()
        {
            return (Mathf.Abs((Screen.width * 1.0f / Screen.height * 1.0f) - (1335f / 750f)) <= 0.01f);
        }

        /// <summary>
        /// For 960x640, 480x320
        /// </summary>
        /// <returns></returns>
        public static bool Is3x2Resolution()
        {
            return (Mathf.Abs((Screen.width * 1.0f / Screen.height * 1.0f) - (960f / 640f)) <= 0.01f);
        }

        /// <summary>
        /// For 800x480
        /// </summary>
        /// <returns></returns>
        public static bool Is5x3Resolution()
        {
            return (Mathf.Abs((Screen.width * 1.0f / Screen.height * 1.0f) - (800f / 480f)) <= 0.01f);
        }

        public static int GetGMTTime()
        {
            return System.DateTimeOffset.Now.Offset.Hours;
        }

        public static string GetGMTTimeWithStringFormat()
        {
            int gmtOffset = Mathf.Abs(GetGMTTime() + 12);

            string result = gmtOffset.ToString();

            return result;
        }

        public static string GetTimeZoneAnalytic()
        {
            System.DateTime date = System.DateTime.Now;
            int totalSecondInDay = ((date.Hour * 60) + date.Minute) * 60 + date.Second;
            totalSecondInDay = Mathf.RoundToInt(totalSecondInDay * 1f / 1800f) * 1800;
            int hour = totalSecondInDay / 3600;
            int minute = (totalSecondInDay / 60) - (hour * 60);

            string hourStr = hour > 9 ? hour.ToString() : CreateStrings("0", hour.ToString());
            string minStr = minute > 9 ? minute.ToString() : CreateStrings("0", minute.ToString());

            return CreateStrings(hourStr, "_", minStr);
        }

        // Check connection
        public static void CheckConnection(MonoBehaviour observer, System.Action<bool> callback)
        {
            observer.StartCoroutine(CheckConnectionCoroutine(callback));
        }

        private static IEnumerator CheckConnectionCoroutine(System.Action<bool> callback)
        {
            string url = "https://www.google.com";
            WWW www = new WWW(url);

            yield return www;

            bool isSuccessful = false;

            if (www.isDone && www.bytesDownloaded > 0)
            {
                isSuccessful = true;
            }
            if (www.isDone && www.bytesDownloaded == 0)
            {
                isSuccessful = false;
            }

            callback(isSuccessful);
        }

        // simple check connection
        public static bool CheckInternetConnection()
        {
            return (Application.internetReachability != NetworkReachability.NotReachable);
        }
        //

        /// <summary>
        /// Gets the android sdk version.
        /// </summary>
        /// <returns>The android sdk version.</returns>
        public static int GetAndroidSdkVersion()
        {
            int sdkVersion = -1;

#if UNITY_EDITOR
            sdkVersion = -1;
#elif UNITY_ANDROID
        using(var buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            sdkVersion = buildVersion.GetStatic<int>("SDK_INT");
        }
#endif

            return sdkVersion;
        }

        /// <summary>
        /// Deletes the list object and free memory.
        /// </summary>
        /// <param name="list">List.</param>
        public static void DeleteListObject<T>(List<T> list)
        {
            //list.Clear();
            //list.Capacity = 0;
            list = null;
        }

        /// <summary>
        /// Check object exist in array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="value">Value.</param>
        /// <param name="comparer">Comparer.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static int ArrayExistObject<T>(T[] array, System.Func<T, bool> comparer)
        {
            int result = -1;
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                if (comparer(array[i]) == true)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Draws the debug point.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="distance">Distance.</param>
        public static void DrawDebugPoint(Vector3 position, Color color, float distance = 100f, float duration = 300f)
        {
#if UNITY_EDITOR
            Vector3[] startPoints = new Vector3[]
                {
                position + Vector3.up * distance * 0.5f,
                position + Vector3.left * distance * 0.5f,
                position + Vector3.back * distance * 0.5f,
                };

            Vector3[] endPoints = new Vector3[]
                {
                position + Vector3.down * distance * 0.5f,
                position + Vector3.right * distance * 0.5f,
                position + Vector3.forward * distance * 0.5f,
                };

            //Debug.Log(string.Format("<>===### Draw Position: {0}", position));
            for (int i = 0; i < startPoints.Length; i++)
            {
                Debug.DrawLine(startPoints[i], endPoints[i], color, duration);
            }
#endif
        }

        /// <summary>
        /// Gets the copy of.
        /// </summary>
        /// <returns>The copy of.</returns>
        /// <param name="comp">Comp.</param>
        /// <param name="other">Other.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        /// <summary>
        /// Gets the screen point.
        /// </summary>
        /// <returns>The screen point.</returns>
        /// <param name="worldPos">World position.</param>
        /// <param name="mainCamera">Main camera.</param>
        public static Vector2 GetScreenPoint(Vector3 worldPos, Camera mainCamera = null)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            Vector3 pos2D = mainCamera.WorldToScreenPoint(worldPos);
            if (pos2D.z < 0)
            {
                // FLIP 2D pos if z < 0
                pos2D *= -1f;
            }

            pos2D.z = 0f;

            return pos2D;
        }

        /// <summary>
        /// Checks the in screen.
        /// </summary>
        /// <returns><c>true</c>, if in screen was checked, <c>false</c> otherwise.</returns>
        /// <param name="position">Position.</param>
        /// <param name="mainCamera">Main camera.</param>
        /// <param name="horOffset">Hor offset.</param>
        /// <param name="verOffset">Ver offset.</param>
        public static bool CheckInScreen(Vector3 position, Camera mainCamera = null, float horOffset = 0f, float verOffset = 0f)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            Vector3 posViewport = mainCamera.WorldToViewportPoint(position);

            bool onScreen = posViewport.z > 0
                                       && posViewport.x > (0 - horOffset)
                                       && posViewport.x < (1 + horOffset)
                                       && posViewport.y > (0 - verOffset)
                                       && posViewport.y < (1 + verOffset);

            return onScreen;
        }

        /// <summary>
        /// Checks the rec transform into screen.
        /// </summary>
        /// <returns><c>true</c>, if rec transform into screen was checked, <c>false</c> otherwise.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="mainCanvas">Main canvas.</param>
        public static bool CheckRecTransformIntoScreen(RectTransform rectTransform, Canvas mainCanvas)
        {
            bool result = false;

            // Check four points of recttransform
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            //result = rec.Overlaps(canvasRect);
            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 viewportPoint = mainCanvas.worldCamera.WorldToViewportPoint(corners[i]);

                if (viewportPoint.x >= 0f && viewportPoint.x <= 1f
                    && viewportPoint.y >= 0f && viewportPoint.y <= 1f)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Detect angle between vectors, result is range (0 -> 360)
        /// </summary>
        /// <param name="fromVector">From vector</param>
        /// <param name="toVector">To vector</param>
        /// <param name="checkSideUnit"> Detect side to clamp to 360 range. \nIf checkSideUnit == Vector3.Zero, checkSideUnit = Vector3.right </param>
        /// <returns></returns>
        public static float GetAngleBetweenTwoVector3In360Type(Vector3 fromVector, Vector3 toVector, Vector3 checkSideUnit)
        {
            if (checkSideUnit.x == 0f && checkSideUnit.y == 0f && checkSideUnit.z == 0f)
            {
                checkSideUnit = Vector3.right;
            }

            float angle = Vector3.Angle(fromVector, toVector);
            float result = (Vector3.Angle(checkSideUnit, toVector) > 90f) ? 360f - angle : angle;

            return result;
        }

        /// <summary>
        /// Gets the is touch on GUI.
        /// </summary>
        /// <returns><c>true</c>, if is touch on GUI was gotten, <c>false</c> otherwise.</returns>
        public static bool GetIsTouchOnGUI()
        {
            bool result = false;
#if UNITY_EDITOR
            // Check if the mouse was clicked over a UI element
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                result = true;
            }
#else
        // Check if there is a touch
        if (Input.touchCount > 0)
        {
            int count = Input.touchCount;
            for (int i = 0; i < count; i++)
            {
                // Check if finger is over a UI element
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    result = true;
                    break;
                }
            }
        }
#endif
            return result;
        }

        /// <summary>
        /// Gets the is touch on raycast target GUI.
        /// </summary>
        /// <returns><c>true</c>, if is touch on raycast target GUI was gotten, <c>false</c> otherwise.</returns>
        public static bool GetIsTouchOnRaycastTargetGUI()
        {
            bool result = false;
            //#if UNITY_EDITOR
            //        // Check if the mouse was clicked over a UI element
            //        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            //        {
            //            //if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
            //            //{
            //            //    result = true;
            //            //}
            //            return true;
            //        }
            //#else
            //        // Check if there is a touch
            //        if (Input.touchCount > 0)
            //        {
            //            int count = Input.touchCount;
            //            for (int i = 0; i < count; i++)
            //            {
            //                // Check if finger is over a UI element
            //                //Debug.Log("<>==### AAAIII: " + UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId));

            //                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
            //                {
            //                    //if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
            //                    //{
            //                    //    result = true;
            //                    //}
            //                    //break;
            //                    return true;
            //                }
            //            }
            //        }
            //#endif
            //return result;

            UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    var img = results[i].gameObject.GetComponent<UnityEngine.UI.Image>();
                    if (img != null)
                    {
                        /*Debug.Log(string.Format("<>==### name: {0}, Image: {1}, raycastTarget: {2}", 
                            results[i].gameObject.name,
                            results[i].gameObject.GetComponent<UnityEngine.UI.Image>(),
                            results[i].gameObject.GetComponent<UnityEngine.UI.Image>().raycastTarget));*/

                        if (results[i].gameObject.GetComponent<UnityEngine.UI.Image>().raycastTarget)
                        {
                            return true;
                        }
                    }

                    /*Debug.Log("<>==### name: " + results[i].gameObject.name + " button: " + results[i].gameObject.GetComponent<UnityEngine.UI.Button>());
                    if (results[i].gameObject.transform.parent != null
                        && results[i].gameObject.transform.parent.GetComponent<UnityEngine.UI.Button>() != null)
                    {
                        return true;
                    }*/


                }
            }


            return false;
        }

        /// <summary>
        /// Ises the left for vector 2d.
        /// </summary>
        /// <returns><c>true</c>, if left for vector2 d was ised, <c>false</c> otherwise.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public static bool IsLeftForVector2D(Vector2 from, Vector2 to)
        {
            return (-from.x * to.y + from.y * to.x < 0);
        }

        /// <summary>
        /// Get2s the DR otation in360.
        /// </summary>
        /// <returns>The DR otation in360.</returns>
        /// <param name="vector">Vector.</param>
        public static float Get2DRotationIn360(Vector3 vector, bool inverserX = false)
        {
            float inverseUnit = (inverserX == true) ? -1 : 1;
            float rot = (Mathf.Atan2(vector.y, vector.x * inverseUnit) * Mathf.Rad2Deg);

            if (rot < 0)
            {
                rot = rot + 360f;
            }

            return rot;
        }

        /// <summary>
        /// Gets the euler in180.
        /// </summary>
        /// <returns>The euler in180.</returns>
        /// <param name="euler">Euler.</param>
        public static Vector3 ConvertEulerTo180Limit(Vector3 euler)
        {
            Vector3 result = euler;
            if (result.x > 180f)
            {
                result.x -= 360f;
            }

            if (result.y > 180f)
            {
                result.y -= 360f;
            }

            if (result.z > 180f)
            {
                result.z -= 360f;
            }

            return result;
        }

        #region Enum util
        /// <summary>
        /// Gets the enum string value of.
        /// </summary>
        /// <returns>The enum string value of.</returns>
        /// <param name="value">Value.</param>
        public static string GetEnumStringValueOf(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            System.ComponentModel.DescriptionAttribute[] attributes = (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// Gets the enum value of.
        /// </summary>
        /// <returns>The enum value of.</returns>
        /// <param name="value">Value.</param>
        /// <param name="enumType">Enum type.</param>
        public static object GetEnumValueOf(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (GetEnumStringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException(string.Format("The string is not a description or value of the specified enum. value: {0} enemuType: {1}", value, enumType.ToString()));
        }
        #endregion

        /// <summary>
        /// Gets the range of splat vectors.
        /// </summary>
        /// <returns>The range of splat vectors.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="splatAxis">Splat axis.</param>
        /// <param name="isSqrt">If set to <c>true</c> is sqrt.</param>
        public static float GetRangeOfSplatVectors(ref Vector3 from, ref Vector3 to, Vector3 splatAxis, bool isSqrt = true)
        {
            if (splatAxis.x == 0f)
            {
                from.x = 0f;
                to.x = 0;
            }
            if (splatAxis.y == 0)
            {
                from.y = 0f;
                to.y = 0f;
            }
            if (splatAxis.z == 0f)
            {
                from.z = 0f;
                to.z = 0f;
            }

            float distance = 0f;
            if (isSqrt)
            {
                distance = (to - from).sqrMagnitude;
            }
            else
            {
                distance = (to - from).magnitude;
            }

            return distance;
        }

        /// <summary>
        /// Gets the range of splat vectors. Not assign old positions
        /// </summary>
        /// <returns>The range of splat vectors.</returns>
        public static float GetRangeOfSplatVectors(Vector3 from, Vector3 to, Vector3 splatAxis, bool isSqrt = true)
        {
            if (splatAxis.x == 0f)
            {
                from.x = 0f;
                to.x = 0;
            }
            if (splatAxis.y == 0)
            {
                from.y = 0f;
                to.y = 0f;
            }
            if (splatAxis.z == 0f)
            {
                from.z = 0f;
                to.z = 0f;
            }

            float distance = 0f;
            if (isSqrt)
            {
                distance = (to - from).sqrMagnitude;
            }
            else
            {
                distance = (to - from).magnitude;
            }

            return distance;
        }

        /// <summary>
        /// Checks the point in eclip. 2D coordinate
        /// F(x,y) = x²/a² + y²/b² - 1 = 0 
        /// result == 0 : Point on Eclip
        /// result < 0 : Eclip containt point
        /// result > 0 : point out Eclip
        /// </summary>
        /// <returns><c>true</c>, if point in eclip was checked, <c>false</c> otherwise.</returns>
        /// <param name="centerX">Center x.</param>
        /// <param name="centerY">Center y.</param>
        /// <param name="maxR">Max r.</param>
        /// <param name="minR">Minimum r.</param>
        /// <param name="pointX">Point x.</param>
        /// <param name="pointY">Point y.</param>
        /// <param name="result">Result.</param>
        public static bool CheckPointInEclip(float centerX, float centerY, float maxR, float minR, float pointX, float pointY, out float result)
        {
            float sqrtX = (pointX - centerX) * (pointX - centerX);
            float sqrtY = (pointY - centerY) * (pointY - centerY);
            float sqrtA = maxR * maxR;
            float sqrtB = minR * minR;

            result = (sqrtX / sqrtA) + (sqrtY / sqrtB) - 1;

            return (result <= 0f);
        }

        /// <summary>
        /// Get Value If Enum By Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumName">name of enum</param>
        /// <returns></returns>
        public static T GetValueEnumByName<T>(string enumName)
        {
            return (T)System.Enum.Parse(typeof(T), enumName, true);
        }

        public static string GetCurrency1kFormat(int money)
        {
            string result = money.ToString();
            if (money >= 1000)
            {
                //CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                //result = String.Format(elGR, "{0:0,0}", money);
                result = String.Format(CultureInfo.InvariantCulture,
                                     "{0:0,0}", money);
            }
            return result;
        }
        
        /// <summary>
        /// Conver from world pos to screen space camera gui pos
        /// </summary>
        /// <param name="convertObject"></param>
        /// <param name="canvasRect"></param>
        /// <param name="worldPos"></param>
        /// <param name="mainCamera"></param>
        public static void ConvertFromWorldToScreenSpaceGUI(RectTransform convertObject, RectTransform canvasRect,
            Vector3 worldPos, Camera mainCamera)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos );
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, mainCamera,
                out localPoint);
            
            convertObject.localPosition = localPoint;
        }
    }
}