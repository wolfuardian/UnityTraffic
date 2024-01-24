using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(PathBuilder))]
    public class PathBuilderEditor : UnityEditor.Editor
    {
        private PathBuilder        pathBuilder;
        private SerializedProperty waypointsProp;

        private void OnEnable()
        {
            pathBuilder   = (PathBuilder)target;
            waypointsProp = new SerializedObject(pathBuilder.Path).FindProperty("waypoints");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (!Validate()) return;

            DrawEditModeSwitch();
            DrawActionsSection();
            DrawPointConfigSection();

            serializedObject.ApplyModifiedProperties();
        }

        private bool Validate()
        {
            var valid = true;

            if (pathBuilder.Path == null)
            {
                Debug.LogError("路徑物件為空，請確認是否有設定。", this);
                valid = false;
            }

            return valid;
        }

        private void OnSceneGUI()
        {
            HandleSceneClickToAddWaypoint();
        }

        private void HandleSceneClickToAddWaypoint()
        {
            if (pathBuilder.editMode != PathBuilder.EditMode.Add || !UnityUtils.IsLeftMouseButtonDown()) return;
            OnAddWaypoint();
            Event.current.Use();
        }

        private void OnAddWaypoint()
        {
            if (!UnityUtils.TryGetRaycastHit(out var hitPoint)) return;
            if (pathBuilder.editMode != PathBuilder.EditMode.Add) return;

            var newWaypoint = CreateWaypointObject(hitPoint);
            var path        = pathBuilder.Path;
            path.Waypoints.Add(newWaypoint);
        }

        private Transform CreateWaypointObject(Vector3 position)
        {
            var path        = pathBuilder.Path;
            var newWaypoint = new GameObject("Waypoint" + path.Waypoints.Count).transform;
            newWaypoint.position = position;
            newWaypoint.SetParent(pathBuilder.transform);

            SetupWaypointMesh(newWaypoint);
            return newWaypoint;
        }

        private static void SetupWaypointMesh(Component waypoint)
        {
            var meshFilter = waypoint.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateSphereMesh();

            var meshRenderer = waypoint.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));

            var waypointGizmos = waypoint.gameObject.AddComponent<WaypointGizmos>();
#if UNITY_EDITOR
            InternalEditorUtility.SetIsInspectorExpanded(waypointGizmos, true);
#endif
        }

        private static Mesh CreateSphereMesh()
        {
            return Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
        }

        private void DrawEditModeSwitch()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);
            DisplayCurrentEditMode();

            if (GUILayout.Button("Toggle Mode", GUILayout.Height(48)))
            {
                ToggleEditMode();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawActionsSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(pathBuilder.editMode == PathBuilder.EditMode.Add);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset All Waypoints", GUILayout.Height(48)))
            {
                ClearPoints();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }

        private void DrawPointConfigSection()
        {
            var headerStyle = UnityEditorUtils.CreateHeaderStyle(FontStyle.Bold, 12);
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(pathBuilder.editMode == PathBuilder.EditMode.Add);
            EditorGUILayout.LabelField("Point Config", headerStyle);
            if (GUILayout.Button(pathBuilder.isOpenPointConfigPanel
                    ? "Close Waypoint Config Panel"
                    : "Open Waypoint Config Panel"))
            {
                TogglePointConfigPanel();
            }

            EditorGUI.EndDisabledGroup();

            if (pathBuilder.isOpenPointConfigPanel)
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

            EditorGUI.indentLevel--;
        }

        private void DrawWaypointsConfig(Component component)
        {
            var waypointSo    = new SerializedObject(component);
            var pathBuilderSo = new SerializedObject(pathBuilder);
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
                DeleteItem(component);
                pathBuilderSo.ApplyModifiedProperties();
            }
        }

        private void TogglePointConfigPanel()
        {
            pathBuilder.isOpenPointConfigPanel = !pathBuilder.isOpenPointConfigPanel;
        }

        private void ToggleEditMode()
        {
            pathBuilder.editMode = (PathBuilder.EditMode)(((int)pathBuilder.editMode + 1) %
                                                          Enum.GetNames(typeof(PathBuilder.EditMode)).Length);

            switch (pathBuilder.editMode)
            {
                case PathBuilder.EditMode.None:
                    UnityEditorUtils.SetInspectorLock(false);
                    break;
                case PathBuilder.EditMode.Add:
                    UnityEditorUtils.SetInspectorLock(true);
                    break;
            }

            Selection.activeObject = pathBuilder.gameObject;
        }

        private void DisplayCurrentEditMode()
        {
            var customStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 14 };
            var modeLabel = pathBuilder.editMode switch
            {
                PathBuilder.EditMode.None => "Current Mode: None",
                PathBuilder.EditMode.Add => "Current Mode: Add Mode",
                _ => "Unknown Mode"
            };
            GUILayout.Label(modeLabel, customStyle, GUILayout.Height(24f));

            if (pathBuilder.editMode == PathBuilder.EditMode.Add)
            {
                EditorGUILayout.HelpBox("Click on the scene to add path points", MessageType.Info);
            }
        }

        private static void DeleteItem(Component component)
        {
            if (component != null)
            {
                Undo.RecordObject(component.gameObject, "Delete Item");
                Undo.DestroyObjectImmediate(component.gameObject);
            }
        }

        private void ClearPoints()
        {
            Undo.SetCurrentGroupName("Clear Path Points");
            foreach (var waypoint in pathBuilder.Path.Waypoints.Where(point => point != null))
            {
                Undo.DestroyObjectImmediate(waypoint.gameObject);
            }
        }
    }
}