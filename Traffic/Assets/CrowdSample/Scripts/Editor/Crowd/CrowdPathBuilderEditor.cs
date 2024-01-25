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
        private SerializedProperty crowdPathProp;
        private SerializedProperty waypointsProp;

        private bool isConfigPanelExpanded;

        #endregion

        #region Properties

        #endregion


        #region Public Methods

        #endregion

        #region Private Methods

        private bool IsPathBuilderValid()
        {
            var valid = true;

            if (crowdPathBuilder.CrowdPath == null)
            {
                Debug.LogError("路徑物件為空，請確認是否有設定。", this);
                valid = false;
            }

            return valid;
        }

        private void HandleSceneClickToAddWaypoint()
        {
            if (crowdPathBuilder.editMode != CrowdPathBuilder.EditMode.Add ||
                !UnityUtils.IsLeftMouseButtonDown()) return;
            OnAddWaypoint();
            Event.current.Use();
        }

        private void OnAddWaypoint()
        {
            if (!UnityUtils.TryGetRaycastHit(out var hitPoint)) return;
            if (crowdPathBuilder.editMode != CrowdPathBuilder.EditMode.Add) return;

            var newWaypoint = CreateWaypointObject(hitPoint);
            var path        = crowdPathBuilder.CrowdPath;
            path.Waypoints.Add(newWaypoint);
        }

        private Transform CreateWaypointObject(Vector3 position)
        {
            var path        = crowdPathBuilder.CrowdPath;
            var newWaypoint = new GameObject("Waypoint" + path.Waypoints.Count).transform;
            newWaypoint.position = position;
            newWaypoint.SetParent(crowdPathBuilder.transform);

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
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset All Waypoints"))
            {
                ClearPoints();
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
                TogglePointConfigPanel();
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
                DeleteItem(component);
                pathBuilderSo.ApplyModifiedProperties();
            }
        }

        private void TogglePointConfigPanel()
        {
            isConfigPanelExpanded = !isConfigPanelExpanded;
        }

        private void ToggleEditMode()
        {
            crowdPathBuilder.editMode = (CrowdPathBuilder.EditMode)(((int)crowdPathBuilder.editMode + 1) %
                                                                    Enum.GetNames(typeof(CrowdPathBuilder.EditMode))
                                                                        .Length);

            switch (crowdPathBuilder.editMode)
            {
                case CrowdPathBuilder.EditMode.None:
                    UnityEditorUtils.SetInspectorLock(false);
                    break;
                case CrowdPathBuilder.EditMode.Add:
                    UnityEditorUtils.SetInspectorLock(true);
                    break;
            }

            Selection.activeObject = crowdPathBuilder.gameObject;
        }

        private void DisplayCurrentEditMode()
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
            foreach (var waypoint in crowdPathBuilder.CrowdPath.Waypoints.Where(point => point != null))
            {
                Undo.DestroyObjectImmediate(waypoint.gameObject);
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            try
            {
                crowdPathBuilder = (CrowdPathBuilder)target;
                crowdPathProp    = serializedObject.FindProperty("crowdPath");
                waypointsProp    = serializedObject.FindProperty("waypoints");
            }
            catch (Exception)
            {
                // ignored 找不到原因，只好先這樣處理
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!IsPathBuilderValid()) return;

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
            HandleSceneClickToAddWaypoint();
        }

        #endregion
    }
}