using UnityEngine;
using System.Collections;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdAgentFactoryController : MonoBehaviour, IUpdatable
    {
        public                   Path            path;
        private                  CrowdAgentFactory    crowdAgentFactory;
        [SerializeField] private List<Transform> trackingAgents = new List<Transform>();

        private Coroutine spawnRoutineCoroutine;


        [SerializeField] private AgentDataConfig agentDataConfig;

        [SerializeField] private AgentGenerationConfig agentGenerationConfig;

        // [SerializeField] private List<GameObject>      crowdAgentPrefabs = new List<GameObject>();
        [SerializeField] private int currentAgentCount;
        [SerializeField] private int totalCreatedCount;


        public AgentDataConfig AgentDataConfig => agentDataConfig;

        public AgentGenerationConfig AgentGenerationConfig => agentGenerationConfig;

        // public List<GameObject>      CrowdAgentPrefabs     => crowdAgentPrefabs;
        public int CurrentAgentCount => currentAgentCount;
        public int TotalCreatedCount => totalCreatedCount;

        public void SetCurrentAgentCount(int value) => currentAgentCount = value;

        [SerializeField] private bool isSpawnable = true;

        public List<Transform> TrackingAgents => trackingAgents;

        #region Parameter Variables

        //

        #endregion

        private void Start()
        {
            if (path == null)
            {
                Debug.LogError("Script: Path 為空，請確認是否有設定。", this);
                isSpawnable = false;
            }

            if (agentDataConfig == null)
            {
                Debug.LogError("Scriptable object: AgentDataConfig 為空，請確認是否有設定。", this);
                isSpawnable = false;
            }

            if (agentGenerationConfig == null)
            {
                Debug.LogError("Scriptable object: AgentGenerationConfig 為空，請確認是否有設定。", this);
                isSpawnable = false;
            }

            if (!isSpawnable) return;

            crowdAgentFactory = new CrowdAgentFactory(agentDataConfig, path);

            if (path.IsSpawnAgentOnce)
            {
                var agentSpawnData = path.AgentSpawnData;
                foreach (var spawnData in agentSpawnData)
                {
                    var prefabIndex = Random.Range(0, AgentDataConfig.AgentPrefabs.Length);
                    var prefab      = AgentDataConfig.AgentPrefabs[prefabIndex];
                    var parent      = transform;

                    var agentInstance = crowdAgentFactory.InstantiateAgent(prefab, parent, spawnData);


                    agentInstance.transform.position =  spawnData.position;
                    agentInstance.name               += CurrentAgentCount;

                    var entity = agentInstance.GetComponent<AgentEntity>();
                    var follow = agentInstance.GetComponent<CrowdPathFollow>();

                    follow.TargetIndex = path.AgentGenerationConfig.IsReverseDirection
                        ? Mathf.CeilToInt(spawnData.curvePos)
                        : Mathf.FloorToInt(spawnData.curvePos);

                    follow.SetNavigateMode(path.AgentGenerationConfig.IsClosedPath
                        ? CrowdPathFollow.NavigateMode.Loop
                        : CrowdPathFollow.NavigateMode.PingPong);

                    follow.Reverse = path.AgentGenerationConfig.IsReverseDirection;
                }
            }
            else
            {
                spawnRoutineCoroutine = StartCoroutine(SpawnRoutine());
            }
        }

        private IEnumerator SpawnRoutine()
        {
            var createdIndex = 0;

            while (gameObject.activeSelf)
            {
                // Debug.Log("CurrentAgentCount: " + CurrentAgentCount);
                // Debug.Log("agentGenerationConfig.MaxCount: " + agentGenerationConfig.MaxCount);
                if (CurrentAgentCount < agentGenerationConfig.MaxCount)
                {
                    var prefabIndex = Random.Range(0, AgentDataConfig.AgentPrefabs.Length);
                    var prefab      = AgentDataConfig.AgentPrefabs[prefabIndex];
                    var parent      = transform;

                    var spawnData     = path.AgentSpawnData[0];
                    var agentInstance = crowdAgentFactory.InstantiateAgent(prefab, parent, spawnData);


                    agentInstance.transform.position    =  spawnData.position;
                    agentInstance.transform.eulerAngles =  Quaternion.LookRotation(spawnData.direction).eulerAngles;
                    agentInstance.name                  += CurrentAgentCount;

                    trackingAgents.Add(agentInstance.transform);

                    var entity = agentInstance.GetComponent<AgentEntity>();

                    var follow = agentInstance.GetComponent<CrowdPathFollow>();
                    follow.Points = path.Waypoints.Select(waypoint => waypoint.position).ToList();
                    follow.Ranges = path.Waypoints.Select(waypoint => waypoint.GetComponent<Waypoint>().Radius)
                        .ToList();
                    follow.ShouldDestroyOnGoal = true;
                    follow.SetNavigateMode(CrowdPathFollow.NavigateMode.Once);
                    follow.TargetIndex  =  0;
                    follow.DestroyEvent += () => { currentAgentCount--; };
                    follow.DestroyEvent += () => { trackingAgents.Remove(agentInstance.transform); };
                    follow.CreatedIndex =  createdIndex;

                    currentAgentCount++;
                    totalCreatedCount++;
                    createdIndex++;

                    Debug.Log("gentGenerationConfig.SpawnInterval: " + agentGenerationConfig.SpawnInterval);


                    yield return new WaitForSeconds(agentGenerationConfig.SpawnInterval);
                }
                else
                {
                    yield return null;
                }

                yield return null;
            }
        }

        // private void OnCrowdAgentExited(AgentEntity agent)
        // {
        // SetCurrentAgentCount(CurrentAgentCount - 1);
        // }

#if UNITY_EDITOR
        private void Awake()
        {
            if (path == null) path = GetComponent<Path>();
        }

        public void UpdateGizmo()
        {
            FetchGenerationConfigs();
            FetchAgentDataConfig();
        }

        private void FetchGenerationConfigs()
        {
            // if (AgentGenerationConfig == null) return;
            // SetSpawnAgentOnce(AgentGenerationConfig.IsSpawnAgentOnce);
        }

        private void FetchAgentDataConfig()
        {
            if (AgentDataConfig == null) return;
            // SetCurrentAgentCount(AgentDataConfig.MaxAgentCount);
        }
#endif
    }
}