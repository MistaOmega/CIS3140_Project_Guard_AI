/*
 * Common Utility functions and variables for Unity
 */

using UnityEngine;

namespace Utilities
{
    public static class Utils
    {
        private static readonly Vector3 Vector3Zero = Vector3.zero;
        private static readonly Vector3 Vector3One = Vector3.one;

        public static Font GetDefaultFont()
        {
            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        public static Vector3 GetRandomDirectionVector2D()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        public static Vector3 GetRandomDirectionVector3D()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        public static Vector3 GetRandomDirectionVector3DFixedY(float y)
        {
            return new Vector3(Random.Range(-1f, 1f), y, Random.Range(-1f, 1f)).normalized;
        }
    }
}