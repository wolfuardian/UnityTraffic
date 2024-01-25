﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using CrowdSample.Scripts.Utils;
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
            DrawSection(crowdWizard.GroupInstances, crowdWizard.AddGroupInstance);
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region GUI Methods

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

            UnityUtils.RemoveInstances(instances, toRemove);

            if (GUILayout.Button("Add Group Instance"))
            {
                addInstanceAction?.Invoke();
            }

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}