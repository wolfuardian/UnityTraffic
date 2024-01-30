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

        [SerializeField] private List<GameObject> m_managerInstances = new List<GameObject>();

        #endregion

        #region Properties

        public List<GameObject> managerInstances => m_managerInstances;

        #endregion

        #region Public Methods

        public void AddInstance()
        {
            var newGeneratorInst = new GameObject("Crowd_" + managerInstances.Count);
            newGeneratorInst.transform.SetParent(transform);
            managerInstances.Add(newGeneratorInst);
            ConfigureGeneratorInstance(newGeneratorInst);
        }

        #endregion

        #region Private Methods

        private static void ConfigureGeneratorInstance(GameObject inst)
        {
            var crowdGenerator = inst.AddComponent<CrowdManager>();
            InternalEditorUtility.SetIsInspectorExpanded(crowdGenerator, true);
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdWizard))]
    public class CrowdWizardEditor : Editor
    {
        #region Field Declarations

        private CrowdWizard        crowdWizard;
        private SerializedProperty managerInstancesProp;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdWizard          = (CrowdWizard)target;
            managerInstancesProp = serializedObject.FindProperty("m_managerInstances");
        }

        public override void OnInspectorGUI()
        {
            DrawCrowdWizard("人流設定精靈");
        }

        #endregion

        #region GUI Methods

        private void DrawCrowdWizard(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            DrawManagersTable(crowdWizard.managerInstances, crowdWizard.AddInstance);
            EditorGUILayout.EndVertical();
        }

        private void DrawManagersTable(List<GameObject> instances, System.Action addInstanceAction)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            var alignedFieldWidth = EditorGUIUtility.currentViewWidth * 0.25f;
            EditorGUILayout.LabelField("編輯名稱", EditorStyles.boldLabel, GUILayout.Width(alignedFieldWidth));
            EditorGUILayout.LabelField("人流群組", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            instances.RemoveAll(item => item == null);

            var toRemove = new List<GameObject>();

            for (var i = 0; i < managerInstancesProp.arraySize; i++)
            {
                var managerInst = managerInstancesProp.GetArrayElementAtIndex(i);
                if (managerInst.objectReferenceValue == null) continue; // 跳過已經被刪除的 waypoint, 防止介面卡住
                EditorGUILayout.BeginHorizontal();
                if (managerInst.objectReferenceValue is GameObject component)
                {
                    var textProp = EditorGUILayout.TextField(component.name, GUILayout.Width(alignedFieldWidth));
                    if (textProp != component.name)
                    {
                        component.name = textProp;
                    }

                    var updatedComponent = EditorGUILayout.ObjectField("", component, typeof(GameObject), true);
                    managerInst.objectReferenceValue = updatedComponent;
                    serializedObject.ApplyModifiedProperties();

                    if (GUILayout.Button("前往↗️", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f)))
                    {
                        Selection.activeObject = component;
                    }

                    if (GUILayout.Button("刪️", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.05f)))
                    {
                        var isConfirmed = EditorUtility.DisplayDialog(
                            "確認刪除",
                            "你確定要刪除這個物件嗎？這個操作無法復原。",
                            "刪除",
                            "取消"
                        );
                        if (isConfirmed)
                        {
                            toRemove.Add(component);
                        }
                    }
                }

                var wizardSo = new SerializedObject(crowdWizard);
                wizardSo.ApplyModifiedProperties();

                EditorGUILayout.EndHorizontal();
            }

            UnityUtils.RemoveInstances(instances, toRemove);

            if (GUILayout.Button("新增"))
            {
                addInstanceAction?.Invoke();
            }

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}