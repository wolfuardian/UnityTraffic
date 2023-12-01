using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Crowd;
using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdWizard))]
    public class CrowdWizardEditor : UnityEditor.Editor
    {
        private CrowdWizard _crowdWizard;

        private void OnEnable()
        {
            _crowdWizard = (CrowdWizard)target;
            serializedObject.FindProperty("crowdAgentPrefabs");
            serializedObject.FindProperty("newCrowdAgentPrefab");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");


            EditorGUILayout.LabelField("Initialization", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(_crowdWizard.IsPathCreated());
            if (GUILayout.Button("Create Path"))
            {
                _crowdWizard.CreatePath();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(_crowdWizard.IsCrowdAgentCreated());
            if (GUILayout.Button("Create Agent"))
            {
                _crowdWizard.CreateCrowdAgent();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            if (_crowdWizard.IsPathCreated())
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("Path Instances", EditorStyles.boldLabel);


                var toRemove = new List<GameObject>();

                foreach (var pathInstance in _crowdWizard.pathInstances)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("Instance", pathInstance, typeof(GameObject), true);
                    if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
                    {
                        toRemove.Add(pathInstance);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                foreach (var remove in toRemove)
                {
                    DestroyImmediate(remove);
                    _crowdWizard.pathInstances.Remove(remove);
                }

                if (GUILayout.Button("Add Path Instance"))
                {
                    _crowdWizard.CreatePathInstance();
                }

                EditorGUILayout.EndVertical();
            }


            EditorGUILayout.Space(10);

            if (_crowdWizard.IsCrowdAgentCreated())
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("Agent Instance", EditorStyles.boldLabel);
                var toRemove = new List<GameObject>();

                foreach (var crowdAgentInstance in _crowdWizard.crowdAgentInstances)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("Instance", crowdAgentInstance, typeof(GameObject), true);
                    if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
                    {
                        toRemove.Add(crowdAgentInstance);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                foreach (var remove in toRemove)
                {
                    DestroyImmediate(remove);
                    _crowdWizard.crowdAgentInstances.Remove(remove);
                }

                if (GUILayout.Button("Add Agent Instance"))
                {
                    _crowdWizard.CreateCrowdAgentInstance();
                }

                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}