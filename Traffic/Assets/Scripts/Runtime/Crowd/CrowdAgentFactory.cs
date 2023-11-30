using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using SObject;

namespace Runtime.Crowd
{
    public class CrowdAgentFactory : MonoBehaviour
    {
        public CrowdAgentData crowdAgentData;
        public CrowdPath crowdPath;

        [SerializeField] private List<GameObject> crowdAgentPrefabs = new List<GameObject>();

        public int currentAgentCount;
        private float _speed;

        private void OnValidate()
        {
            if (crowdAgentData == null) return;

            crowdAgentPrefabs.Clear();
            crowdAgentPrefabs.AddRange(crowdAgentData.crowdAgentPrefabs);
        }


        private void Start()
        {
            if (crowdAgentData == null) return;

            currentAgentCount = 0;
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (gameObject.activeSelf)
            {
                if (currentAgentCount < crowdAgentData.maxAgentCount)
                {
                    SpawnCrowdAgent();
                    yield return new WaitForSeconds(crowdAgentData.spawnInterval);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void SpawnCrowdAgent()
        {
            _speed = Random.Range(crowdAgentData.minSpeed, crowdAgentData.maxSpeed);

            var prefabIndex = Random.Range(0, crowdAgentPrefabs.Count);
            var crowdAgentPrefab = crowdAgentPrefabs[prefabIndex];
            var crowdAgentInstance =
                Instantiate(crowdAgentPrefabs[prefabIndex], GetSpawnPosition(), Quaternion.identity);
            crowdAgentInstance.transform.SetParent(transform);

            crowdAgentInstance.name = crowdAgentPrefab.name + currentAgentCount;

            crowdAgentInstance.AddComponent<AgentEntity>();

            var entity = crowdAgentInstance.GetComponent<AgentEntity>();
            entity.onAgentExited += OnCrowdAgentExited;
            entity.SetSpeed(_speed);
            entity.SetTurningRadius(crowdAgentData.turningRadius);
            entity.SetStoppingDistance(crowdAgentData.stoppingDistance);

            crowdAgentInstance.AddComponent<AgentTracker>();

            var tracker = crowdAgentInstance.GetComponent<AgentTracker>();
            tracker.SetAgentEntity(entity);
            tracker.SetWaypoints(crowdPath.waypoints);
            tracker.SetTurningRadius(crowdAgentData.turningRadius);

            currentAgentCount++;
        }

        private Vector3 GetSpawnPosition()
        {
            var spawnPosition = GetRandomPointInRadius(crowdPath.waypoints[0]);
            return spawnPosition;
        }

        private void OnCrowdAgentExited(AgentEntity agent)
        {
            currentAgentCount--;
        }

        private static Vector3 GetRandomPointInRadius(GameObject point)
        {
            var crowdPathPoint = point.GetComponent<CrowdPathPoint>();
            if (crowdPathPoint != null)
            {
                var radius = crowdPathPoint.allowableRadius;
                var randomDirection = Random.insideUnitSphere * radius;
                var position = point.transform.position;
                randomDirection += position;
                randomDirection.y = position.y;
                return randomDirection;
            }

            return point.transform.position;
        }
    }
}