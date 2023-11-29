// PathBuilderEditor.cs

using System.Linq;
using UnityEditor;
using UnityEngine;
using Runtime.Crowd;

namespace Editor.Crowd
{
    [CustomEditor(typeof(CrowdPath))]
    public class CrowdPathEditor : UnityEditor.Editor
    {
        private CrowdPath m_CrowdPath;
        private static bool isLocked;
        private SerializedProperty m_WaypointsProp;

        private void OnEnable()
        {
            m_CrowdPath = (CrowdPath)target;
            m_WaypointsProp = serializedObject.FindProperty("waypoints");
        }

        private void OnSceneGUI()
        {
            if (!m_CrowdPath.isInEditMode || Event.current.type != EventType.MouseDown || Event.current.button != 0)
                return;

            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return;

            Undo.RecordObject(m_CrowdPath, "Add Point");
            m_CrowdPath.AddPoint(hit.point);
            Event.current.Use();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_WaypointsProp);


            if (GUILayout.Button(m_CrowdPath.isInEditMode ? "Exit Edit Mode" : "Enter Edit Mode"))
            {
                ToggleEditMode();
            }

            if (m_CrowdPath.isInEditMode) return;
            if (GUILayout.Button("Clear Points"))
            {
                ClearPoints();
            }

            if (GUILayout.Button("Refresh"))
            {
                m_CrowdPath.waypoints = m_CrowdPath.GetComponentsInChildren<CrowdPathPoint>()
                    .Select(point => point.gameObject).ToList();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ToggleEditMode()
        {
            m_CrowdPath.isInEditMode = !m_CrowdPath.isInEditMode;

            if (m_CrowdPath.isInEditMode)
            {
                LockInspector();
            }
            else
            {
                UnlockInspectorAndSelectObject();
            }
        }

        private void ClearPoints()
        {
            foreach (var point in m_CrowdPath.waypoints.Where(point => point != null))
            {
                DestroyImmediate(point);
            }

            m_CrowdPath.waypoints.Clear();
            m_CrowdPath.isInEditMode = false;
            UnlockInspectorAndSelectObject();
        }

        private void UnlockInspectorAndSelectObject()
        {
            UnlockInspector();

            if (m_CrowdPath != null)
            {
                Selection.activeObject = m_CrowdPath.gameObject;
            }
        }

        private static void LockInspector()
        {
            if (isLocked) return;

            ActiveEditorTracker.sharedTracker.isLocked = true;
            isLocked = true;
        }

        private static void UnlockInspector()
        {
            if (!isLocked) return;

            ActiveEditorTracker.sharedTracker.isLocked = false;
            isLocked = false;
        }
    }
}