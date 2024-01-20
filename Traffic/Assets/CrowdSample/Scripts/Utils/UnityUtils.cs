using UnityEditor;
using UnityEngine;

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
    }
}