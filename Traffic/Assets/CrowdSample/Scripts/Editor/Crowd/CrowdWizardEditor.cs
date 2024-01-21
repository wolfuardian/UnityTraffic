using System.Collections.Generic;
using CrowdSample.Scripts.Runtime.Crowd;
using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Editor.Crowd
{
    [CustomEditor(typeof(CrowdWizard))]
    public class CrowdWizardEditor : UnityEditor.Editor
    {
        private CrowdWizard crowdWizard;

        private void OnEnable()
        {
            crowdWizard = (CrowdWizard)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            DrawInitializationSection();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            if (crowdWizard.IsPathCreated)
            {
                DrawSection("Path Instances", crowdWizard.pathInstances, crowdWizard.OnCreatePathInstance);
            }

            EditorGUILayout.Space(10);

            if (crowdWizard.IsAgentCreated)
            {
                DrawSection("Agent Instance", crowdWizard.agentInstances, crowdWizard.OnCreateAgentInstance);
            }

            // UnityEditorUtils.UpdateAllGizmos();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInitializationSection()
        {
            EditorGUILayout.LabelField("Initialization", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(crowdWizard.IsPathCreated);
            if (GUILayout.Button("Create Path"))
            {
                crowdWizard.OnCreatePath();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(crowdWizard.IsAgentCreated);
            if (GUILayout.Button("Create Agent"))
            {
                crowdWizard.OnCreateAgent();
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

        private static void RemoveInstances(ICollection<GameObject> instances, List<GameObject> toRemove)
        {
            foreach (var remove in toRemove)
            {
                instances.Remove(remove);
                
                if (remove == null) continue;
                
                DestroyImmediate(remove);
            }
        }
    }
}