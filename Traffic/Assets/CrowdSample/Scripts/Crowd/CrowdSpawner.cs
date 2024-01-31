using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdSpawner : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private CrowdPath        m_path;
        [SerializeField] private CrowdAgentConfig m_agentConfig;
        [SerializeField] private bool             m_spawnable = true;

        [SerializeField] private List<Transform> m_trackingAgents = new List<Transform>();
        [SerializeField] private int             m_createdTotalCount;
        [SerializeField] private int             m_queueTotalCount;

        [SerializeField] private int            m_maxIdSoFar;
        private readonly         HashSet<int>   activeIds    = new HashSet<int>();
        private readonly         SortedSet<int> availableIds = new SortedSet<int>();

        // Plug-in
        [SerializeField] private CrowdAgentAddonLicensePlate m_crowdAgentAddonLicensePlate;

        #endregion

        #region Properties

        public CrowdPath path
        {
            get => m_path;
            set => m_path = value;
        }

        public CrowdAgentConfig agentConfig
        {
            get => m_agentConfig;
            set => m_agentConfig = value;
        }

        public bool spawnable
        {
            get => m_spawnable;
            set => m_spawnable = value;
        }

        public List<Transform> trackingAgents => m_trackingAgents;

        public int queueTotalCount
        {
            get => m_queueTotalCount;
            set => m_queueTotalCount = value;
        }

        public int createdTotalCount
        {
            get => m_createdTotalCount;
            set => m_createdTotalCount = value;
        }

        // ID 管理
        private int maxIdSoFar
        {
            get => m_maxIdSoFar;
            set => m_maxIdSoFar = value;
        }


        // Plug-in

        public CrowdAgentAddonLicensePlate crowdAgentAddonLicensePlate
        {
            get => m_crowdAgentAddonLicensePlate;
            set => m_crowdAgentAddonLicensePlate = value;
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            crowdAgentAddonLicensePlate = GetComponent<CrowdAgentAddonLicensePlate>();
            if (crowdAgentAddonLicensePlate) crowdAgentAddonLicensePlate.crowdSpawner = this;
        }

        private void Start()
        {
            if (!spawnable) return;
            StartSpawn();
        }

        #endregion


        #region Private Methods

        private void StartSpawn()
        {
            if (path.crowdPathConfig.spawnOnce)
            {
                SpawnOnce();
            }
            else
            {
                SpawnContinually();
            }
        }

        private void SpawnOnce()
        {
            var spawnPoints = path.spawnPoints;

            if (agentConfig.prefabs.Length == 0)
            {
                Debug.LogError($"[錯誤] 路徑 '{path.name}' 上沒有設定任何代理物件。", path);
                return;
            }

            trackingAgents.Clear();

            foreach (var spawnPoint in spawnPoints)
            {
                var prefabIndex = Random.Range(0, agentConfig.prefabs.Length);
                var prefab      = agentConfig.prefabs[prefabIndex];
                if (prefab == null)
                {
                    Debug.LogError($"[錯誤] 路徑 '{path.name}' 上的代理物件為空。", path);
                    return;
                }

                var parent   = transform;
                var instance = InstantiateAgent(prefab, parent, spawnPoint);
                instance.name                  = prefab.name + queueTotalCount;
                instance.transform.position    = spawnPoint.position;
                instance.transform.eulerAngles = Quaternion.LookRotation(spawnPoint.direction).eulerAngles;
            }
        }

        private void SpawnContinually()
        {
            StartCoroutine(SpawnRoutine());
        }


        private IEnumerator SpawnRoutine()
        {
            var pathConfig  = path.crowdPathConfig;
            var spawnPoints = path.spawnPoints;

            if (agentConfig.prefabs.Length == 0)
            {
                Debug.LogError($"[錯誤] 路徑 '{path.name}' 上沒有設定任何代理物件。", path);
                yield break;
            }

            trackingAgents.Clear();

            while (gameObject.activeSelf)
            {
                if (queueTotalCount < pathConfig.maxSpawnCount)
                {
                    var prefabIndex = Random.Range(0, agentConfig.prefabs.Length);
                    var prefab      = agentConfig.prefabs[prefabIndex];
                    if (prefab == null)
                    {
                        Debug.LogError($"[錯誤] 路徑 '{path.name}' 上的代理物件為空。", path);
                        yield break;
                    }

                    var spawnPoint = spawnPoints[0];

                    var parent   = transform;
                    var instance = InstantiateAgent(prefab, parent, spawnPoint);
                    instance.name                  = prefab.name + queueTotalCount;
                    instance.transform.position    = spawnPoint.position;
                    instance.transform.eulerAngles = Quaternion.LookRotation(spawnPoint.direction).eulerAngles;

                    yield return new WaitForSeconds(pathConfig.spawnInterval);
                }
                else
                {
                    yield return null;
                }

                yield return null;
            }
        }

        public GameObject InstantiateAgent(GameObject instance, Transform parent, SpawnPoint spawnPoint)
        {
            var agentInstance = Instantiate(instance, Vector3.zero, Quaternion.identity, parent);
            if (agentInstance.GetComponent<NavMeshAgent>() == null)
            {
                agentInstance.AddComponent<NavMeshAgent>();
            }

            Configure(agentInstance, spawnPoint);

            return agentInstance;
        }

        private void Configure(GameObject agentInstance, SpawnPoint spawnPoint)
        {
            var pathConfig = path.crowdPathConfig;

            var navigator = agentInstance.AddComponent<CrowdNavigator>();
            if (pathConfig.generationMode == CrowdPathConfig.GenerationMode.InfinityFlow)
            {
                navigator.navigationMode = CrowdNavigator.NavigationMode.Once;
            }
            else
            {
                navigator.navigationMode = pathConfig.pathClosed
                    ? CrowdNavigator.NavigationMode.Loop
                    : CrowdNavigator.NavigationMode.PingPong;
            }

            navigator.waypoints     =  path.waypoints;
            navigator.targetPointID =  spawnPoint.targetPointNum % path.waypoints.Count;
            navigator.shouldDestroy =  pathConfig.shouldDestroy;
            navigator.DestroyEvent  += () => { queueTotalCount--; };
            navigator.DestroyEvent  += () => { trackingAgents.Remove(agentInstance.transform); };
            navigator.DestroyEvent  += () => { ReleaseId(navigator.spawnID); };
            navigator.spawnID       =  AssignNewSpawnId();
            navigator.QueueStateEvent += nav =>
            {
                nav.queueIndex      = trackingAgents.IndexOf(nav.transform);
                nav.queueTotalCount = queueTotalCount;
            };

            var entity = agentInstance.AddComponent<CrowdAgent>();
            entity.navMeshAgent.speed            = Random.Range(agentConfig.minSpeed, agentConfig.maxSpeed);
            entity.navMeshAgent.angularSpeed     = agentConfig.angularSpeed;
            entity.navMeshAgent.acceleration     = agentConfig.acceleration;
            entity.navMeshAgent.stoppingDistance = agentConfig.stoppingDistance;
            entity.navMeshAgent.autoBraking      = agentConfig.autoBraking;
            entity.navMeshAgent.radius           = 1.5f;
            entity.rigidBody.useGravity          = false;
            entity.rigidBody.isKinematic         = true;
            entity.rigidBody.angularDrag         = 0; // 雖然　isKinematic　已經能避免被撞開，但仍有機會被影響旋動量，因此這邊設定為 0

            if (agentConfig.useLicensePlate)
            {
                var addonLicensePlate     = agentInstance.AddComponent<CrowdAgentAddonLicensePlate>();
                var licensePlateNumber    = agentConfig.licensePlateNumber;
                var licensePlateAuthState = agentConfig.licensePlateAuthStates;

                if (agentConfig.useLicensePlateFromCsv)
                {
                    addonLicensePlate.RegistryDataFromCsv(agentConfig.licensePlateCsvPath);
                    licensePlateNumber = addonLicensePlate.GetRandomPlateNumber();
                    var authStateCode = addonLicensePlate.GetAuthState(licensePlateNumber);
                    licensePlateAuthState = Enum.TryParse(authStateCode, out LicensePlateStates parsedState)
                        ? parsedState
                        : LicensePlateStates.Deny;
                }

                addonLicensePlate.licensePlateNumber     = licensePlateNumber;
                addonLicensePlate.licensePlateAuthStates = licensePlateAuthState;
            }

            trackingAgents.Add(agentInstance.transform);

            queueTotalCount++;
            createdTotalCount++;
        }

        private int AssignNewSpawnId()
        {
            if (availableIds.Count == 0)
            {
                maxIdSoFar++;
                return maxIdSoFar;
            }

            var id = availableIds.Min;
            availableIds.Remove(id);
            activeIds.Add(id);
            return id;
        }

        private void ReleaseId(int id)
        {
            activeIds.Remove(id);
            availableIds.Add(id);
        }

        #endregion
    }

    [CustomEditor(typeof(CrowdSpawner))]
    public class CrowdSpawnerEditor : Editor
    {
        #region Field Declarations

        private CrowdSpawner crowdSpawner;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            crowdSpawner = (CrowdSpawner)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(4);

            DrawBackButton();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region GUI Methods

        private void DrawBackButton()
        {
            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button("返回", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f)))
            {
                Selection.activeObject = crowdSpawner.transform.parent;
            }

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}