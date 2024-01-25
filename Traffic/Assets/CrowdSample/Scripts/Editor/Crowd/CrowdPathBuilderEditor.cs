using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdPathBuilder))]
    public class CrowdPathBuilderEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private CrowdPathBuilder   crowdPathBuilder;
        private SerializedProperty waypointsProp;

        private bool isConfigPanelExpanded;

        #endregion

        #region Private Methods

        private void HandleSceneClickAddWaypoint()
        {
            if (crowdPathBuilder.editMode != CrowdPathBuilder.EditMode.Add ||
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
            if (crowdPathBuilder.editMode != CrowdPathBuilder.EditMode.Add) return;

            var path           = crowdPathBuilder.CrowdPath;
            var parent         = crowdPathBuilder.transform;
            var newWaypoint    = UnityUtils.CreatePoint("Waypoint" + path.Waypoints.Count, hitPoint, parent);
            var waypointGizmos = newWaypoint.gameObject.AddComponent<WaypointGizmos>();
#if UNITY_EDITOR
            InternalEditorUtility.SetIsInspectorExpanded(waypointGizmos, true);
#endif
            path.Waypoints.Add(newWaypoint);
        }

        private void ToggleEditMode()
        {
            var editModes = Enum.GetValues(typeof(CrowdPathBuilder.EditMode));
            var editMode  = crowdPathBuilder.editMode;
            editMode = (CrowdPathBuilder.EditMode)(((int)editMode + 1) % editModes.Length);
            switch (editMode)
            {
                case CrowdPathBuilder.EditMode.None:
                    UnityEditorUtils.SetInspectorLock(false);
                    break;
                case CrowdPathBuilder.EditMode.Add:
                    UnityEditorUtils.SetInspectorLock(true);
                    break;
            }

            Selection.activeObject    = crowdPathBuilder.gameObject;
            crowdPathBuilder.editMode = editMode;
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            try
            {
                crowdPathBuilder = (CrowdPathBuilder)target;
                serializedObject.FindProperty("crowdPath");
                waypointsProp = serializedObject.FindProperty("waypoints");
            }
            catch (Exception)
            {
                // ignored 找不到原因，只好先這樣處理
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            serializedObject.Update();

            DrawEditModeSwitch();

            var isLockInspectorInEditing = crowdPathBuilder.editMode == CrowdPathBuilder.EditMode.Add;
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

        #region GUI Methods

        private void DrawEditModeSwitch()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);
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
            var modeLabel = crowdPathBuilder.editMode switch
            {
                CrowdPathBuilder.EditMode.None => "Current Mode: None",
                CrowdPathBuilder.EditMode.Add => "Current Mode: Add Mode",
                _ => "Unknown Mode"
            };
            GUILayout.Label(modeLabel, customStyle, GUILayout.Height(24f));

            if (crowdPathBuilder.editMode == CrowdPathBuilder.EditMode.Add)
            {
                EditorGUILayout.HelpBox("點擊場景中的位置來新增航點。", MessageType.Info);
            }
        }

        private void DrawActionsSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset All Waypoints"))
            {
                var waypoints = crowdPathBuilder.CrowdPath.Waypoints.Where(point => point != null);

                UnityUtils.ClearPoints(waypoints);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawPointConfigSection()
        {
            var headerStyle = UnityEditorUtils.CreateHeaderStyle(FontStyle.Bold, 12);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Point Config", headerStyle);
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

            try
            {
                for (var i = 0; i < waypointsProp.arraySize; i++)
                {
                    var waypoint = waypointsProp.GetArrayElementAtIndex(i);
                    if (waypoint.objectReferenceValue == null) continue; // 跳過已經被刪除的 waypoint, 防止介面卡住
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(waypoint, GUIContent.none);

                    if (waypoint.objectReferenceValue is Transform waypointTr)
                    {
                        var component = waypointTr.GetComponent<Waypoint>();
                        if (component != null)
                        {
                            DrawWaypointsConfig(component);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            catch (Exception)
            {
                // ignored 找不到原因，只好先這樣處理
            }


            EditorGUI.indentLevel--;
        }

        private void DrawWaypointsConfig(Component component)
        {
            var waypointSo    = new SerializedObject(component);
            var pathBuilderSo = new SerializedObject(crowdPathBuilder);
            waypointSo.Update();

            var radiusProp = waypointSo.FindProperty("radius");
            EditorGUILayout.LabelField("Radius", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f));
            EditorGUILayout.PropertyField(radiusProp, GUIContent.none,
                GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f));
            if (GUI.changed)
            {
                waypointSo.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
            {
                UnityUtils.DeleteItem(component);
                pathBuilderSo.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}