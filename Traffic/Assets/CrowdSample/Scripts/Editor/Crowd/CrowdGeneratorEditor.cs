using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
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
            DrawInitCrowdConfigSection();
            DrawCrowdSection(crowdGenerator.CrowdGenerationConfig);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(1);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("個體", EditorStyles.boldLabel);
            DrawInitAgentConfigSection();
            DrawAgentSection(crowdGenerator.CrowdAgentConfig);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

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

        private void DrawInitCrowdConfigSection()
        {
            EditorGUILayout.PropertyField(crowdGenerationConfigProp, new GUIContent("生成設定"));
        }

        private void DrawCrowdSection(CrowdGenerationConfig config)
        {
        }

        private void DrawInitAgentConfigSection()
        {
            EditorGUILayout.PropertyField(crowdAgentConfigProp, new GUIContent("代理設定"));
        }

        private void DrawAgentSection(CrowdAgentConfig config)
        {
        }

        private static void DrawSection(List<GameObject> instances, System.Action addInstanceAction)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("群組 (Instances)", EditorStyles.boldLabel);

            var alignedFieldWidth = EditorGUIUtility.currentViewWidth * 0.25f;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("|編輯名稱", EditorStyles.boldLabel, GUILayout.Width(alignedFieldWidth));
            EditorGUILayout.LabelField("|人流群組", EditorStyles.boldLabel);


            EditorGUILayout.EndHorizontal();

            instances.RemoveAll(item => item == null);

            var toRemove = new List<GameObject>();
            foreach (var instance in instances)
            {
                EditorGUILayout.BeginHorizontal();


                var textProp = EditorGUILayout.TextField(instance.name, GUILayout.Width(alignedFieldWidth));
                if (textProp != instance.name)
                {
                    instance.name = textProp;
                }

                EditorGUILayout.ObjectField("", instance, typeof(GameObject), true);
                if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
                {
                    toRemove.Add(instance);
                }

                EditorGUILayout.EndHorizontal();
            }

            RemoveInstances(instances, toRemove);

            if (GUILayout.Button("Add Group Instance"))
            {
                addInstanceAction?.Invoke();
            }

            EditorGUILayout.EndVertical();
        }

        private static void RemoveInstances(ICollection<GameObject> instances, List<GameObject> toRemove)
        {
            foreach (var remove in toRemove)
            {
                instances.Remove(remove);

                if (remove == null) continue;

                DestroyImmediate(remove);
            }
        }

        #endregion
    }
}