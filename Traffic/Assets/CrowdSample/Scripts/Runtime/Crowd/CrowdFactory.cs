using System.Linq;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdFactory
    {
        private readonly CrowdAgentConfig    config;
        private readonly CrowdPathController crowdPathController;

        public CrowdFactory(CrowdAgentConfig config, CrowdPathController crowdPathController)
        {
            this.config              = config;
            this.crowdPathController = crowdPathController;
        }

        public GameObject InstantiateAgent(GameObject prefab, Transform parent, AgentSpawnData spawnData)
        {
            var agentInstance = Object.Instantiate(prefab, GetSpawnPosition(), Quaternion.identity, parent);
            agentInstance.name = prefab.name;

            ConfigureAgentEntity(agentInstance, spawnData);
            // ConfigureAgentTracker(agentInstance, spawnData);
            ConfigurePathFollow(agentInstance, spawnData);

            return agentInstance;
        }

        private void ConfigureAgentEntity(GameObject agentInstance, AgentSpawnData spawnData)
        {
            var entity = agentInstance.AddComponent<AgentEntity>();
            entity.SetShouldNotDestroy(crowdPathController.IsSpawnAgentOnce);
            entity.SetSpeed(Random.Range(config.MinSpeed, config.MaxSpeed));
            entity.SetAgentID(config.permissionID);
            entity.SetAngularSpeed(config.AngularSpeed);
            entity.SetAcceleration(config.Acceleration);
            entity.SetStoppingDistance(config.StoppingDistance);
            entity.SetLicensePlateNumber(GeneratorUtils.GenerateLicensePlateNumber());
            entity.NavMeshAgent.autoBraking = false;
        }

        private void ConfigurePathFollow(GameObject agentInstance, AgentSpawnData spawnData)
        {
            var follow = agentInstance.AddComponent<CrowdPathFollow>();
            follow.PointsSet           = crowdPathController.PointsSet;
            follow.RadiusSet           = crowdPathController.RadiusSet;
            follow.ShouldDestroyOnGoal = true;
            follow.TargetIndex         = Mathf.FloorToInt(spawnData.curvePos) % crowdPathController.PointsSet.Count;
            // follow.TargetIndex  = spawnData.targetIndex;
        }

        private Vector3 GetSpawnPosition()
        {
            var point         = crowdPathController.PointsSet[0];
            var radius        = crowdPathController.RadiusSet[0];
            var spawnPosition = SpatialUtils.GetRandomPointInRadius(point, radius);
            return spawnPosition;
        }
    }
}