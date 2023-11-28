// CrowdWizardEditor.cs

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Runtime.Crowd;

namespace Editor.Crowd
{
    [CustomEditor(typeof(CrowdWizard))]
    public class CrowdWizardEditor : UnityEditor.Editor
    {
        private CrowdWizard _crowdWizard;
        SerializedProperty crowdAgentPrefabsProp;
        SerializedProperty newCrowdAgentPrefabProp;

        private void OnEnable()
        {
            _crowdWizard = (CrowdWizard)target;
            crowdAgentPrefabsProp = serializedObject.FindProperty("crowdAgentPrefabs");
            newCrowdAgentPrefabProp = serializedObject.FindProperty("newCrowdAgentPrefab");
        }

        public override void OnInspectorGUI()
        {
            // var crowdWizard = (CrowdWizard)target;

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

            if (_crowdWizard.IsPathCreated())
            {
                EditorGUILayout.Space(20);
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


            }
            if (GUILayout.Button("Add Path Instance"))
            {
                _crowdWizard.CreatePathInstance();
            }

            if (_crowdWizard.IsCrowdAgentCreated())
            {
                EditorGUILayout.Space(20);
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
            }
            
            if (GUILayout.Button("Add Agent Instance"))
            {
                _crowdWizard.CreateCrowdAgentInstance();
            }
            
            
            
            //
            //
            // for (int i = 0; i < crowdWizard.crowdAgentPrefabs.Count; i++)
            // {
            //     EditorGUILayout.BeginHorizontal();
            //     EditorGUILayout.ObjectField("Prefab", crowdWizard.crowdAgentPrefabs[i], typeof(GameObject), false);
            //
            //     if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
            //     {
            //         toRemove.Add(crowdWizard.crowdAgentPrefabs[i]);
            //     }
            //
            //     EditorGUILayout.EndHorizontal();
            // }
            //
            // foreach (var remove in toRemove)
            // {
            //     crowdWizard.crowdAgentPrefabs.Remove(remove);
            // }
            //
            // EditorGUILayout.Space(20);
            // EditorGUILayout.LabelField("Add Crowd Agent Variant", EditorStyles.boldLabel);
            //
            // EditorGUILayout.BeginHorizontal();
            //
            // EditorGUILayout.PropertyField(newCrowdAgentPrefabProp, new GUIContent("New Prefab"));
            // if (GUILayout.Button("Add to List"))
            // {
            //     crowdWizard.AddCrowdAgentPrefab();
            // }
            //
            // EditorGUILayout.EndHorizontal();
            //
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            // if (crowdWizard.IsCrowdAgentCreated())
            // {
            //     EditorGUILayout.Space();
            //     EditorGUILayout.LabelField("Crowd Agent Prefabs", EditorStyles.boldLabel);
            //
            //     var toRemove = new List<GameObject>();
            //
            //     foreach (var pathInstance in crowdWizard.pathInstances)
            //     {
            //         EditorGUILayout.BeginHorizontal();
            //         EditorGUILayout.ObjectField("Instance", pathInstance, typeof(GameObject), true);
            //         if (GUILayout.Button("Delete", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f)))
            //         {
            //             toRemove.Add(pathInstance);
            //         }
            //
            //         EditorGUILayout.EndHorizontal();
            //     }
            // }


            // EditorGUILayout.Space(20);
            // EditorGUILayout.LabelField("Display Configuration", EditorStyles.boldLabel);
            // EditorGUILayout.BeginHorizontal();
            // if (GUILayout.Button(crowdWizard.IsPointHide ? "Hide Point Mesh" : "Show Point Mesh"))
            // {
            //     TogglePointDisplayMode();
            // }
            //
            // EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }


        // private void TogglePointDisplayMode()
        // {
        //     _CrowdWizard.TogglePointDisplayMode();
        // }
    }
}