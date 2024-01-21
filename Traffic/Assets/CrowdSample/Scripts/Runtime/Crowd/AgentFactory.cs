using UnityEngine;
using System.Collections;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentFactory : MonoBehaviour, IUpdatable
    {
        public  Path         path;
        private AgentSpawner agentSpawner;

        private Coroutine spawnRoutineCoroutine;


        [SerializeField] private AgentDataConfig agentDataConfig;

        [SerializeField] private AgentGenerationConfig agentGenerationConfig;

        // [SerializeField] private List<GameObject>      crowdAgentPrefabs = new List<GameObject>();
        [SerializeField] private int currentAgentCount;


        public AgentDataConfig AgentDataConfig => agentDataConfig;

        public AgentGenerationConfig AgentGenerationConfig => agentGenerationConfig;

        // public List<GameObject>      CrowdAgentPrefabs     => crowdAgentPrefabs;
        public int CurrentAgentCount => currentAgentCount;

        public void SetCurrentAgentCount(int value) => currentAgentCount = value;

        private bool isSpawnable = true;


        #region Parameter Variables

        [SerializeField] private bool spawnAgentOnce;

        public bool SpawnAgentOnce => spawnAgentOnce;

        public void SetSpawnAgentOnce(bool value) => spawnAgentOnce = value;

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

            agentSpawner = new AgentSpawner(agentDataConfig, path);

            if (spawnAgentOnce)
            {
                for (var currentCount = 0; currentCount < agentGenerationConfig.InstantCount; currentCount++)
                {
                    SpawnAgent(currentCount);
                }
            }
            else
            {
                spawnRoutineCoroutine = StartCoroutine(SpawnRoutine());
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (gameObject.activeSelf)
            {
                // if (CurrentAgentCount < AgentData.maxAgentCount)
                // {
                //     SpawnAgent();
                //     yield return new WaitForSeconds(AgentData.spawnInterval);
                // }
                // else
                // {
                //     yield return null;
                // }
                yield return null;
            }
        }

        private void SpawnAgent()
        {
            var prefabIndex = Random.Range(0, AgentDataConfig.AgentPrefabs.Length);
            Debug.Log(
                "prefabIndex: " + prefabIndex + ", Prefab name: " + AgentDataConfig.AgentPrefabs[prefabIndex].name);
            // var crowdAgentInstance = agentSpawner.SpawnAgent(CrowdAgentPrefabs[prefabIndex], transform);
            // agentSpawner.SetSpawnPosition(pathController.GetRandomPointInRadius());
            // agentSpawner.SetSpawnRotation(pathController.GetRandomRotation());
            // crowdAgentInstance.name += CurrentAgentCount;

            // var entity = crowdAgentInstance.GetComponent<AgentEntity>();
            // entity.onAgentExited += OnCrowdAgentExited;

            SetCurrentAgentCount(CurrentAgentCount + 1);
        }

        private void SpawnAgent(int currentCount)
        {
            var prefabIndex = Random.Range(0, AgentDataConfig.AgentPrefabs.Length);

            Debug.Log("currentCount: " + currentCount);
            Debug.Log(
                "prefabIndex: " + prefabIndex + ", Prefab name: " + AgentDataConfig.AgentPrefabs[prefabIndex].name);
            var agentInst = agentSpawner.SpawnAgent(AgentDataConfig.AgentPrefabs[prefabIndex], transform);
            agentInst.transform.position = SpatialUtils.GetRandomPointInRadius(path.Waypoints[0].transform.position,
                path.Waypoints[0].GetComponent<Waypoint>().Radius);
            agentInst.name += CurrentAgentCount;

            // var entity = crowdAgentInstance.GetComponent<AgentEntity>();
            // entity.onAgentExited += OnCrowdAgentExited;

            SetCurrentAgentCount(CurrentAgentCount + 1);
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
            if (AgentGenerationConfig == null) return;
            SetSpawnAgentOnce(AgentGenerationConfig.IsSpawnAgentOnce);
        }

        private void FetchAgentDataConfig()
        {
            if (AgentDataConfig == null) return;
            // SetCurrentAgentCount(AgentDataConfig.MaxAgentCount);
        }
#endif
    }
}