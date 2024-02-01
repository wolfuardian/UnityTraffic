using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Crowd
{
    [CreateAssetMenu(fileName = "CrowdPathConfig", menuName = "CrowdWizard/Crowd Path Config")]
    public class CrowdPathConfig : ScriptableObject
    {
        #region Field Declarations

        [SerializeField] private GenerationMode m_generationMode = GenerationMode.InfinityFlow;
        [SerializeField] private float          m_spawnInterval  = 2f;
        [SerializeField] private int            m_instantCount   = 15;
        [SerializeField] private int            m_maxSpawnCount  = 100;
        [SerializeField] private float          m_spacing        = 5f;
        [SerializeField] private float          m_offset;
        [SerializeField] private bool           m_spawnOnce  = true;
        [SerializeField] private bool           m_useSpacing = true;

        [SerializeField] private bool m_reverse;
        [SerializeField] private bool m_pathClosed;
        [SerializeField] private bool m_shouldDestroy;

        public enum GenerationMode
        {
            InfinityFlow,
            MultipleCircle,
            SingleCircle,
            Custom
        }

        #endregion

        #region Properties

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

        public bool shouldDestroy
        {
            get => m_shouldDestroy;
            set => m_shouldDestroy = value;
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

    [CustomEditor(typeof(CrowdPathConfig))]
    public class CrowdPathConfigEditor : Editor
    {
        #region Field Declarations

        private CrowdPathConfig crowdPathConfig;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdPathConfig = (CrowdPathConfig)target;
        }

        public override void OnInspectorGUI()
        {
            DrawPathGenerationConfig(crowdPathConfig, "路徑與生成設定");

            if (GUI.changed)
            {
                CrowdUtils.UpdateAllReceiverImmediately();
            }

            SceneView.RepaintAll();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        private static void DrawPathGenerationConfig(CrowdPathConfig pathConfig, string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            pathConfig.generationMode =
                (CrowdPathConfig.GenerationMode)EditorGUILayout.EnumPopup("生成模式", pathConfig.generationMode);

            switch (pathConfig.generationMode)
            {
                case CrowdPathConfig.GenerationMode.InfinityFlow:
                    DisplayInfinityFlowOptions(pathConfig, "InfinityFlow：按指定的生成間隔持續生成代理。");
                    break;
                case CrowdPathConfig.GenerationMode.MultipleCircle:
                    DisplayMultipleCircleOptions(pathConfig, "MultipleCircle：一次性生成指定數量的代理。");
                    break;
                case CrowdPathConfig.GenerationMode.SingleCircle:
                    DisplaySingleCircleOptions(pathConfig, "SingleCircle：生成單個代理，並可設置路徑為開放或封閉。");
                    break;
                case CrowdPathConfig.GenerationMode.Custom:
                    DrawCustomOptions(pathConfig, "Custom：允許自定義所有參數。");
                    break;
            }

            pathConfig.ApplyPresetProperties();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private static void DisplayInfinityFlowOptions(CrowdPathConfig pathConfig, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_SpawnInterval(pathConfig);
            Draw_Reverse(pathConfig);
            Draw_MaxSpawnCount(pathConfig);
        }

        private static void DisplayMultipleCircleOptions(CrowdPathConfig pathConfig, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_InstantCount(pathConfig);
            Draw_Reverse(pathConfig);
            Draw_MaxSpawnCount(pathConfig);
            Draw_Offset(pathConfig);
            Draw_UseSpacing(pathConfig);
            if (pathConfig.useSpacing)
            {
                Draw_Spacing(pathConfig);
            }
        }

        private static void DisplaySingleCircleOptions(CrowdPathConfig pathConfig, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_ClosedLoop(pathConfig);
            Draw_Reverse(pathConfig);
            Draw_Offset(pathConfig);
        }

        private static void DrawCustomOptions(CrowdPathConfig pathConfig, string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
            Draw_SpawnAgentOnce(pathConfig);
            Draw_ClosedLoop(pathConfig);
            Draw_Reverse(pathConfig);
            Draw_ShouldDestroyOnGoal(pathConfig);
            Draw_SpawnInterval(pathConfig);
            Draw_InstantCount(pathConfig);
            Draw_MaxSpawnCount(pathConfig);
            Draw_Offset(pathConfig);
            Draw_UseSpacing(pathConfig);
            if (pathConfig.useSpacing)
            {
                Draw_Spacing(pathConfig);
            }
        }

        private static void Draw_SpawnAgentOnce(CrowdPathConfig pathConfig)
        {
            pathConfig.spawnOnce = EditorGUILayout.Toggle("一次性生成代理", pathConfig.spawnOnce);
        }

        private static void Draw_ClosedLoop(CrowdPathConfig pathConfig)
        {
            pathConfig.pathClosed = EditorGUILayout.Toggle("封閉路徑", pathConfig.pathClosed);
        }

        private static void Draw_Reverse(CrowdPathConfig pathConfig)
        {
            pathConfig.reverse = EditorGUILayout.Toggle("反轉路徑", pathConfig.reverse);
        }

        private static void Draw_ShouldDestroyOnGoal(CrowdPathConfig pathConfig)
        {
            pathConfig.shouldDestroy = EditorGUILayout.Toggle("抵達終點後銷毀", pathConfig.shouldDestroy);
        }

        private static void Draw_InstantCount(CrowdPathConfig pathConfig)
        {
            pathConfig.instantCount = EditorGUILayout.IntSlider("一次性生成數量", pathConfig.instantCount, 1, 100);
        }

        private static void Draw_SpawnInterval(CrowdPathConfig pathConfig)
        {
            pathConfig.spawnInterval = EditorGUILayout.Slider("生成間隔", pathConfig.spawnInterval, 0.1f, 10f);
        }

        private static void Draw_MaxSpawnCount(CrowdPathConfig pathConfig)
        {
            pathConfig.maxSpawnCount = EditorGUILayout.IntSlider("最大生成數量", pathConfig.maxSpawnCount, 1, 100);
        }

        private static void Draw_Offset(CrowdPathConfig pathConfig)
        {
            pathConfig.offset = EditorGUILayout.FloatField("偏移", pathConfig.offset);
            pathConfig.offset = Mathf.Clamp(pathConfig.offset, 0, float.MaxValue);
        }

        private static void Draw_UseSpacing(CrowdPathConfig pathConfig)
        {
            pathConfig.useSpacing = EditorGUILayout.Toggle("使用間距", pathConfig.useSpacing);
        }

        private static void Draw_Spacing(CrowdPathConfig pathConfig)
        {
            pathConfig.spacing = EditorGUILayout.Slider("間距", pathConfig.spacing, 1f, 10f);
        }

        #endregion
    }
}