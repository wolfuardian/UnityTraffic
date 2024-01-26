using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdFactoryController : MonoBehaviour
    {
        #region Field Declarations

        private CrowdFactory crowdFactory;
        private Coroutine    spawnRoutineCoroutine;

        [SerializeField] private GameObject[]     m_agentPrefabs;
        [SerializeField] private SpawnPointData[] m_spawnPointsData;
        [SerializeField] private List<Transform>  m_trackingAgents = new List<Transform>();
        [SerializeField] private int              m_createdCount;
        [SerializeField] private int              m_createdCountTotal;
        [SerializeField] private bool             m_spawnable;
        [SerializeField] private bool             m_spawnAgentOnce;
        [SerializeField] private float            m_spawnInterval;
        [SerializeField] private int              m_maxSpawnCount;

        #endregion

        #region Properties

        public GameObject[] agentPrefabs
        {
            get => m_agentPrefabs ??= Array.Empty<GameObject>();
            set => m_agentPrefabs = value;
        }


        public List<Transform> trackingAgents
        {
            get => m_trackingAgents;
            set => m_trackingAgents = value;
        }

        public bool spawnable
        {
            get => m_spawnable;
            set => m_spawnable = value;
        }

        public bool spawnAgentOnce
        {
            get => m_spawnAgentOnce;
            set => m_spawnAgentOnce = value;
        }

        public float spawnInterval
        {
            get => m_spawnInterval;
            set => m_spawnInterval = value;
        }

        public int maxSpawnCount
        {
            get => m_maxSpawnCount;
            set => m_maxSpawnCount = value;
        }

        public float                        stoppingDistance    { get; set; }
        public float                        speed               { get; set; }
        public float                        angularSpeed        { get; set; }
        public float                        acceleration        { get; set; }
        public bool                         autoBraking         { get; set; }
        public string                       agentID             { get; set; }
        public string                       type                { get; set; }
        public string                       category            { get; set; }
        public string                       alias               { get; set; }
        public string                       model               { get; set; }
        public string                       time                { get; set; }
        public string                       noted               { get; set; }
        public SpawnPointData[]             spawnPointsData     { get; set; }
        public bool                         shouldDestroyOnGoal { get; set; }
        public bool                         reverse             { get; set; }
        public bool                         closedLoop          { get; set; }
        public int                          targetIndex         { get; set; }
        public CrowdPathFollow.NavigateMode navigateMode        { get; set; }
        public int                          createdCount        => m_createdCount;
        public int                          createdCountTotal   => m_createdCountTotal;

        #endregion

        #region Unity Methods

        private void Start()
        {
            m_spawnable = false;

            if (!m_spawnable) return;

            ConstructFactory();
            StartSpawn();
        }

        #endregion

        #region Private Methods

        private void ConstructFactory()
        {
            var navMeshAgentData = new NavMeshAgentData()
            {
                m_stoppingDistance = stoppingDistance,
                m_speed            = speed,
                m_angularSpeed     = angularSpeed,
                m_acceleration     = acceleration,
                m_autoBraking      = autoBraking
            };

            var agentData = new AgentData()
            {
                m_agentID  = agentID,
                m_type     = type,
                m_category = category,
                m_alias    = alias,
                m_model    = model,
                m_time     = time,
                m_noted    = noted
            };
            var pathFollowData = new PathFollowData()
            {
                m_spawnPointsData     = spawnPointsData,
                m_shouldDestroyOnGoal = shouldDestroyOnGoal,
                m_reverse             = reverse,
                m_closedLoop          = closedLoop,
                m_targetIndex         = targetIndex,
                m_navigateMode        = navigateMode
            };
            var crowdFactoryConfig = new CrowdFactoryConfig()
            {
                m_navMeshAgentData = navMeshAgentData,
                m_agentData        = agentData,
                m_pathFollowData   = pathFollowData
            };

            crowdFactory = new CrowdFactory(crowdFactoryConfig);
        }

        private void StartSpawn()
        {
            if (m_spawnAgentOnce)
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
            foreach (var spawnData in m_spawnPointsData)
            {
                var prefabIndex = Random.Range(0, m_agentPrefabs.Length);
                var prefab      = m_agentPrefabs[prefabIndex];
                var parent      = transform;

                var agentInstance = crowdFactory.InstantiateAgent(prefab, parent);
                agentInstance.name                  = prefab.name + createdCount;
                agentInstance.transform.position    = spawnData.m_position;
                agentInstance.transform.eulerAngles = Quaternion.LookRotation(spawnData.m_direction).eulerAngles;

                m_trackingAgents.Clear();
                m_trackingAgents.Add(agentInstance.transform);

                var follow = agentInstance.GetComponent<CrowdPathFollow>();

                follow.targetIndex = reverse
                    ? Mathf.CeilToInt(spawnData.m_pathLocation)
                    : Mathf.FloorToInt(spawnData.m_pathLocation);

                follow.SetNavigateMode(closedLoop
                    ? CrowdPathFollow.NavigateMode.Loop
                    : CrowdPathFollow.NavigateMode.PingPong);

                follow.reverse = reverse;
            }
        }

        private void SpawnContinually()
        {
            spawnRoutineCoroutine = StartCoroutine(SpawnRoutine());
        }


        private IEnumerator SpawnRoutine()
        {
            var createdIndex = 0;
            m_trackingAgents.Clear();

            while (gameObject.activeSelf)
            {
                if (createdCount < maxSpawnCount)
                {
                    var prefabIndex = Random.Range(0, m_agentPrefabs.Length);
                    var prefab      = m_agentPrefabs[prefabIndex];
                    var parent      = transform;

                    var spawnData = m_spawnPointsData[0];

                    var agentInstance = crowdFactory.InstantiateAgent(prefab, parent);
                    agentInstance.name                  = prefab.name + createdCount;
                    agentInstance.transform.position    = spawnData.m_position;
                    agentInstance.transform.eulerAngles = Quaternion.LookRotation(spawnData.m_direction).eulerAngles;

                    m_trackingAgents.Add(agentInstance.transform);

                    var follow = agentInstance.GetComponent<CrowdPathFollow>();
                    follow.shouldDestroyOnGoal = true;
                    follow.SetNavigateMode(CrowdPathFollow.NavigateMode.Once);
                    follow.targetIndex  =  0;
                    follow.DestroyEvent += () => { m_createdCount--; };
                    follow.DestroyEvent += () => { m_trackingAgents.Remove(agentInstance.transform); };
                    follow.createdIndex =  createdIndex;

                    m_createdCount++;
                    m_createdCountTotal++;
                    createdIndex++;

                    yield return new WaitForSeconds(m_spawnInterval);
                }
                else
                {
                    yield return null;
                }

                yield return null;
            }
        }

        #endregion
    }
}