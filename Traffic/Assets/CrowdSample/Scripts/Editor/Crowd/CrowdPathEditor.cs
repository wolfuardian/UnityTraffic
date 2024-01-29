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
        private SerializedProperty arrowScaleProp;

        private bool configPanelExpanded;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdPath      = (CrowdPath)target;
            waypointsProp  = serializedObject.FindProperty("m_waypoints");
            arrowScaleProp = serializedObject.FindProperty("m_arrowScale");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawEditModeSwitch("編輯模式");

            var isLockInspectorInEditing = crowdPath.editMode == CrowdPath.EditMode.Add;

            EditorGUI.BeginDisabledGroup(isLockInspectorInEditing);

            DrawActionsSection("動作");

            DrawPointConfigSection("編輯路徑點");

            DrawingSettings("設定");

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            HandleClickOnScene();
        }

        #endregion

        #region Private Methods

        private void HandleClickOnScene()
        {
            if (crowdPath.editMode != CrowdPath.EditMode.Add ||
                !UnityUtils.IsLeftMouseButtonDown())
            {
                return;
            }

            ClickAddWaypointOnScene();
            Event.current.Use();
        }

        private void ClickAddWaypointOnScene()
        {
            if (!UnityUtils.TryGetRaycastHit(out var hitPoint)) return;
            if (crowdPath.editMode != CrowdPath.EditMode.Add) return;

            var parent       = crowdPath.transform;
            var waypointInst = UnityUtils.CreatePoint("Waypoint" + crowdPath.waypoints.Count, hitPoint, parent);
            var waypoint     = waypointInst.gameObject.AddComponent<Waypoint>();
            crowdPath.waypoints.Add(waypoint);
        }

        #endregion

        #region GUI Methods

        private void DrawEditModeSwitch(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
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

            if (GUILayout.Button("切換模式", GUILayout.Height(48)))
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

            EditorGUILayout.EndVertical();
        }

        private void DrawActionsSection(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("重設所有路徑點", GUILayout.Height(24)))
            {
                UnityUtils.ClearPoints(crowdPath.GetWaypointsTransform());
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawPointConfigSection(string label)
        {
            var headerStyle = UnityUtils.CreateHeaderStyle(FontStyle.Bold, 12);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, headerStyle);

            var defaultButtonColor = GUI.backgroundColor;
            var toggledButtonColor = new Color(0.7f, 0.7f, 0.7f);
            
            GUI.backgroundColor = configPanelExpanded ? toggledButtonColor : defaultButtonColor;
            if (GUILayout.Button(configPanelExpanded
                    ? "關閉面板"
                    : "開啟編輯面板", GUILayout.Height(24)))
            {
                configPanelExpanded = !configPanelExpanded;
            }

            GUI.backgroundColor = defaultButtonColor;

            if (configPanelExpanded)
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
                        var waypointSo    = new SerializedObject(component);
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
                            UnityUtils.DeleteItem(component);
                            pathBuilderSo.ApplyModifiedProperties();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawingSettings(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(arrowScaleProp, new GUIContent("圖示大小"));
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}