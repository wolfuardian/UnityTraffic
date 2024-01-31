using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Crowd
{
    public static class Misc
    {
        public static string GenerateLicensePlateNumber()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            var rnd = new System.Random();

            var plate = "";
            for (var i = 0; i < 3; i++)
            {
                plate += letters[rnd.Next(letters.Length)];
            }

            plate += "-";
            for (var i = 0; i < 4; i++)
            {
                plate += numbers[rnd.Next(numbers.Length)];
            }

            return plate;
        }

        public static Vector3 GetRandomPointInRadius(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                return center;
            }

            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection += center;
            return new Vector3(randomDirection.x, center.y, randomDirection.z);
        }

        public static Vector3 GetScatterPosition(Vector3 position, float radius)
        {
            var spawnPosition = GetRandomPointInRadius(position, radius);
            return spawnPosition;
        }
    }

    public static class GizmosUtils
    {
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

            var bodyPoints = new[]
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

    public interface IUpdateReceiver
    {
        void UpdateImmediately();
    }

    public class CrowdUtils
    {
        public static bool TryGetRaycastHit(out Vector3 hitPoint)
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                hitPoint = hit.point;
                return true;
            }

            hitPoint = Vector3.zero;
            return false;
        }

        public static bool IsLeftMouseButtonDown()
        {
            return Event.current.type == EventType.MouseDown && Event.current.button == 0;
        }

        public static void SelectGameObject(GameObject gameObject)
        {
            if (gameObject != null)
            {
                Selection.activeObject = gameObject;
            }
        }

        public static void SetupDefaultPoint(Component point)
        {
            var meshFilter = point.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");

            var meshRenderer = point.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }

        public static void DeleteItem(Component component)
        {
            if (component == null) return;

            Undo.RecordObject(component.gameObject, "Delete Item");
            Undo.DestroyObjectImmediate(component.gameObject);
        }

        public static void ClearPoints(IEnumerable<Transform> waypoints)
        {
            Undo.SetCurrentGroupName("Clear Path Points");
            foreach (var waypoint in waypoints)
            {
                if (waypoint == null) continue;
                Undo.DestroyObjectImmediate(waypoint.gameObject);
            }
        }

        public static Transform CreatePoint(string pointName, Vector3 position, Transform parent = null)
        {
            var newPoint = new GameObject(pointName).transform;

            newPoint.position = position;

            if (parent) newPoint.SetParent(parent);

            SetupDefaultPoint(newPoint);

            Undo.RegisterCreatedObjectUndo(newPoint.gameObject, "Create Waypoint");

            return newPoint;
        }

        public static void RemoveInstances(ICollection<GameObject> instances, List<GameObject> toRemove)
        {
            foreach (var remove in toRemove)
            {
                instances.Remove(remove);

                if (remove == null) continue;

                Object.DestroyImmediate(remove);
            }
        }

        public static void UpdateAllReceiverImmediately()
        {
            foreach (var updatable in Object.FindObjectsOfType<MonoBehaviour>().OfType<IUpdateReceiver>())
            {
                updatable.UpdateImmediately();
            }
        }

        public static void SetInspectorLock(bool lockState)
        {
            try
            {
                // Get the Inspector window type
                var inspectorWindowType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
                if (inspectorWindowType == null)
                {
                    throw new InvalidOperationException("Could not find InspectorWindow type.");
                }

                // Get the current Inspector window instance
                var inspectorWindow = EditorWindow.GetWindow(inspectorWindowType);
                if (inspectorWindow == null)
                {
                    throw new InvalidOperationException("Could not find an open Inspector window.");
                }

                // Use reflection to set the lock state
                var isLockedProperty =
                    inspectorWindowType.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
                if (isLockedProperty == null)
                {
                    throw new InvalidOperationException("Could not find 'isLocked' property in InspectorWindow type.");
                }

                isLockedProperty.SetValue(inspectorWindow, lockState, null);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error setting Inspector lock state: " + ex.Message);
            }
        }

        public static GUIStyle CreateHeaderStyle(FontStyle fontStyle, int fontSize)
        {
            var headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = fontStyle,
                fontSize  = fontSize,
                normal    = { textColor = EditorStyles.boldLabel.normal.textColor }
            };
            return headerStyle;
        }
    }
}