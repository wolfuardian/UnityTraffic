using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CrowdSample.Scripts.Crowd
{
    [CreateAssetMenu(fileName = "CrowdAgent", menuName = "CrowdWizard/Crowd Agent")]
    public class CrowdAgentConfig : ScriptableObject
    {
        #region Field Declarations

        [SerializeField] private GameObject[] m_prefabs;

        [SerializeField] private float m_minSpeed         = 5f;
        [SerializeField] private float m_maxSpeed         = 6f;
        [SerializeField] private float m_angularSpeed     = 5000f;
        [SerializeField] private float m_acceleration     = 50f;
        [SerializeField] private float m_stoppingDistance = 1f;
        [SerializeField] private bool  m_autoBraking      = true;

        [SerializeField] private PermissionStates m_permissionStates = PermissionStates.Guest;

        [SerializeField] private string m_userType     = "Car";
        [SerializeField] private string m_userIdentity = "XYZ-1234";

        #endregion

        #region Properties

        public GameObject[] prefabs
        {
            get => m_prefabs;
            set => m_prefabs = value;
        }

        public float minSpeed
        {
            get => m_minSpeed;
            set => m_minSpeed = value;
        }

        public float maxSpeed
        {
            get => m_maxSpeed;
            set => m_maxSpeed = value;
        }

        public float angularSpeed
        {
            get => m_angularSpeed;
            set => m_angularSpeed = value;
        }

        public float acceleration
        {
            get => m_acceleration;
            set => m_acceleration = value;
        }

        public float stoppingDistance
        {
            get => m_stoppingDistance;
            set => m_stoppingDistance = value;
        }

        public bool autoBraking
        {
            get => m_autoBraking;
            set => m_autoBraking = value;
        }

        public PermissionStates permissionStates
        {
            get => m_permissionStates;
            set => m_permissionStates = value;
        }

        public string userType
        {
            get => m_userType;
            set => m_userType = value;
        }

        public string userID
        {
            get => m_userIdentity;
            set => m_userIdentity = value;
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdAgentConfig))]
    public class CrowdAgentConfigEditor : Editor
    {
        #region Field Declarations

        private CrowdAgentConfig   crowdAgentConfig;
        private SerializedProperty prefabsProp;
        private SerializedProperty minSpeedProp;
        private SerializedProperty maxSpeedProp;
        private SerializedProperty angularSpeedProp;
        private SerializedProperty accelerationProp;
        private SerializedProperty stoppingDistanceProp;
        private SerializedProperty autoBrakingProp;

        private SerializedProperty permissionStatesProp;

        private SerializedProperty userTypeProp;
        private SerializedProperty userIdentityProp;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdAgentConfig     = (CrowdAgentConfig)target;
            prefabsProp          = serializedObject.FindProperty("m_prefabs");
            minSpeedProp         = serializedObject.FindProperty("m_minSpeed");
            maxSpeedProp         = serializedObject.FindProperty("m_maxSpeed");
            angularSpeedProp     = serializedObject.FindProperty("m_angularSpeed");
            accelerationProp     = serializedObject.FindProperty("m_acceleration");
            stoppingDistanceProp = serializedObject.FindProperty("m_stoppingDistance");
            autoBrakingProp      = serializedObject.FindProperty("m_autoBraking");

            permissionStatesProp = serializedObject.FindProperty("m_permissionStates");

            userTypeProp     = serializedObject.FindProperty("m_userType");
            userIdentityProp = serializedObject.FindProperty("m_userIdentity");
        }

        public override void OnInspectorGUI()
        {
            DrawPrefabConfig("代理模型設定");

            DrawAgentConfig("NavAgentMesh 物件設定");

            DrawUserConfig("使用者資料設定");

            if (GUI.changed)
            {
                CrowdUtils.UpdateAllReceiverImmediately();
            }

            SceneView.RepaintAll();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        private void DrawPrefabConfig(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(prefabsProp, new GUIContent("模型資源"));
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private void DrawAgentConfig(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var shouldDisplayWarning = prefabsProp.arraySize == 0 ||
                                       Enumerable.Range(0, prefabsProp.arraySize)
                                           .Select(index => prefabsProp.GetArrayElementAtIndex(index))
                                           .Any(item => item.objectReferenceValue == null);
            if (shouldDisplayWarning)
            {
                EditorGUILayout.HelpBox("請確認所有資源都已正確設置。", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(minSpeedProp,         new GUIContent("最小速度"));
            EditorGUILayout.PropertyField(maxSpeedProp,         new GUIContent("最大速度"));
            EditorGUILayout.PropertyField(angularSpeedProp,     new GUIContent("轉動速度"));
            EditorGUILayout.PropertyField(accelerationProp,     new GUIContent("加速度"));
            EditorGUILayout.PropertyField(stoppingDistanceProp, new GUIContent("停止距離"));
            EditorGUILayout.PropertyField(autoBrakingProp,      new GUIContent("自動煞車"));
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private void DrawUserConfig(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(permissionStatesProp, new GUIContent("權限狀態"));
            EditorGUILayout.PropertyField(userTypeProp,         new GUIContent("使用者類型"));
            EditorGUILayout.PropertyField(userIdentityProp,     new GUIContent("使用者 ID"));
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        #endregion
    }
}