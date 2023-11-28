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
        private SerializedProperty m_PointsProp;
        private SerializedProperty m_IsLoopingProp;
        private SerializedProperty m_hasGoalProp;
        private SerializedProperty m_pathModeProp;
        private SerializedProperty m_AgentSpeedProp;

        private void OnEnable()
        {
            m_CrowdPath = (CrowdPath)target;
            m_PointsProp = serializedObject.FindProperty("points");
            m_IsLoopingProp = serializedObject.FindProperty("isLooping");
            m_hasGoalProp = serializedObject.FindProperty("hasGoal");
            m_pathModeProp = serializedObject.FindProperty("pathMode");
            m_AgentSpeedProp = serializedObject.FindProperty("agentSpeed");
        }

        private void OnSceneGUI()
        {
            if (!m_CrowdPath.IsInEditMode || Event.current.type != EventType.MouseDown || Event.current.button != 0)
                return;

            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return;

            Undo.RecordObject(m_CrowdPath, "Add Point");
            m_CrowdPath.AddPoint(hit.point);
            Event.current.Use();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_PointsProp);

            if (GUILayout.Button(m_CrowdPath.IsInEditMode ? "Exit Edit Mode" : "Enter Edit Mode"))
            {
                ToggleEditMode();
            }

            if (m_CrowdPath.IsInEditMode) return;
            if (GUILayout.Button("Clear Points"))
            {
                ClearPoints();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Path Configuration", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_IsLoopingProp);
            EditorGUILayout.PropertyField(m_hasGoalProp);
            EditorGUILayout.PropertyField(m_pathModeProp);
            EditorGUILayout.PropertyField(m_AgentSpeedProp);

            serializedObject.ApplyModifiedProperties();
        }

        private void ToggleEditMode()
        {
            m_CrowdPath.IsInEditMode = !m_CrowdPath.IsInEditMode;

            if (m_CrowdPath.IsInEditMode)
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
            foreach (var point in m_CrowdPath.points.Where(point => point != null))
            {
                DestroyImmediate(point);
            }

            m_CrowdPath.points.Clear();
            m_CrowdPath.IsInEditMode = false;
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