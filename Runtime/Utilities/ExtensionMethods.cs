using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace MobileFramework.Core.Utilities
{
    public static class ExtensionMethods
    {
        // --- Transform ---

        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void DestroyChildren(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        // --- Vector ---

        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);

        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        // --- string ---

        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || maxLength <= 0)
            {
                return string.Empty;
            }

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        // --- collezioni ---

        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            random ??= new Random();
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static T RandomElement<T>(this IReadOnlyList<T> list, Random random = null)
        {
            if (list == null || list.Count == 0)
            {
                throw new InvalidOperationException("RandomElement su lista vuota.");
            }

            random ??= new Random();
            return list[random.Next(list.Count)];
        }

        // --- float ---

        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return Mathf.Approximately(fromMax, fromMin)
                ? toMin
                : toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
    }
}
