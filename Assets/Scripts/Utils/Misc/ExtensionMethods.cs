using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace TDB.Utils.Misc
{
    public static class ExtensionMethods
    {
        private static Random rng = new Random();
        
        public static T RandomSampleByWeight<T>(this List<T> list, Func<T, float> weight)
        {
            float totalWeight = list.Sum(weight);

            if (totalWeight <= 0f) return default(T);

            float randomValue = UnityEngine.Random.Range(0, totalWeight);

            T result = list.Last();

            float cumulative = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                cumulative += weight(list[i]);
                if (randomValue <= cumulative)
                {
                    result = list[i];
                    break;
                }
            }
            
            return result;
        }

        public static List<int> GetAllCode()
        {
            var result = new List<int>();

            for (int i = 0; i < 81; i++)
            {
                string ternary = ConvertToTernary(i).PadLeft(4, '0');

                if (ternary.Contains('0') && ternary.Contains('1') && ternary.Contains('2'))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        static string ConvertToTernary(int n)
        {
            if (n == 0) return "0";
            string result = "";
            while (n > 0)
            {
                result = (n % 3) + result;
                n /= 3;
            }
            return result;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
        
        public static IEnumerable<T> Shuffled<T>(this IList<T> list)
        {
            var newList = new List<T>(list);
            newList.Shuffle();
            return newList;
        }
        
        public static bool IsAncestorOf(this Transform ancestor, Transform child)
        {
            Transform current = child;
            while (current != null)
            {
                if (current == ancestor)
                    return true;
                current = current.parent;
            }
            return false;
        }
        
#if UNITY_EDITOR
        public static List<T> GetChildScriptableObjects<T>(this ScriptableObject parent) where T : ScriptableObject
        {
            if (parent == null) return new List<T>();

            // Get the asset path of the parent ScriptableObject
            string assetPath = AssetDatabase.GetAssetPath(parent);
            if (string.IsNullOrEmpty(assetPath)) return new List<T>();

            // Load all assets at this path
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            // Filter only ScriptableObjects that are not the parent itself
            return assets.OfType<T>().Where(obj => obj != parent).ToList();
        }
#endif

        public static Vector3 WorldToScreenPoint(this Vector3 worldPos, Camera camera)
        {
            return camera.WorldToScreenPoint(worldPos);
        }
        
        /// <summary>
        /// Returns true if value is non-empty and is fully contained in mask.
        /// </summary>
        public static bool Contains<T>(this T value, T flag) where T : Enum
        {
            var valueAsByte = Convert.ToByte(value);
            var flagAsByte = Convert.ToByte(flag);
            return flagAsByte != 0 && (valueAsByte & flagAsByte) == flagAsByte;
        }
        
        public static List<int> FindAllIndices<T>(this List<T> list, System.Predicate<T> match)
        {
            return Enumerable.Range(0, list.Count)
                    .Where(i => match(list[i]))
                    .ToList();
        }

        public static Color SetAlpha(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }

        public static Vector3 ToVector3(this Vector2 v2, float z)
        {
            return new Vector3(v2.x, v2.y, z);
        }

        public static Vector3 ToVector3(this Vector2 v2)
        {
            return new Vector3(v2.x, v2.y, 0);
        }

        public static Vector2 ToVector2(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public static Vector3 SetZ(this Vector3 v3, float z)
        {
            v3.z = z;
            return v3;
        }
        public static Vector3 SetY(this Vector3 v3, float y)
        {
            v3.y = y;
            return v3;
        }
        public static Vector3 SetX(this Vector3 v3, float x)
        {
            v3.x = x;
            return v3;
        }
        
        public static Vector2 SetY(this Vector2 v2, float y)
        {
            v2.y = y;
            return v2;
        }
        public static Vector2 SetX(this Vector2 v2, float x)
        {
            v2.x = x;
            return v2;
        }

        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }

        public static Vector2 GetAveragePoint(this Collision2D collision)
        {
            Vector2 point = Vector2.zero;

            int contactCount = collision.contactCount;
            for (int i = 0; i < contactCount; i++)
            {
                var contact = collision.GetContact(i);
                point += contact.point;
            }

            return point / contactCount;
        }

        public static Vector2 GetTotalImpulse(this Collision2D collision)
        {
            Vector2 impulse = Vector2.zero;

            int contactCount = collision.contactCount;
            for (int i = 0; i < contactCount; i++)
            {
                var contact = collision.GetContact(i);
                impulse += contact.normal * contact.normalImpulse;
                //impulse.x += contact.tangentImpulse * contact.normal.y;
                //impulse.y -= contact.tangentImpulse * contact.normal.x;
            }

            return impulse;
        }

        public static float GetYImpulse(this Collision2D collision)
        {
            float impulse = 0;

            int contactCount = collision.contactCount;
            for (int i = 0; i < contactCount; i++)
            {
                var contact = collision.GetContact(i);
                impulse += contact.normal.y * contact.normalImpulse;
            }

            return impulse;
        }

        public static List<Vector2> GetImpulses(this Collision2D collision)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);

            return contacts.Select(c => c.normal).ToList();
        }

        public static float Dot(this Vector2 v1, Vector2 v2)
        {
            return Vector2.Dot(v1, v2);
        }

        public static float Cross(this Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        public static Vector2 Multiply(this Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }

        public static float Sign(this float v)
        {
            return v == 0 ? 0 : Mathf.Sign(v);
        }

        public static float GetLength(this LineRenderer line)
        {
            Vector3[] vertices = new Vector3[line.positionCount];
            line.GetPositions(vertices);
            float length = 0f;
            for(int i=1; i<line.positionCount; i++)
            {
                length += (vertices[i] - vertices[i - 1]).magnitude;
            }
            return length;
        }

        public static T Top<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }
    }
}
