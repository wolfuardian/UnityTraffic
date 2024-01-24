using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Crowd;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdWizard))]
    public class CrowdWizardEditor : UnityEditor.Editor
    {
        #region Field Declarations

        private CrowdWizard crowdWizard;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdWizard = (CrowdWizard)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            DrawInitializationSection();
            EditorGUILayout.EndVertical();
        }

        #endregion


        #region Private Methods

        private void DrawInitializationSection()
        {
            EditorGUILayout.LabelField("初始化", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(crowdWizard.Initialized);
            if (GUILayout.Button("Create GroupRoot"))
            {
                crowdWizard.CreateGroupRoot();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);

            if (crowdWizard.Initialized)
            {
                DrawSection(crowdWizard.groupInstances, crowdWizard.AddGroupInstance);
            }
        }

        private void DrawSection(List<GameObject> instances, System.Action addInstanceAction)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("群組 (Instances)", EditorStyles.boldLabel);

            var alignedFieldWidth = EditorGUIUtility.currentViewWidth * 0.25f;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("|編輯名稱", EditorStyles.boldLabel, GUILayout.Width(alignedFieldWidth));
            EditorGUILayout.LabelField("|人流群組", EditorStyles.boldLabel);


            EditorGUILayout.EndHorizontal();

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