using UnityEngine;
using CrowdSample.Scripts.Utils;
using CrowdSample.Scripts.Runtime.Data;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentSpawner
    {
        private readonly AgentDataConfig config;
        private readonly Path            path;

        public AgentSpawner(AgentDataConfig config, Path path)
        {
            this.config = config;
            this.path   = path;
        }

        public GameObject SpawnAgent(GameObject prefab, Transform parent)
        {
            var agentInstance = Object.Instantiate(prefab, GetSpawnPosition(), Quaternion.identity, parent);
            agentInstance.name = prefab.name;

            ConfigureAgentEntity(agentInstance);
            ConfigureAgentTracker(agentInstance);

            return agentInstance;
        }

        private void ConfigureAgentEntity(GameObject agentInstance)
        {
            var entity = agentInstance.AddComponent<AgentEntity>();
            entity.SetSpeed(Random.Range(config.MinSpeed, config.MaxSpeed));
            entity.SetAgentID(config.permissionID);
            entity.SetAngularSpeed(config.AngularSpeed);
            entity.SetAcceleration(config.Acceleration);
            entity.SetTurningRadius(config.TurningRadius);
            entity.SetStoppingDistance(config.StoppingDistance);
            entity.SetLicensePlateNumber(GeneratorUtils.GenerateLicensePlateNumber());
        }

        private void ConfigureAgentTracker(GameObject agentInstance)
        {
            var tracker = agentInstance.AddComponent<AgentTracker>();
            tracker.SetAgentEntity(agentInstance.GetComponent<AgentEntity>());
            tracker.SetPath(path);
            tracker.InitializeWaypoint();
            tracker.SetTurningRadius(config.TurningRadius);
        }

        private Vector3 GetSpawnPosition()
        {
            var point          = path.Waypoints[0];
            var crowdPathPoint = point.GetComponent<Waypoint>();
            var radius         = crowdPathPoint.Radius;
            var position       = point.transform.position;
            var spawnPosition  = SpatialUtils.GetRandomPointInRadius(position, radius);
            return spawnPosition;
        }
    }
}