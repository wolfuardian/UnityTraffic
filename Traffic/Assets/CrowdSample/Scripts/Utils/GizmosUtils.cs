using UnityEngine;

namespace CrowdSample.Scripts.Utils
{
    public static class GizmosUtils
    {
        public static void Arrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 2.5f,
            float                        arrowHeadAngle = 20.0f)
        {
            if (direction == Vector3.zero) return; // 檢查並跳過零向量

            Gizmos.DrawRay(pos, direction);

            var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                        new Vector3(0, 0, 1);
            var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                       new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void Arrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 2.5f,
            float                        arrowHeadAngle = 20.0f)
        {
            if (direction == Vector3.zero) return; // 檢查並跳過零向量

            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                        new Vector3(0, 0, 1);
            var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                       new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void Polygon(Vector3 pos, Color color, float radius = 1f, int segments = 360)
        {
            Gizmos.color = color;

            var previousPoint = pos + new Vector3(radius, 0, 0);

            for (var i = 1; i <= segments; i++)
            {
                var angle = (float)i / segments * 2 * Mathf.PI;

                var currentPoint = pos + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                Gizmos.DrawLine(previousPoint, currentPoint);

                previousPoint = currentPoint;
            }
        }

        public static void ThicknessArrow(Vector3 pos, Vector3 direction, Color color, float scale = 1f)
        {
            Gizmos.color = color;

            var arrowLength     = 1.0f;
            var arrowThickness  = 0.2f;
            var arrowHeadLength = 0.5f;
            var arrowHeadWidth  = 0.4f;

            arrowLength     *= scale;
            arrowThickness  *= scale;
            arrowHeadLength *= scale;
            arrowHeadWidth  *= scale;

            var up    = Vector3.Cross(direction, Vector3.right).normalized * arrowThickness / 2;
            var right = Vector3.Cross(direction, up).normalized * arrowThickness / 2;

            var bodyEnd = pos + direction * (arrowLength - arrowHeadLength);

            var bodyPoints = new Vector3[]
            {
                pos - right,
                pos + right,
                bodyEnd + right,
                bodyEnd - right,
                pos - right
            };

            for (var i = 0; i < bodyPoints.Length - 1; i++)
            {
                if (i == 2) continue;
                Gizmos.DrawLine(bodyPoints[i], bodyPoints[i + 1]);
            }

            var headTip = bodyEnd + direction * arrowHeadLength;

            var rightTr        = Vector3.Cross(direction, up).normalized;
            var headWidthRight = bodyEnd + rightTr * arrowHeadWidth / 2;
            var headWidthLeft  = bodyEnd - rightTr * arrowHeadWidth / 2;

            Gizmos.DrawLine(headWidthLeft,   headTip);
            Gizmos.DrawLine(headTip,         headWidthRight);
            Gizmos.DrawLine(headWidthRight,  bodyEnd + right);
            Gizmos.DrawLine(bodyEnd - right, headWidthLeft);
        }

        public static void ThicknessArrow(Vector3 pos, Vector3 direction, Color color, Vector2 scale)
        {
            Gizmos.color = color;

            var arrowLength     = 1.0f;
            var arrowThickness  = 0.2f;
            var arrowHeadLength = 0.5f;
            var arrowHeadWidth  = 0.4f;

            arrowLength     *= scale.x;
            arrowThickness  *= scale.y;
            arrowHeadLength *= scale.x;
            arrowHeadWidth  *= scale.y;

            var up    = Vector3.Cross(direction, Vector3.right).normalized * arrowThickness / 2;
            var right = Vector3.Cross(direction, up).normalized * arrowThickness / 2;

            var bodyEnd = pos + direction * (arrowLength - arrowHeadLength);

            var bodyPoints = new Vector3[]
            {
                pos - right,
                pos + right,
                bodyEnd + right,
                bodyEnd - right,
                pos - right
            };

            for (var i = 0; i < bodyPoints.Length - 1; i++)
            {
                if (i == 2) continue;
                Gizmos.DrawLine(bodyPoints[i], bodyPoints[i + 1]);
            }

            var headTip = bodyEnd + direction * arrowHeadLength;

            var rightTr        = Vector3.Cross(direction, up).normalized;
            var headWidthRight = bodyEnd + rightTr * arrowHeadWidth / 2;
            var headWidthLeft  = bodyEnd - rightTr * arrowHeadWidth / 2;

            Gizmos.DrawLine(headWidthLeft,   headTip);
            Gizmos.DrawLine(headTip,         headWidthRight);
            Gizmos.DrawLine(headWidthRight,  bodyEnd + right);
            Gizmos.DrawLine(bodyEnd - right, headWidthLeft);
        }
    }
}