using UnityEngine;
using System.Collections;
using CrowdSample.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdFactoryController : MonoBehaviour, IUpdateReceiver
    {
        #region Field Declarations

        //

        #endregion

        #region Properties

        //

        #endregion

        #region Unity Methods

        //

        #endregion

        #region Public Methods

        //

        #endregion

        #region Private Methods

        //

        #endregion

        #region Unity Event Methods

        //

        #endregion

        #region Debug and Visualization Methods

        //

        #endregion


        public                   CrowdPathController crowdPathController;
        private                  CrowdFactory        crowdFactory;
        [SerializeField] private List<Transform>     trackingAgents = new List<Transform>();

        private Coroutine spawnRoutineCoroutine;


        [SerializeField] private CrowdAgentConfig crowdAgentConfig;

        [SerializeField] private CrowdGenerationConfig crowdGenerationConfig;

        // [SerializeField] private List<GameObject>      crowdAgentPrefabs = new List<GameObject>();
        [SerializeField] private int currentAgentCount;
        [SerializeField] private int totalCreatedCount;

        [SerializeField] private bool isSpawnAgentOnce;
        [SerializeField] private bool isReverseDirection;
        [SerializeField] private bool isClosedPath;


        public CrowdAgentConfig CrowdAgentConfig => crowdAgentConfig;

        public CrowdGenerationConfig CrowdGenerationConfig => crowdGenerationConfig;

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
            if (crowdPathController == null)
            {
                Debug.LogError("Script: Path 為空，請確認是否有設定。", this);
                isSpawnable = false;
            }

            if (crowdAgentConfig == null)
            {
                Debug.LogError("Scriptable object: AgentDataConfig 為空，請確認是否有設定。", this);
                isSpawnable = false;
            }

            if (crowdGenerationConfig == null)
            {
                Debug.LogError("Scriptable object: AgentGenerationConfig 為空，請確認是否有設定。", this);
                isSpawnable = false;
            }

            if (!isSpawnable) return;

            crowdFactory = new CrowdFactory(crowdAgentConfig, crowdPathController);

            if (crowdPathController.IsSpawnAgentOnce)
            {
                var agentSpawnData = crowdPathController.AgentSpawnData;
                foreach (var spawnData in agentSpawnData)
                {
                    var prefabIndex = Random.Range(0, CrowdAgentConfig.AgentPrefabs.Length);
                    var prefab      = CrowdAgentConfig.AgentPrefabs[prefabIndex];
                    var parent      = transform;

                    var agentInstance = crowdFactory.InstantiateAgent(prefab, parent, spawnData);


                    agentInstance.transform.position =  spawnData.position;
                    agentInstance.name               += CurrentAgentCount;

                    var entity = agentInstance.GetComponent<AgentEntity>();
                    var follow = agentInstance.GetComponent<CrowdPathFollow>();

                    follow.TargetIndex = isReverseDirection
                        ? Mathf.CeilToInt(spawnData.curvePos)
                        : Mathf.FloorToInt(spawnData.curvePos);

                    follow.SetNavigateMode(isClosedPath
                        ? CrowdPathFollow.NavigateMode.Loop
                        : CrowdPathFollow.NavigateMode.PingPong);

                    follow.Reverse = isReverseDirection;
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
                if (CurrentAgentCount < crowdGenerationConfig.MaxCount)
                {
                    var prefabIndex = Random.Range(0, CrowdAgentConfig.AgentPrefabs.Length);
                    var prefab      = CrowdAgentConfig.AgentPrefabs[prefabIndex];
                    var parent      = transform;

                    var spawnData     = crowdPathController.AgentSpawnData[0];
                    var agentInstance = crowdFactory.InstantiateAgent(prefab, parent, spawnData);


                    agentInstance.transform.position    =  spawnData.position;
                    agentInstance.transform.eulerAngles =  Quaternion.LookRotation(spawnData.direction).eulerAngles;
                    agentInstance.name                  += CurrentAgentCount;

                    trackingAgents.Add(agentInstance.transform);

                    var entity = agentInstance.GetComponent<AgentEntity>();

                    var follow = agentInstance.GetComponent<CrowdPathFollow>();
                    follow.PointsSet           = crowdPathController.PointsSet;
                    follow.RadiusSet           = crowdPathController.RadiusSet;
                    follow.ShouldDestroyOnGoal = true;
                    follow.SetNavigateMode(CrowdPathFollow.NavigateMode.Once);
                    follow.TargetIndex  =  0;
                    follow.DestroyEvent += () => { currentAgentCount--; };
                    follow.DestroyEvent += () => { trackingAgents.Remove(agentInstance.transform); };
                    follow.CreatedIndex =  createdIndex;

                    currentAgentCount++;
                    totalCreatedCount++;
                    createdIndex++;

                    Debug.Log("gentGenerationConfig.SpawnInterval: " + crowdGenerationConfig.SpawnInterval);


                    yield return new WaitForSeconds(crowdGenerationConfig.SpawnInterval);
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
            if (crowdPathController == null) crowdPathController = GetComponent<CrowdPathController>();
        }

        public void UpdateImmediately()
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
            if (CrowdAgentConfig == null) return;
            // SetCurrentAgentCount(AgentDataConfig.MaxAgentCount);
        }
#endif
    }
}