using UnityEditor;
using UnityEngine;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdGenerator))]
    public class CrowdGeneratorEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private CrowdGenerator     crowdGenerator;
        private SerializedProperty pathProp;
        private SerializedProperty spawnerProp;
        private SerializedProperty crowdConfigProp;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdGenerator  = (CrowdGenerator)target;
            pathProp        = serializedObject.FindProperty("m_path");
            spawnerProp     = serializedObject.FindProperty("m_spawner");
            crowdConfigProp = serializedObject.FindProperty("m_crowdConfig");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("初始化", EditorStyles.boldLabel);
            DrawInitializationSection();
            EditorGUILayout.EndVertical();

            if (!crowdGenerator.initialized) return;

            EditorGUILayout.Space(1);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("設定檔", EditorStyles.boldLabel);
            DrawInitializeCrowdConfigSection();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region GUI Methods

        private void DrawInitializationSection()
        {
            EditorGUI.BeginDisabledGroup(crowdGenerator.createdPath);
            if (GUILayout.Button("Create _Path"))
            {
                crowdGenerator.CreatePathInstance();
            }

            EditorGUILayout.PropertyField(pathProp, new GUIContent("路徑物件"));
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(crowdGenerator.createdSpawner);

            if (GUILayout.Button("Create _Spawner"))
            {
                crowdGenerator.CreateSpawnerInstance();
            }

            EditorGUILayout.PropertyField(spawnerProp, new GUIContent("生成器物件"));
            EditorGUI.EndDisabledGroup();

            if (crowdGenerator.initialized) return;

            var errorCount = 0;
            if (!crowdGenerator.createdPath) errorCount++;
            if (!crowdGenerator.createdSpawner) errorCount++;
            EditorGUILayout.HelpBox($"請先完成初始化。還有 {errorCount} 個物件還沒初始化", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInitializeCrowdConfigSection()
        {
            EditorGUILayout.PropertyField(crowdConfigProp, new GUIContent("生成設定"));
        }

        #endregion
    }
}