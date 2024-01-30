using CrowdSample.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdInitialization : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        [SerializeField] private CrowdPath    m_path;
        [SerializeField] private CrowdSpawner m_spawner;
        [SerializeField] private CrowdConfig  m_crowdConfig;

        #endregion

        #region Properties

        public CrowdConfig crowdConfig    => m_crowdConfig;
        public CrowdPath   path           => m_path;
        public bool        createdPath    => m_path != null;
        public bool        createdSpawner => m_spawner != null;
        public bool        initialized    => createdPath && createdSpawner;

        #endregion

#if UNITY_EDITOR

        #region Implementation Methods

        public void UpdateImmediately()
        {
            if (createdPath)

            {
                m_path.crowdConfig = m_crowdConfig;
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
            if (crowdConfig == null) return;
            UnityUtils.UpdateAllReceiverImmediately();
        }

        #endregion

#endif

        #region Public Methods

        public void Initialize()
        {
            if (initialized) return;

            CreatePathInstance();
            CreateSpawnerInstance();
        }


        public void CreatePathInstance()
        {
            if (createdPath) return;

            var instance  = new GameObject("Path");
            var component = instance.AddComponent<CrowdPath>();

            component.crowdConfig = m_crowdConfig;

            instance.transform.SetParent(transform);

            m_path = component;
        }

        public void CreateSpawnerInstance()
        {
            if (createdSpawner) return;

            var instance  = new GameObject("Spawner");
            var component = instance.AddComponent<CrowdSpawner>();

            component.path = m_path;

            instance.transform.SetParent(transform);

            m_spawner = component;
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdInitialization))]
    public class CrowdInitializationEditor : Editor
    {
        #region Field Declarations

        private CrowdInitialization crowdInitialization;
        private SerializedProperty  pathProp;
        private SerializedProperty  spawnerProp;
        private SerializedProperty  crowdConfigProp;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdInitialization = (CrowdInitialization)target;
            pathProp            = serializedObject.FindProperty("m_path");
            spawnerProp         = serializedObject.FindProperty("m_spawner");
            crowdConfigProp     = serializedObject.FindProperty("m_crowdConfig");
        }

        public override void OnInspectorGUI()
        {
            var errorCount = 0;
            if (!crowdInitialization.createdPath) errorCount++;
            if (!crowdInitialization.createdSpawner) errorCount++;
            if (crowdConfigProp.objectReferenceValue == null) errorCount++;

            DrawInitialization("初始化");

            EditorGUILayout.Space(1);

            DrawConfiguration("設定");

            if (errorCount > 0)
            {
                EditorGUILayout.HelpBox($"請先完成初始化。還有 {errorCount} 個物件還沒初始化", MessageType.Warning);
            }
            else
            {
                DrawAction("動作");
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region GUI Methods

        private void DrawInitialization(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(crowdInitialization.initialized);
            if (GUILayout.Button("執行初始化", GUILayout.Height(48)))
            {
                crowdInitialization.Initialize();
            }

            EditorGUILayout.PropertyField(pathProp,    new GUIContent("路徑物件"));
            EditorGUILayout.PropertyField(spawnerProp, new GUIContent("生成器物件"));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void DrawConfiguration(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(crowdConfigProp, new GUIContent("設定資源檔"));
            EditorGUILayout.EndVertical();
        }

        private void DrawAction(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            if (GUILayout.Button("跳轉至路徑建造", GUILayout.Height(48)))
            {
                Selection.activeObject = crowdInitialization.path;
            }

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}