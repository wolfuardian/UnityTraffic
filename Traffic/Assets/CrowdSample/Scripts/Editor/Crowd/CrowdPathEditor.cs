using System;
using System.Collections.Generic;
using System.Linq;
using CrowdSample.Scripts.Runtime.Crowd;
using CrowdSample.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdPath))]
    public class CrowdPathEditor : UnityEditor.Editor
    {
        private static bool               _isLocked;
        private        CrowdPath          _crowdPath;
        private        SerializedProperty _waypointsProp;

        private void OnEnable()
        {
            _crowdPath     = (CrowdPath)target;
            _waypointsProp = serializedObject.FindProperty("waypoints");
        }

        private void OnSceneGUI()
        {
            if (!(_crowdPath.isInEditMode && UnityUtils.IsLeftMouseButtonDown())) return;
            if (!UnityUtils.TryGetRaycastHit(out var hitPoint)) return;

            _crowdPath.AddPoint(hitPoint);
            Event.current.Use();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var headerGUIStyle = UnityUtils.GetHeaderStyle(FontStyle.Bold, 12);

            DrawAddModeSection(headerGUIStyle);
            DrawActionsSection(headerGUIStyle);
            DrawPointConfigSection(headerGUIStyle);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAddModeSection(GUIStyle headerStyle)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Mode", headerStyle);
            var customStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 14
            };
            switch (_crowdPath.editMode)
            {
                case CrowdPath.EditMode.None:
                    GUILayout.Label("當前模式: None", customStyle, GUILayout.Height(24f));
                    break;
                case CrowdPath.EditMode.Add:
                    GUILayout.Label("當前模式: Add Mode", customStyle, GUILayout.Height(24f));
                    break;
            }

            if (GUILayout.Button("切換模式", GUILayout.Height(48)))
            {
                ToggleEditMode();
            }

            if (_crowdPath.editMode == CrowdPath.EditMode.Add)
            {
                EditorGUILayout.HelpBox("點擊場景中的位置添加路徑點", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawActionsSection(GUIStyle headerStyle)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Actions", headerStyle);
            EditorGUI.BeginDisabledGroup(_crowdPath.isInEditMode);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                RefreshGUI();
            }

            if (GUILayout.Button("Reset", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
            {
                ClearPoints();
                UnlockInspectorAndSelectObject();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void DrawPointConfigSection(GUIStyle headerStyle)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(_crowdPath.isInEditMode);
            EditorGUILayout.LabelField("Point Config", headerStyle);
            if (GUILayout.Button(_crowdPath.isOpenPointConfigPanel
                    ? "Close Point Config Panel"
                    : "Open Point Config Panel"))
            {
                TogglePointConfigPanel();
            }

            EditorGUI.EndDisabledGroup();

            if (_crowdPath.isOpenPointConfigPanel)
            {
                DrawPointConfigPanel();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPointConfigPanel()
        {
            EditorGUI.indentLevel++;
            for (var i = 0; i < _waypointsProp.arraySize; i++)
            {
                var waypoint = _waypointsProp.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(waypoint, GUIContent.none);

                if (waypoint.objectReferenceValue is GameObject waypointGo)
                {
                    var crowdPathPoint = waypointGo.GetComponent<CrowdPathPoint>();
                    if (crowdPathPoint != null)
                    {
                        DrawCrowdPathPointConfig(crowdPathPoint);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawCrowdPathPointConfig(CrowdPathPoint crowdPathPoint)
        {
            var waypointSo = new SerializedObject(crowdPathPoint);
            var radiusProp = waypointSo.FindProperty("allowableRadius");
            EditorGUILayout.LabelField("Radius", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f));
            EditorGUILayout.PropertyField(radiusProp, GUIContent.none,
                GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f));
            if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
            {
                DestroyImmediate(crowdPathPoint.gameObject);
                RefreshGUI();
            }
        }

        private void ToggleEditMode()
        {
            var totalModes  = Enum.GetNames(typeof(CrowdPath.EditMode)).Length;
            var currentMode = (int)_crowdPath.editMode;
            _crowdPath.editMode = (CrowdPath.EditMode)((currentMode + 1) % totalModes);

            switch (_crowdPath.editMode)
            {
                case CrowdPath.EditMode.None:
                    _crowdPath.isInEditMode = false;

                    UnlockInspectorAndSelectObject();
                    break;
                case CrowdPath.EditMode.Add:
                    _crowdPath.isInEditMode = true;

                    _isLocked = UnityUtils.LockInspector(_isLocked);
                    SelectLastPoint();
                    break;
            }
        }

        private void SelectLastPoint()
        {
            if (_crowdPath.waypoints.Count <= 0) return;
            Selection.activeGameObject = _crowdPath.waypoints[_crowdPath.waypoints.Count - 1];
        }

        private void TogglePointConfigPanel()
        {
            _crowdPath.isOpenPointConfigPanel = !_crowdPath.isOpenPointConfigPanel;
        }

        private void ClearPoints()
        {
            foreach (var point in _crowdPath.waypoints.Where(point => point != null))
            {
                DestroyImmediate(point);
            }

            _crowdPath.waypoints.Clear();
            _crowdPath.isInEditMode = false;
        }

        private void RefreshGUI()
        {
            _crowdPath.waypoints = _crowdPath.GetComponentsInChildren<CrowdPathPoint>()
                .Select(point => point.gameObject).ToList();
        }

        private void UnlockInspectorAndSelectObject()
        {
            _isLocked = UnityUtils.UnlockInspector(_isLocked);
            UnityUtils.SelectGameObject(_crowdPath != null ? _crowdPath.gameObject : null);
        }
    }
}