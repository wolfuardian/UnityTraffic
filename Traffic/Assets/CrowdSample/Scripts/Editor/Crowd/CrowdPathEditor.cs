using System;
using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdPath))]
    public class CrowdPathEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private CrowdPath          crowdPath;
        private SerializedProperty waypointsProp;

        private bool isConfigPanelExpanded;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdPath     = (CrowdPath)target;
            waypointsProp = serializedObject.FindProperty("m_waypoints");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawEditModeSwitch();

            var isLockInspectorInEditing = crowdPath.editMode == CrowdPath.EditMode.Add;
            EditorGUI.BeginDisabledGroup(isLockInspectorInEditing);

            DrawActionsSection();
            DrawPointConfigSection();

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            HandleSceneClickAddWaypoint();
        }

        #endregion

        #region Private Methods

        private void HandleSceneClickAddWaypoint()
        {
            if (crowdPath.editMode != CrowdPath.EditMode.Add ||
                !UnityUtils.IsLeftMouseButtonDown())
            {
                return;
            }

            OnSceneClickAddWaypoint();
            Event.current.Use();
        }

        private void OnSceneClickAddWaypoint()
        {
            if (!UnityUtils.TryGetRaycastHit(out var hitPoint)) return;
            if (crowdPath.editMode != CrowdPath.EditMode.Add) return;

            var parent       = crowdPath.transform;
            var waypointInst = UnityUtils.CreatePoint("Waypoint" + crowdPath.waypoints.Count, hitPoint, parent);
            var waypoint     = waypointInst.gameObject.AddComponent<Waypoint>();
            crowdPath.waypoints.Add(waypoint);
        }

        private void ToggleEditMode()
        {
            var editModes = Enum.GetValues(typeof(CrowdPath.EditMode));
            var editMode  = crowdPath.editMode;

            editMode = (CrowdPath.EditMode)(((int)editMode + 1) % editModes.Length);

            crowdPath.editMode = editMode;

            switch (crowdPath.editMode)
            {
                case CrowdPath.EditMode.None:
                    UnityUtils.SetInspectorLock(false);
                    break;
                case CrowdPath.EditMode.Add:
                    UnityUtils.SetInspectorLock(true);
                    break;
            }

            Selection.activeObject = crowdPath.gameObject;
        }

        #endregion

        #region GUI Methods

        private void DrawEditModeSwitch()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("編輯模式", EditorStyles.boldLabel);
            DrawCurrentEditMode();

            if (GUILayout.Button("Toggle Mode", GUILayout.Height(48)))
            {
                ToggleEditMode();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCurrentEditMode()
        {
            var customStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 14 };
            var modeLabel = crowdPath.editMode switch
            {
                CrowdPath.EditMode.None => "Current Mode: None",
                CrowdPath.EditMode.Add => "Current Mode: Add Mode",
                _ => "Unknown Mode"
            };
            GUILayout.Label(modeLabel, customStyle, GUILayout.Height(24f));

            if (crowdPath.editMode == CrowdPath.EditMode.Add)
            {
                EditorGUILayout.HelpBox("點擊場景中的位置來新增航點。", MessageType.Info);
            }
        }

        private void DrawActionsSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("動作", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset All Waypoints"))
            {
                UnityUtils.ClearPoints(crowdPath.GetWaypointsTransform());
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawPointConfigSection()
        {
            var headerStyle = UnityUtils.CreateHeaderStyle(FontStyle.Bold, 12);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("路徑點設定", headerStyle);
            if (GUILayout.Button(isConfigPanelExpanded
                    ? "Close Waypoint Config Panel"
                    : "Open Waypoint Config Panel"))
            {
                isConfigPanelExpanded = !isConfigPanelExpanded;
            }

            if (isConfigPanelExpanded)
            {
                DrawWaypointsConfigPanel();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawWaypointsConfigPanel()
        {
            EditorGUI.indentLevel++;
            for (var i = 0; i < waypointsProp.arraySize; i++)
            {
                var waypoint = waypointsProp.GetArrayElementAtIndex(i);
                if (waypoint.objectReferenceValue == null) continue; // 跳過已經被刪除的 waypoint, 防止介面卡住
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(waypoint, GUIContent.none);
                if (waypoint.objectReferenceValue is Waypoint component)
                {
                    DrawWaypointsConfig(component);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawWaypointsConfig(Component waypoint)
        {
            var waypointSo    = new SerializedObject(waypoint);
            var pathBuilderSo = new SerializedObject(crowdPath);

            waypointSo.Update();

            var radiusProp = waypointSo.FindProperty("m_radius");
            EditorGUILayout.LabelField("Radius", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f));
            EditorGUILayout.PropertyField(radiusProp, GUIContent.none,
                GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f));
            if (GUI.changed)
            {
                waypointSo.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
            {
                UnityUtils.DeleteItem(waypoint);
                pathBuilderSo.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}