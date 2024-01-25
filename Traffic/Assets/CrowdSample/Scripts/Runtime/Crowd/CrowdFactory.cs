using System.Linq;
using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdFactory
    {
        private readonly CrowdAgentConfig config;
        private readonly CrowdPath            crowdPath;

        public CrowdFactory(CrowdAgentConfig config, CrowdPath crowdPath)
        {
            this.config = config;
            this.crowdPath   = crowdPath;
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
            entity.SetShouldNotDestroy(crowdPath.IsSpawnAgentOnce);
            entity.SetSpeed(Random.Range(config.MinSpeed, config.MaxSpeed));
            entity.SetAgentID(config.permissionID);
            entity.SetAngularSpeed(config.AngularSpeed);
            entity.SetAcceleration(config.Acceleration);
            entity.SetStoppingDistance(config.StoppingDistance);
            entity.SetLicensePlateNumber(GeneratorUtils.GenerateLicensePlateNumber());
            entity.NavMeshAgent.autoBraking = false;
        }

        private void ConfigureAgentTracker(GameObject agentInstance, AgentSpawnData spawnData)
        {
            var tracker = agentInstance.AddComponent<AgentTracker>();
            tracker.AgentEntityInstance = agentInstance.GetComponent<AgentEntity>();
            tracker.GlobalJourney       = spawnData.curvePos;
        }

        private void ConfigurePathFollow(GameObject agentInstance, AgentSpawnData spawnData)
        {
            var follow = agentInstance.AddComponent<CrowdPathFollow>();
            follow.Points = crowdPath.Waypoints.Select(waypoint => waypoint.position).ToList();
            follow.Ranges = crowdPath.Waypoints.Select(waypoint => waypoint.GetComponent<Waypoint>().Radius).ToList();
            follow.ShouldDestroyOnGoal = true;
            follow.TargetIndex = Mathf.FloorToInt(spawnData.curvePos) % crowdPath.Waypoints.Count;
            // follow.TargetIndex  = spawnData.targetIndex;
        }

        private Vector3 GetSpawnPosition()
        {
            var point          = crowdPath.Waypoints[0];
            var crowdPathPoint = point.GetComponent<Waypoint>();
            var radius         = crowdPathPoint.Radius;
            var position       = point.transform.position;
            var spawnPosition  = SpatialUtils.GetRandomPointInRadius(position, radius);
            return spawnPosition;
        }
    }
}