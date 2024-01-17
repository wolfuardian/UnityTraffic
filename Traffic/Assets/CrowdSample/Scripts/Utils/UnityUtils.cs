using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Utils
{
    public class UnityUtils
    {
        public static bool LockInspector(bool isLocked)
        {
            if (isLocked) return true;
            ActiveEditorTracker.sharedTracker.isLocked = true;
            return true;
        }

        public static bool UnlockInspector(bool isLocked)
        {
            if (!isLocked) return false;
            ActiveEditorTracker.sharedTracker.isLocked = false;
            return false;
        }

        public static GUIStyle GetHeaderStyle(FontStyle fontStyle, int fontSize)
        {
            var headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = fontStyle, fontSize = fontSize,
                normal =
                {
                    textColor = EditorStyles.boldLabel.normal.textColor
                }
            };
            return headerStyle;
        }

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
    }
}