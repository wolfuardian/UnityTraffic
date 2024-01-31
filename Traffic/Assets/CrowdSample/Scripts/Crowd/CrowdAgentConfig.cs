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


        [SerializeField] private string m_userType = "Car";


        // Plug-in
        [SerializeField] private bool               m_useLicensePlate;
        [SerializeField] private bool               m_useLicensePlateFromCsv;
        [SerializeField] private string             m_licensePlateCsvPath    = "Assets/AGVSample/Data/LicensePlate.csv";
        [SerializeField] private string             m_licensePlateNumber     = "XYZ-1234";
        [SerializeField] private LicensePlateStates m_licensePlateAuthStates = LicensePlateStates.Guest;

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

        public LicensePlateStates licensePlateAuthStates
        {
            get => m_licensePlateAuthStates;
            set => m_licensePlateAuthStates = value;
        }

        public string userType
        {
            get => m_userType;
            set => m_userType = value;
        }

        public string licensePlateNumber
        {
            get => m_licensePlateNumber;
            set => m_licensePlateNumber = value;
        }

        public bool useLicensePlate
        {
            get => m_useLicensePlate;
            set => m_useLicensePlate = value;
        }

        public bool useLicensePlateFromCsv
        {
            get => m_useLicensePlateFromCsv;
            set => m_useLicensePlateFromCsv = value;
        }

        public string licensePlateCsvPath
        {
            get => m_licensePlateCsvPath;
            set => m_licensePlateCsvPath = value;
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

        // Plug-in
        private SerializedProperty useLicensePlateProp;
        private SerializedProperty useLicensePlateFromCsvProp;
        private SerializedProperty licensePlateCsvPathProp;
        private SerializedProperty licensePlateProp;
        private SerializedProperty licensePlateStatesProp;

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


            // Plug-in
            useLicensePlateProp        = serializedObject.FindProperty("m_useLicensePlate");
            useLicensePlateFromCsvProp = serializedObject.FindProperty("m_useLicensePlateFromCsv");
            licensePlateCsvPathProp    = serializedObject.FindProperty("m_licensePlateCsvPath");
            licensePlateProp           = serializedObject.FindProperty("m_licensePlateNumber");
            licensePlateStatesProp     = serializedObject.FindProperty("m_licensePlateAuthStates");
        }

        public override void OnInspectorGUI()
        {
            DrawPrefabConfig("代理模型設定");

            DrawAgentConfig("NavAgentMesh 物件設定");

            DrawUserConfig("使用者資料設定");

            DrawPluginLicensePlateConfig("車牌驗證設定");

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

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private void DrawPluginLicensePlateConfig(string label)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            useLicensePlateProp.boolValue = EditorGUILayout.Toggle("使用車牌資料", useLicensePlateProp.boolValue);
            if (useLicensePlateProp.boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(useLicensePlateFromCsvProp.boolValue);
                EditorGUILayout.PropertyField(licensePlateProp,       new GUIContent("車牌號碼"));
                EditorGUILayout.PropertyField(licensePlateStatesProp, new GUIContent("權限狀態"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(4);

                useLicensePlateFromCsvProp.boolValue =
                    EditorGUILayout.Toggle("使用車牌資料庫", useLicensePlateFromCsvProp.boolValue);
                if (useLicensePlateFromCsvProp.boolValue)
                {
                    EditorGUILayout.BeginVertical("box");
                    licensePlateCsvPathProp.stringValue =
                        EditorGUILayout.TextField("車牌資料路徑", licensePlateCsvPathProp.stringValue);

                    EditorGUILayout.Space(4);


                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField("格式範例：", EditorStyles.miniBoldLabel);

                    EditorGUILayout.Space(4);

                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label("    PID",  EditorStyles.label, GUILayout.Width(100));
                    GUILayout.Label("    AUTH", EditorStyles.label, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label("    ABC-123", EditorStyles.label, GUILayout.Width(100));
                    GUILayout.Label("    0",       EditorStyles.label, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label("    XYZ-789", EditorStyles.label, GUILayout.Width(100));
                    GUILayout.Label("    1",       EditorStyles.label, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label("    HJK-456", EditorStyles.label, GUILayout.Width(100));
                    GUILayout.Label("    2",       EditorStyles.label, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space(4);

                    EditorGUILayout.LabelField("權限狀態: 0 = Admin, 1 = Guest, 2 = Deny", EditorStyles.miniLabel);
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}