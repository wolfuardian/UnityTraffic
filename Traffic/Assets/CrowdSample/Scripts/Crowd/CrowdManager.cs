using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdManager : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private CrowdPath              m_path;
        [SerializeField] private CrowdSpawner           m_spawner;
        [SerializeField] private CrowdPathConfig   m_crowdPathConfig;
        [SerializeField] private List<GameObject>       m_spawnerInstances = new List<GameObject>();
        [SerializeField] private List<CrowdAgentConfig> m_agentConfigs     = new List<CrowdAgentConfig>();

        #endregion

        #region Properties

        public CrowdPath              path                 => m_path;
        public CrowdPathConfig   crowdPathConfig => m_crowdPathConfig;
        public List<GameObject>       spawnerInstances     => m_spawnerInstances;
        public List<CrowdAgentConfig> agentConfigs         => m_agentConfigs;
        public bool                   createdSpawner       => m_spawner != null;
        public bool                   initialized          => m_path != null;

        #endregion

#if UNITY_EDITOR

        #region Implementation Methods

        public void UpdateImmediately()
        {
            if (m_path != null)

            {
                m_path.crowdPathConfig = m_crowdPathConfig;
            }

            if (createdSpawner)
            {
                m_spawner.path = m_path;
            }
        }

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            if (crowdPathConfig == null) return;
            UnityUtils.UpdateAllReceiverImmediately();
        }

        #endregion

#endif

        #region Public Methods

        public void Initialize()
        {
            if (initialized) return;

            CreatePathInstance();
        }


        public void CreatePathInstance()
        {
            if (m_path != null) return;

            var instance  = new GameObject("Path");
            var component = instance.AddComponent<CrowdPath>();

            component.crowdPathConfig = m_crowdPathConfig;

            instance.transform.SetParent(transform);

            m_path = component;
        }

        public void AddInstance()
        {
            var instance = new GameObject("Spawner_" + spawnerInstances.Count);
            instance.transform.SetParent(transform);
            spawnerInstances.Add(instance);

            var spawner = instance.AddComponent<CrowdSpawner>();
            spawner.path = path;

            InternalEditorUtility.SetIsInspectorExpanded(spawner, true);
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdManager))]
    public class CrowdManagerEditor : Editor
    {
        #region Field Declarations

        private CrowdManager       crowdManager;
        private SerializedProperty pathProp;
        private SerializedProperty spawnerInstancesProp;
        private SerializedProperty crowdSpawnConfigProp;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdManager         = (CrowdManager)target;
            pathProp             = serializedObject.FindProperty("m_path");
            spawnerInstancesProp = serializedObject.FindProperty("m_spawnerInstances");
            crowdSpawnConfigProp = serializedObject.FindProperty("m_crowdPathConfig");
        }

        public override void OnInspectorGUI()
        {
            var errorCount = 0;
            if (crowdManager.path == null) errorCount++;
            if (crowdSpawnConfigProp.objectReferenceValue == null) errorCount++;

            DrawPathConfig("路徑");

            EditorGUILayout.Space(1);

            DrawAgentConfig("代理物件設定");

            if (errorCount > 0)
            {
                EditorGUILayout.HelpBox($"請先完成初始化。還有 {errorCount} 個物件還沒初始化", MessageType.Warning);
            }

            EditorGUILayout.Space(4);

            DrawBackButton();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion


        #region GUI Methods

        private void DrawPathConfig(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(crowdManager.initialized);
            if (GUILayout.Button("初始化路徑", GUILayout.Height(48)))
            {
                crowdManager.Initialize();
            }

            EditorGUILayout.PropertyField(pathProp, new GUIContent("路徑物件"));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(crowdSpawnConfigProp, new GUIContent("路徑生成設定"));
            if (GUILayout.Button("前往路徑編輯器↗️", GUILayout.Height(24)))
            {
                Selection.activeObject = crowdManager.path;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawAgentConfig(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            var self = crowdManager;
            DrawAgentsTable(self.spawnerInstances, self.agentConfigs, self.AddInstance);
            EditorGUILayout.EndVertical();
        }

        private void DrawAgentsTable(List<GameObject> instances, List<CrowdAgentConfig> configs, System.Action action)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            var alignedFieldWidth = EditorGUIUtility.currentViewWidth * 0.25f;
            EditorGUILayout.LabelField("生成器",   EditorStyles.boldLabel, GUILayout.Width(alignedFieldWidth));
            EditorGUILayout.LabelField("代理設定檔", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            instances.RemoveAll(item => item == null);
            configs.RemoveAll(item => item == null);

            var toRemove = new List<GameObject>();

            for (var i = 0; i < spawnerInstancesProp.arraySize; i++)
            {
                var spawnerInst = spawnerInstancesProp.GetArrayElementAtIndex(i);
                if (spawnerInst.objectReferenceValue == null) continue; // 跳過已經被刪除的 waypoint, 防止介面卡住
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(spawnerInst, GUIContent.none, GUILayout.Width(alignedFieldWidth));
                if (spawnerInst.objectReferenceValue is GameObject component)
                {
                    var spawner       = component.GetComponent<CrowdSpawner>();
                    var spawnerInstSo = new SerializedObject(spawner);
                    var configProp    = spawnerInstSo.FindProperty("m_agentConfig");
                    EditorGUILayout.PropertyField(configProp, GUIContent.none);


                    if (GUILayout.Button("前往↗️", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f)))
                    {
                        Selection.activeObject = component;
                    }

                    if (GUILayout.Button("✖️️", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.05f)))
                    {
                        var isConfirmed = EditorUtility.DisplayDialog(
                            "確認刪除",
                            "你確定要刪除這個物件嗎？這個操作無法復原。",
                            "刪除",
                            "取消"
                        );
                        if (isConfirmed)
                        {
                            toRemove.Add(component);
                        }
                    }


                    spawnerInstSo.ApplyModifiedProperties();
                }

                EditorGUILayout.EndHorizontal();
            }

            UnityUtils.RemoveInstances(instances, toRemove);

            if (GUILayout.Button("新增"))
            {
                action?.Invoke();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawBackButton()
        {
            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button("返回️", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f)))
            {
                Selection.activeObject = crowdManager.transform.parent;
            }

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}