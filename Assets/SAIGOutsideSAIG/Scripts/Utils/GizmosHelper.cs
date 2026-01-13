using UnityEngine;

namespace Utils
{
    public static class GizmosHelper
    {
        /// <summary>
        /// Draws a ray (line with arrow) in the Scene view.
        /// </summary>
        /// <param name="origin">The starting point of the ray.</param>
        /// <param name="direction">The direction of the ray (should be normalized).</param>
        /// <param name="length">The length of the ray.</param>
        /// <param name="color">The color of the ray.</param>
        /// <param name="arrowHeadLength">The length of the arrow head.</param>
        /// <param name="arrowHeadAngle">The angle of the arrow head.</param>
        public static void DrawRay(Vector3 origin, Vector3 direction, float length, Color color, float arrowHeadLength = 0.2f, float arrowHeadAngle = 20f)
        {
            Vector3 end = origin + direction.normalized * length;
            DrawLine(origin, end, color);
            DrawArrow(origin, end, color, arrowHeadLength, arrowHeadAngle);
        }

        public static void DrawWireSphere(Vector3 position, float radius, Color color)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);
        Gizmos.color = oldColor;
    }

    public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = oldColor;
    }

    public static void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(from, to);
        Gizmos.color = oldColor;
    }

    public static void DrawArrow(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(from, to);

        Vector3 direction = to - from;
        if (direction == Vector3.zero) return;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
        Gizmos.DrawLine(to, to + right * arrowHeadLength);
        Gizmos.DrawLine(to, to + left * arrowHeadLength);
        Gizmos.color = oldColor;
    }
}
}
