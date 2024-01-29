using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace CrowdSample.Scripts.Utils
{
    public class UnityUtils
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
                var inspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
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