using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdGenerator))]
    public class CrowdGeneratorEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private CrowdGenerator crowdGenerator;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdGenerator = (CrowdGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            DrawInitializationSection();
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical("box");

            // DrawSection(crowdGenerator.GroupInstances, crowdGenerator.AddGroupInstance);

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Private Methods

        private void DrawInitializationSection()
        {
            EditorGUILayout.LabelField("初始化", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(crowdGenerator.IsPathCreated);
            if (GUILayout.Button("Create Path"))
            {
                crowdGenerator.CreatePathSingleton();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(crowdGenerator.IsCrowdCreated);

            if (GUILayout.Button("Create Crowd"))
            {
                crowdGenerator.CreateCrowdSingleton();
            }

            EditorGUI.EndDisabledGroup();

            if (!crowdGenerator.Initialized)
            {
                var errorCount = 0;
                if (!crowdGenerator.IsPathCreated) errorCount++;
                if (!crowdGenerator.IsCrowdCreated) errorCount++;
                EditorGUILayout.HelpBox($"請先完成初始化。還有 {errorCount} 個物件還沒初始化", MessageType.Warning);
                return;
            }
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("設定檔配置", EditorStyles.boldLabel);
            
            // TODO: 這裡要改成可以選擇設定檔
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