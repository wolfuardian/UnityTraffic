using System.Linq;
using UnityEditor;
using UnityEngine;
using Runtime.Crowd;

namespace Editor.Crowd
{
    [CustomEditor(typeof(CrowdPath))]
    public class CrowdPathEditor : UnityEditor.Editor
    {
        private CrowdPath _crowdPath;
        private static bool _isLocked;
        private SerializedProperty _waypointsProp;

        private void OnEnable()
        {
            _crowdPath = (CrowdPath)target;
            _waypointsProp = serializedObject.FindProperty("waypoints");
        }

        private void OnSceneGUI()
        {
            if (!_crowdPath.isInEditMode || Event.current.type != EventType.MouseDown ||
                Event.current.button != 0)
                return;
            Debug.Log("OnSceneGUI");
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (!Physics.Raycast(ray, out var hit)) return;

            Undo.RecordObject(_crowdPath, "Add Point");
            _crowdPath.AddPoint(hit.point);
            Event.current.Use();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var headerStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 12 };
            headerStyle.normal.textColor = EditorStyles.boldLabel.normal.textColor;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Add Mode", headerStyle);
            if (GUILayout.Button(_crowdPath.isInEditMode ? "Exit" : "Enter Add Mode"))
            {
                ToggleEditMode();
            }

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Actions", headerStyle);
            EditorGUI.BeginDisabledGroup(_crowdPath.isInEditMode);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                RefreshWaypoints();
            }

            if (GUILayout.Button("Reset", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
            {
                ClearPoints();
            }

            EditorGUILayout.EndHorizontal();


            EditorGUI.EndDisabledGroup();

            GUILayout.Space(5);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(_crowdPath.isInEditMode);
            EditorGUILayout.LabelField("Point Config", headerStyle);
            if (GUILayout.Button(_crowdPath._isOpenPointConfigPanel
                    ? "Close Point Config Panel"
                    : "Open Point Config Panel"))
            {
                TogglePointConfigPanel();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();


            if (_crowdPath._isOpenPointConfigPanel)
            {
                EditorGUI.indentLevel++;
                for (var i = 0; i < _waypointsProp.arraySize; i++)
                {
                    var waypoint = _waypointsProp.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PropertyField(waypoint, GUIContent.none);

                    var waypointGo = waypoint.objectReferenceValue as GameObject;
                    if (waypointGo != null)
                    {
                        var crowdPathPoint = waypointGo.GetComponent<CrowdPathPoint>();
                        if (crowdPathPoint != null)
                        {
                            var waypointSo = new SerializedObject(crowdPathPoint);
                            var radiusProp = waypointSo.FindProperty("allowableRadius");
                            EditorGUILayout.LabelField("Radius",
                                GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f));
                            EditorGUILayout.PropertyField(radiusProp, GUIContent.none,
                                GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f));
                            GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f));
                            if (GUI.changed)
                            {
                                waypointSo.ApplyModifiedProperties();
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }


            serializedObject.ApplyModifiedProperties();
        }

        private void ToggleEditMode()
        {
            _crowdPath.isInEditMode = !_crowdPath.isInEditMode;

            if (_crowdPath.isInEditMode)
            {
                LockInspector();
                SelectLastPoint();
            }
            else
            {
                UnlockInspectorAndSelectObject();
            }
        }

        private void SelectLastPoint()
        {
            if (_crowdPath.waypoints.Count <= 0) return;
            Selection.activeGameObject = _crowdPath.waypoints[_crowdPath.waypoints.Count - 1];
        }

        private void TogglePointConfigPanel()
        {
            _crowdPath._isOpenPointConfigPanel = !_crowdPath._isOpenPointConfigPanel;
        }

        private void ClearPoints()
        {
            foreach (var point in _crowdPath.waypoints.Where(point => point != null))
            {
                DestroyImmediate(point);
            }

            _crowdPath.waypoints.Clear();
            _crowdPath.isInEditMode = false;
            UnlockInspectorAndSelectObject();
        }

        private void RefreshWaypoints()
        {
            _crowdPath.waypoints = _crowdPath.GetComponentsInChildren<CrowdPathPoint>()
                .Select(point => point.gameObject).ToList();
        }


        private void UnlockInspectorAndSelectObject()
        {
            UnlockInspector();

            if (_crowdPath != null)
            {
                Selection.activeObject = _crowdPath.gameObject;
            }
        }

        private static void LockInspector()
        {
            if (_isLocked) return;

            ActiveEditorTracker.sharedTracker.isLocked = true;
            _isLocked = true;
        }

        private static void UnlockInspector()
        {
            if (!_isLocked) return;

            ActiveEditorTracker.sharedTracker.isLocked = false;
            _isLocked = false;
        }
    }
}