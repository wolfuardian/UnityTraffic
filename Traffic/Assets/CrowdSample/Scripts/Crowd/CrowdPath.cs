using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Crowd
{
    [ExecuteInEditMode]
    public class CrowdPath : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private CrowdPathConfig m_crowdPathConfig;
        [SerializeField] private List<CrowdWaypoint>       m_waypoints   = new List<CrowdWaypoint>();
        [SerializeField] private List<SpawnPoint>     m_spawnPoints = new List<SpawnPoint>();
        [SerializeField] private float                m_arrowScale  = 1f;

        public enum EditMode
        {
            None = 0,
            Add  = 1
        }

        #endregion

        #region Properties

        public CrowdPathConfig crowdPathConfig
        {
            get => m_crowdPathConfig;
            set => m_crowdPathConfig = value;
        }

        public List<CrowdWaypoint> waypoints
        {
            get => m_waypoints;
            set => m_waypoints = value;
        }

        public List<SpawnPoint> spawnPoints
        {
            get => m_spawnPoints;
            set => m_spawnPoints = value;
        }

        public float arrowScale => m_arrowScale;

        public EditMode editMode { get; set; }

        #endregion

        #region Implementation Methods

        public void UpdateImmediately()
        {
            waypoints = GetWaypoints();

            if (crowdPathConfig == null)
            {
                return;
            }

            if (crowdPathConfig.reverse)
            {
                waypoints.Reverse();
            }

            for (var i = 0; i < waypoints.Count; i++)
            {
                waypoints[i].waypointID = i;
            }

            if (waypoints.Count < 2)
            {
                return;
            }

            spawnPoints = GetSpawnPoints();
        }

        #endregion

        #region Unity Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateImmediately();
        }

        private void OnDrawGizmos()
        {
            if (waypoints.Count < 2)
            {
                return;
            }

            DrawSpawnArrows();
            DrawPathArrows();
        }
#endif

        #endregion

        #region Public Methods

        public List<CrowdWaypoint> GetWaypoints()
        {
            return transform.GetComponentsInChildren<CrowdWaypoint>().ToList();
        }

        public IEnumerable<Transform> GetWaypointsTransform()
        {
            return waypoints.Select(wp => wp.transform).ToList();
        }

        #endregion

        #region Private Methods

        private List<SpawnPoint> GetSpawnPoints()
        {
            var config      = crowdPathConfig;
            var points      = waypoints.Select(wp => wp.transform.position).ToList();
            var totalLength = CrowdPathResolver.GetTotalLength(points, config.pathClosed);
            var maxCount    = Mathf.FloorToInt(totalLength / config.spacing);

            var spawnCount = config.spawnOnce ? config.instantCount :
                config.useSpacing ? Mathf.Min(config.instantCount, maxCount) : config.instantCount;

            var actualCount = 0;
            for (var i = 0; i < spawnCount; i++)
            {
                var distance = CrowdPathResolver.CalculateDistance(config, i, totalLength);
                if (distance > totalLength && !config.pathClosed) break;
                actualCount++;
            }

            var newSpawnPoints = new List<SpawnPoint>();

            for (var i = 0; i < actualCount; i++)
            {
                var distance = CrowdPathResolver.CalculateDistance(config, i, totalLength);

                newSpawnPoints.Add(NewSpawnPoint(distance, totalLength));
            }

            return newSpawnPoints;
        }

        private SpawnPoint NewSpawnPoint(float distance, float totalLength)
        {
            var globalInterp  = distance / totalLength;
            var config        = crowdPathConfig;
            var points        = waypoints.Select(wp => wp.transform.position).ToList();
            var position      = CrowdPathResolver.GetPositionAt(config, points, globalInterp);
            var direction     = CrowdPathResolver.GetDirectionAt(config, points, globalInterp);
            var pathLocation  = CrowdPathResolver.GetLocalDistanceAt(config, points, globalInterp);
            var newSpawnPoint = new SpawnPoint(position, direction, globalInterp, pathLocation);
            return newSpawnPoint;
        }

        #endregion

        #region Debug and Visualization Methods

        private void DrawSpawnArrows()
        {
            foreach (var spawnData in spawnPoints)
            {
                GizmosUtils.ThicknessArrow(spawnData.position, spawnData.direction, Color.yellow, arrowScale);
                GizmosUtils.Polygon(spawnData.position, Color.yellow, arrowScale * 0.1f, 16);
                GizmosUtils.Polygon(spawnData.position, Color.yellow, arrowScale * 0.2f, 16);

                var style = new GUIStyle
                {
                    normal    = { textColor = Color.cyan },
                    alignment = TextAnchor.MiddleLeft,
                    fontSize  = 9
                };

                var text = "" +
                           "  Interp: " + spawnData.pathInterp + "\n" +
                           "  Location: " + spawnData.pathLocation;
                Handles.Label(spawnData.position, text, style);
            }
        }

        private void DrawPathArrows()
        {
            var config = crowdPathConfig;
            var count  = waypoints.Count;
            var points = waypoints.Select(wp => wp.transform.position).ToList();
            int actualCount;

            if (config.pathClosed)
                actualCount = count;
            else
                actualCount = count - 1;

            for (var index = 0; index < actualCount; index++)
            {
                var nextIndex = (index + 1) % count;
                if (waypoints[index] == null || waypoints[nextIndex] == null)
                {
                    continue;
                }

                var point     = points[index];
                var nextPoint = points[nextIndex];
                Gizmos.DrawLine(point, nextPoint);
                GizmosUtils.Arrow(point, nextPoint - point, Color.cyan);
            }
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdPath))]
    public class CrowdPathEditor : Editor
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

            DrawToggleMode("編輯模式");

            var isLockInEditing = crowdPath.editMode == CrowdPath.EditMode.Add;

            EditorGUI.BeginDisabledGroup(isLockInEditing);

            DrawActions("動作");

            DrawPointConfig("編輯路徑點");

            DrawConfig("設定");

            DrawingDisplay("顯示");

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(4);

            DrawBackButton();

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
                !CrowdUtils.IsLeftMouseButtonDown())
            {
                return;
            }

            ClickAddWaypointOnScene();
            Event.current.Use();
        }

        private void ClickAddWaypointOnScene()
        {
            if (!CrowdUtils.TryGetRaycastHit(out var hitPoint)) return;
            if (crowdPath.editMode != CrowdPath.EditMode.Add) return;

            var parent       = crowdPath.transform;
            var waypointInst = CrowdUtils.CreatePoint("Waypoint" + crowdPath.waypoints.Count, hitPoint, parent);
            var waypoint     = waypointInst.gameObject.AddComponent<CrowdWaypoint>();
            crowdPath.waypoints.Add(waypoint);
        }

        #endregion

        #region GUI Methods

        private void DrawConfig(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_crowdPathConfig"),
                new GUIContent("設定資源檔"));
            EditorGUILayout.EndVertical();
        }

        private void DrawToggleMode(string label)
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
                        CrowdUtils.SetInspectorLock(false);
                        break;
                    case CrowdPath.EditMode.Add:
                        CrowdUtils.SetInspectorLock(true);
                        break;
                }

                Selection.activeObject = crowdPath.gameObject;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawActions(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("重設所有路徑點", GUILayout.Height(24)))
            {
                CrowdUtils.ClearPoints(crowdPath.GetWaypointsTransform());
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawPointConfig(string label)
        {
            var headerStyle = CrowdUtils.CreateHeaderStyle(FontStyle.Bold, 12);
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
                    if (waypoint.objectReferenceValue is CrowdWaypoint component)
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
                            CrowdUtils.DeleteItem(component);
                            pathBuilderSo.ApplyModifiedProperties();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawingDisplay(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(arrowScaleProp, new GUIContent("圖示大小"));
            EditorGUILayout.EndVertical();
        }

        private void DrawBackButton()
        {
            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button("返回️", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f)))
            {
                Selection.activeObject = crowdPath.transform.parent;
            }

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}