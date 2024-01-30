using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdWizard : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private List<GameObject> m_generatorInstances = new List<GameObject>();

        #endregion

        #region Properties

        public List<GameObject> generatorInstances => m_generatorInstances;

        #endregion

        #region Public Methods

        public void AddGroupInstance()
        {
            var newGeneratorInst = new GameObject("Crowd_" + generatorInstances.Count);
            newGeneratorInst.transform.SetParent(transform);
            m_generatorInstances.Add(newGeneratorInst);
            ConfigureGeneratorInstance(newGeneratorInst);
        }

        #endregion

        #region Private Methods

        private static void ConfigureGeneratorInstance(GameObject inst)
        {
            var crowdGenerator = inst.AddComponent<CrowdInitialization>();
            InternalEditorUtility.SetIsInspectorExpanded(crowdGenerator, true);
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdWizard))]
    public class CrowdWizardEditor : Editor
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
            DrawSection(crowdWizard.generatorInstances, crowdWizard.AddGroupInstance);
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