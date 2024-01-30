using System.Linq;
using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Utils;

namespace CrowdSample.Scripts.Crowd
{
    [CreateAssetMenu(fileName = "CrowdConfig", menuName = "CrowdWizard/Crowd Config")]
    public class CrowdConfig : ScriptableObject
    {
        #region Field Declarations

        // Generation
        [SerializeField] private GenerationMode m_generationMode = GenerationMode.InfinityFlow;
        [SerializeField] private float          m_spawnInterval  = 2f;
        [SerializeField] private int            m_instantCount   = 15;
        [SerializeField] private int            m_maxSpawnCount  = 100;
        [SerializeField] private float          m_spacing        = 5f;
        [SerializeField] private float          m_offset         = 0f;
        [SerializeField] private bool           m_spawnOnce      = true;
        [SerializeField] private bool           m_useSpacing     = true;

        // PathFollow
        [SerializeField] private bool m_reverse;
        [SerializeField] private bool m_pathClosed;
        [SerializeField] private bool m_shouldDestroy;

        // Resources
        [SerializeField] private GameObject[] m_prefabs;

        // NavMeshAgentData
        [SerializeField] private float m_minSpeed         = 4f;
        [SerializeField] private float m_maxSpeed         = 5f;
        [SerializeField] private float m_angularSpeed     = 100f;
        [SerializeField] private float m_acceleration     = 5f;
        [SerializeField] private float m_stoppingDistance = 1f;
        [SerializeField] private bool  m_autoBraking      = true;

        // AgentUserData
        [SerializeField] private string m_agentID = "No Data";
        [SerializeField] private string m_type    = "No Data";
        [SerializeField] private string m_category;
        [SerializeField] private string m_alias;
        [SerializeField] private string m_model;
        [SerializeField] private string m_time;
        [SerializeField] private string m_noted;

        public enum GenerationMode
        {
            InfinityFlow,
            MultipleCircle,
            SingleCircle,
            Custom
        }

        #endregion

        #region Properties

        // Generation

        public GenerationMode generationMode
        {
            get => m_generationMode;
            set => m_generationMode = value;
        }

        public int instantCount
        {
            get => m_instantCount;
            set => SetFieldValue(ref m_instantCount, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        public float spawnInterval
        {
            get => m_spawnInterval;
            set => SetFieldValue(ref m_spawnInterval, value, GenerationMode.Custom, GenerationMode.InfinityFlow);
        }

        public int maxSpawnCount
        {
            get => m_maxSpawnCount;
            set => SetFieldValue(ref m_maxSpawnCount, value, GenerationMode.Custom, GenerationMode.InfinityFlow,
                GenerationMode.MultipleCircle);
        }

        public float spacing
        {
            get => m_spacing;
            set => SetFieldValue(ref m_spacing, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        public float offset
        {
            get => m_offset;
            set => SetFieldValue(ref m_offset, value, GenerationMode.Custom, GenerationMode.MultipleCircle,
                GenerationMode.SingleCircle);
        }

        public bool spawnOnce
        {
            get => m_spawnOnce;
            set => SetFieldValue(ref m_spawnOnce, value, GenerationMode.Custom);
        }

        public bool reverse
        {
            get => m_reverse;
            set => SetFieldValue(ref m_reverse, value, GenerationMode.Custom, GenerationMode.InfinityFlow,
                GenerationMode.MultipleCircle,
                GenerationMode.SingleCircle);
        }

        public bool pathClosed
        {
            get => m_pathClosed;
            set => SetFieldValue(ref m_pathClosed, value, GenerationMode.Custom, GenerationMode.InfinityFlow,
                GenerationMode.SingleCircle);
        }

        public bool useSpacing
        {
            get => m_useSpacing;
            set => SetFieldValue(ref m_useSpacing, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        // PathFollow
        public bool shouldDestroy
        {
            get => m_shouldDestroy;
            set => m_shouldDestroy = value;
        }

        // Resources
        public GameObject[] prefabs
        {
            get => m_prefabs;
            set => m_prefabs = value;
        }

        // NavMeshAgentData
        public float minSpeed
        {
            get => m_minSpeed;
            set => m_minSpeed = value;
        }

        public float maxSpeed
        {
            get => m_maxSpeed;
            set => m_maxSpeed = value;
        }

        public float angularSpeed
        {
            get => m_angularSpeed;
            set => m_angularSpeed = value;
        }

        public float acceleration
        {
            get => m_acceleration;
            set => m_acceleration = value;
        }

        public float stoppingDistance
        {
            get => m_stoppingDistance;
            set => m_stoppingDistance = value;
        }

        public bool autoBraking
        {
            get => m_autoBraking;
            set => m_autoBraking = value;
        }
        // AgentUserData

        public string agentID
        {
            get => m_agentID;
            set => m_agentID = value;
        }

        public string type
        {
            get => m_type;
            set => m_type = value;
        }

        public string category
        {
            get => m_category;
            set => m_category = value;
        }

        public string alias
        {
            get => m_alias;
            set => m_alias = value;
        }

        public string model
        {
            get => m_model;
            set => m_model = value;
        }

        public string time
        {
            get => m_time;
            set => m_time = value;
        }

        public string noted
        {
            get => m_noted;
            set => m_noted = value;
        }

        #endregion

        #region Public Methods

        public void ApplyPresetProperties()
        {
            switch (m_generationMode)
            {
                case GenerationMode.InfinityFlow:
                    m_spawnOnce     = false;
                    m_pathClosed    = false;
                    m_shouldDestroy = !m_pathClosed;
                    m_instantCount  = 1;
                    m_offset        = 0;
                    break;
                case GenerationMode.MultipleCircle:
                    m_spawnOnce     = true;
                    m_pathClosed    = true;
                    m_shouldDestroy = !m_pathClosed;
                    break;
                case GenerationMode.SingleCircle:
                    m_spawnOnce     = true;
                    m_shouldDestroy = !m_pathClosed;
                    m_instantCount  = 1;
                    m_maxSpawnCount = 1;
                    break;
                case GenerationMode.Custom:
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void SetFieldValue<T>(ref T field, T value, params GenerationMode[] modes)
        {
            if (modes.Contains(m_generationMode)) field = value;
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdConfig))]
    public class CrowdConfigEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private CrowdConfig        crowdConfig;
        private SerializedProperty prefabsProp;
        private SerializedProperty minSpeedProp;
        private SerializedProperty maxSpeedProp;
        private SerializedProperty angularSpeedProp;
        private SerializedProperty accelerationProp;
        private SerializedProperty stoppingDistanceProp;
        private SerializedProperty autoBrakingProp;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdConfig          = (CrowdConfig)target;
            prefabsProp          = serializedObject.FindProperty("m_prefabs");
            minSpeedProp         = serializedObject.FindProperty("m_minSpeed");
            maxSpeedProp         = serializedObject.FindProperty("m_maxSpeed");
            angularSpeedProp     = serializedObject.FindProperty("m_angularSpeed");
            accelerationProp     = serializedObject.FindProperty("m_acceleration");
            stoppingDistanceProp = serializedObject.FindProperty("m_stoppingDistance");
            autoBrakingProp      = serializedObject.FindProperty("m_autoBraking");
        }

        public override void OnInspectorGUI()
        {
            DrawPathGenerationConfig(crowdConfig, "路徑與生成設定");

            DrawPrefabConfig(crowdConfig, "代理模型設定");

            DrawAgentConfig(crowdConfig, "NavAgentMesh 物件設定");


            if (GUI.changed)
            {
                UnityUtils.UpdateAllReceiverImmediately();
            }

            SceneView.RepaintAll();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        private void DrawPrefabConfig(CrowdConfig config, string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(prefabsProp, new GUIContent("模型資源"));
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private void DrawAgentConfig(CrowdConfig config, string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var shouldDisplayWarning = prefabsProp.arraySize == 0 ||
                                       Enumerable.Range(0, prefabsProp.arraySize)
                                           .Select(index => prefabsProp.GetArrayElementAtIndex(index))
                                           .Any(item => item.objectReferenceValue == null);
            if (shouldDisplayWarning)
            {
                EditorGUILayout.HelpBox("請確認所有資源都已正確設置。", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(minSpeedProp,         new GUIContent("最小速度"));
            EditorGUILayout.PropertyField(maxSpeedProp,         new GUIContent("最大速度"));
            EditorGUILayout.PropertyField(angularSpeedProp,     new GUIContent("轉動速度"));
            EditorGUILayout.PropertyField(accelerationProp,     new GUIContent("加速度"));
            EditorGUILayout.PropertyField(stoppingDistanceProp, new GUIContent("停止距離"));
            EditorGUILayout.PropertyField(autoBrakingProp,      new GUIContent("自動煞車"));
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private static void DrawPathGenerationConfig(CrowdConfig config, string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            config.generationMode =
                (CrowdConfig.GenerationMode)EditorGUILayout.EnumPopup("生成模式", config.generationMode);

            // 依據不同模式切替不同的選項
            switch (config.generationMode)
            {
                case CrowdConfig.GenerationMode.InfinityFlow:
                    DisplayInfinityFlowOptions(config, "InfinityFlow：按指定的生成間隔持續生成代理。");
                    break;
                case CrowdConfig.GenerationMode.MultipleCircle:
                    DisplayMultipleCircleOptions(config, "MultipleCircle：一次性生成指定數量的代理。");
                    break;
                case CrowdConfig.GenerationMode.SingleCircle:
                    DisplaySingleCircleOptions(config, "SingleCircle：生成單個代理，並可設置路徑為開放或封閉。");
                    break;
                case CrowdConfig.GenerationMode.Custom:
                    DrawCustomOptions(config, "Custom：允許自定義所有參數。");
                    break;
            }

            config.ApplyPresetProperties();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private static void DisplayInfinityFlowOptions(CrowdConfig config, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_SpawnInterval(config);
            Draw_Reverse(config);
            Draw_MaxSpawnCount(config);
        }

        private static void DisplayMultipleCircleOptions(CrowdConfig config, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_InstantCount(config);
            Draw_Reverse(config);
            Draw_MaxSpawnCount(config);
            Draw_Offset(config);
            Draw_UseSpacing(config);
            if (config.useSpacing)
            {
                Draw_Spacing(config);
            }
        }

        private static void DisplaySingleCircleOptions(CrowdConfig config, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_ClosedLoop(config);
            Draw_Reverse(config);
            Draw_Offset(config);
        }

        private static void DrawCustomOptions(CrowdConfig config, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_SpawnAgentOnce(config);
            Draw_ClosedLoop(config);
            Draw_Reverse(config);
            Draw_ShouldDestroyOnGoal(config);
            Draw_SpawnInterval(config);
            Draw_InstantCount(config);
            Draw_MaxSpawnCount(config);
            Draw_Offset(config);
            Draw_UseSpacing(config);
            if (config.useSpacing)
            {
                Draw_Spacing(config);
            }
        }

        private static void Draw_SpawnAgentOnce(CrowdConfig config)
        {
            config.spawnOnce = EditorGUILayout.Toggle("一次性生成代理", config.spawnOnce);
        }

        private static void Draw_ClosedLoop(CrowdConfig config)
        {
            config.pathClosed = EditorGUILayout.Toggle("封閉路徑", config.pathClosed);
        }

        private static void Draw_Reverse(CrowdConfig config)
        {
            config.reverse = EditorGUILayout.Toggle("反轉路徑", config.reverse);
        }

        private static void Draw_ShouldDestroyOnGoal(CrowdConfig config)
        {
            config.shouldDestroy = EditorGUILayout.Toggle("抵達終點後銷毀", config.shouldDestroy);
        }

        private static void Draw_InstantCount(CrowdConfig config)
        {
            config.instantCount = EditorGUILayout.IntSlider("一次性生成數量", config.instantCount, 1, 100);
        }

        private static void Draw_SpawnInterval(CrowdConfig config)
        {
            config.spawnInterval = EditorGUILayout.Slider("生成間隔", config.spawnInterval, 0.1f, 10f);
        }

        private static void Draw_MaxSpawnCount(CrowdConfig config)
        {
            config.maxSpawnCount = EditorGUILayout.IntSlider("最大生成數量", config.maxSpawnCount, 1, 100);
        }

        private static void Draw_Offset(CrowdConfig config)
        {
            config.offset = EditorGUILayout.FloatField("偏移", config.offset);
            config.offset = Mathf.Clamp(config.offset, 0, float.MaxValue);
        }

        private static void Draw_UseSpacing(CrowdConfig config)
        {
            config.useSpacing = EditorGUILayout.Toggle("使用間距", config.useSpacing);
        }

        private static void Draw_Spacing(CrowdConfig config)
        {
            config.spacing = EditorGUILayout.Slider("間距", config.spacing, 1f, 10f);
        }

        #endregion
    }
}