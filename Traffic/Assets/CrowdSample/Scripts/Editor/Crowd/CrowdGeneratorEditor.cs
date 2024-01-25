using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Runtime.Crowd;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdGenerator))]
    public class CrowdGeneratorEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private CrowdGenerator     crowdGenerator;
        private SerializedProperty pathGoProp;
        private SerializedProperty crowdAgentGoProp;
        private SerializedProperty crowdAgentConfigProp;
        private SerializedProperty crowdGenerationConfigProp;

        #endregion

        #region Properties

        public bool IsCrowdAgentConfigAssigned      => crowdAgentConfigProp.objectReferenceValue != null;
        public bool IsCrowdGenerationConfigAssigned => crowdGenerationConfigProp.objectReferenceValue != null;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdGenerator            = (CrowdGenerator)target;
            pathGoProp                = serializedObject.FindProperty("pathGo");
            crowdAgentGoProp          = serializedObject.FindProperty("crowdAgentGo");
            crowdAgentConfigProp      = serializedObject.FindProperty("crowdAgentConfig");
            crowdGenerationConfigProp = serializedObject.FindProperty("crowdGenerationConfig");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("初始化", EditorStyles.boldLabel);
            DrawInitializationSection();
            EditorGUILayout.EndVertical();

            if (!crowdGenerator.Initialized) return;

            EditorGUILayout.Space(1);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("人流生成", EditorStyles.boldLabel);
            DrawInitializeCrowdConfigSection();
            DrawCrowdSection(crowdGenerator.CrowdGenerationConfig);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(1);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("個體", EditorStyles.boldLabel);
            DrawInitializeAgentConfigSection();
            DrawAgentSection(crowdGenerator.CrowdAgentConfig);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region GUI Methods

        private void DrawInitializationSection()
        {
            EditorGUI.BeginDisabledGroup(crowdGenerator.IsPathGoCreated);
            if (GUILayout.Button("Create _Path"))
            {
                crowdGenerator.CreatePathGo();
            }

            EditorGUILayout.PropertyField(pathGoProp, new GUIContent("路徑物件"));
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(crowdGenerator.IsCrowdAgentGoCreated);

            if (GUILayout.Button("Create _CrowdAgent"))
            {
                crowdGenerator.CreateCrowdAgentGo();
            }

            EditorGUILayout.PropertyField(crowdAgentGoProp, new GUIContent("人流代理物件"));
            EditorGUI.EndDisabledGroup();

            if (crowdGenerator.Initialized) return;

            var errorCount = 0;
            if (!crowdGenerator.IsPathGoCreated) errorCount++;
            if (!crowdGenerator.IsCrowdAgentGoCreated) errorCount++;
            EditorGUILayout.HelpBox($"請先完成初始化。還有 {errorCount} 個物件還沒初始化", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInitializeCrowdConfigSection()
        {
            EditorGUILayout.PropertyField(crowdGenerationConfigProp, new GUIContent("生成設定"));
        }

        private void DrawCrowdSection(CrowdGenerationConfig config)
        {
        }

        private void DrawInitializeAgentConfigSection()
        {
            EditorGUILayout.PropertyField(crowdAgentConfigProp, new GUIContent("代理設定"));
        }

        private void DrawAgentSection(CrowdAgentConfig config)
        {
        }

        #endregion
    }
}