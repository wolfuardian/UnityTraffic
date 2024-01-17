using CrowdSample.Scripts.Data;
using CrowdSample.Scripts.Utils;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentSpawner
    {
        private readonly CrowdAgentData _crowdAgentData;
        private readonly CrowdPath      _crowdPath;

        public AgentSpawner(CrowdAgentData crowdAgentData, CrowdPath crowdPath)
        {
            _crowdAgentData = crowdAgentData;
            _crowdPath      = crowdPath;
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
            entity.SetSpeed(Random.Range(_crowdAgentData.minSpeed, _crowdAgentData.maxSpeed));
            entity.SetAgentID(_crowdAgentData.permissionID);
            entity.SetAngularSpeed(_crowdAgentData.angularSpeed);
            entity.SetAcceleration(_crowdAgentData.acceleration);
            entity.SetTurningRadius(_crowdAgentData.turningRadius);
            entity.SetStoppingDistance(_crowdAgentData.stoppingDistance);
            entity.SetLicensePlateNumber(GeneratorUtils.GenerateLicensePlateNumber());
        }

        private void ConfigureAgentTracker(GameObject agentInstance)
        {
            var tracker = agentInstance.AddComponent<AgentTracker>();
            tracker.SetAgentEntity(agentInstance.GetComponent<AgentEntity>());
            tracker.SetCrowdPath(_crowdPath);
            tracker.InitializeWaypoint();
            tracker.SetTurningRadius(_crowdAgentData.turningRadius);
        }

        private Vector3 GetSpawnPosition()
        {
            var point          = _crowdPath.waypoints[0];
            var crowdPathPoint = point.GetComponent<CrowdPathPoint>();
            var radius         = crowdPathPoint.allowableRadius;
            var position       = point.transform.position;
            var spawnPosition  = SpatialUtils.GetRandomPointInRadius(position, radius);
            return spawnPosition;
        }
    }
}