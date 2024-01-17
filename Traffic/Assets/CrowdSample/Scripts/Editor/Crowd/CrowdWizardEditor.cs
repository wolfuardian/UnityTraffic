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
            DrawInitializationSection();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            if (_crowdWizard.IsPathCreated())
            {
                DrawSection("Path Instances", _crowdWizard.pathInstances, _crowdWizard.CreatePathInstance);
            }

            EditorGUILayout.Space(10);

            if (_crowdWizard.IsCrowdAgentCreated())
            {
                DrawSection("Agent Instance", _crowdWizard.crowdAgentInstances, _crowdWizard.CreateCrowdAgentInstance);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInitializationSection()
        {
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
        }

        private static void DrawSection(string label, List<GameObject> instances, System.Action addInstanceAction)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            var toRemove = new List<GameObject>();
            foreach (var instance in instances)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("Instance", instance, typeof(GameObject), true);
                if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
                {
                    toRemove.Add(instance);
                }

                EditorGUILayout.EndHorizontal();
            }

            RemoveInstances(instances, toRemove);

            if (GUILayout.Button($"Add {label}"))
            {
                addInstanceAction?.Invoke();
            }

            EditorGUILayout.EndVertical();
        }

        private static void RemoveInstances(List<GameObject> instances, List<GameObject> toRemove)
        {
            foreach (var remove in toRemove)
            {
                DestroyImmediate(remove);
                instances.Remove(remove);
            }
        }
    }
}