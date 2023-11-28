using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using ScriptableObject;

namespace Runtime.Crowd
{
    public class CrowdAgentFactory : MonoBehaviour
    {
        public CrowdAgentData crowdAgentData;
        public int maxAgentCount = 5;
        public CrowdPath crowdPath;

        [SerializeField] private List<GameObject> crowdAgentPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> crowdAgentInstances = new List<GameObject>();

        private void OnValidate()
        {
            if (crowdAgentData == null) return;

            crowdAgentPrefabs.Clear();
            crowdAgentPrefabs.AddRange(crowdAgentData.crowdAgentPrefabs);
        }

        private void Start()
        {
            if (crowdAgentData == null) return;

            InstantiateCrowdAgents();
        }

        private void InstantiateCrowdAgents()
        {
            for (var crowdAgent = 0; crowdAgent < maxAgentCount; crowdAgent++)
            {
                InstantiateCrowdAgent(crowdAgent);
            }
        }

        private void InstantiateCrowdAgent(int crowdAgent)
        {
            var randomIndex = Random.Range(0, crowdAgentPrefabs.Count);
            var crowdAgentPrefab = crowdAgentPrefabs[randomIndex];
            var crowdAgentInstance = Instantiate(crowdAgentPrefab, transform);
            crowdAgentInstance.name = crowdAgentPrefab.name + crowdAgent;

            crowdAgentInstance.AddComponent<AgentEntity>();

            var instAgentEntity = crowdAgentInstance.GetComponent<AgentEntity>();
            instAgentEntity.navMeshAgent = crowdAgentInstance.GetComponent<NavMeshAgent>();
            instAgentEntity.SetSpeed(crowdPath.agentSpeed);

            crowdAgentInstance.AddComponent<AgentTracker>();
            
            var instAgentTracker = crowdAgentInstance.GetComponent<AgentTracker>();
            instAgentTracker.agent = instAgentEntity;
            instAgentTracker.crowdPath = crowdPath;

            crowdAgentInstances.Add(crowdAgentInstance);
        }
    }
}