using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using SObject;
using Random = UnityEngine.Random;

namespace Runtime.Crowd
{
    public class CrowdAgentFactory : MonoBehaviour
    {
        public CrowdAgentData crowdAgentData;
        public CrowdPath crowdPath;

        [SerializeField] private List<GameObject> crowdAgentPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> crowdAgentInstances = new List<GameObject>();

        public int maxAgentCount = 5;
        public float spawnInterval;
        public float minSpeed;
        public float maxSpeed;
        public float speed;
        public float turningRadius;
        public float stoppingDistance;

        public int currentAgentCount;

        private void OnValidate()
        {
            if (crowdAgentData == null) return;

            crowdAgentPrefabs.Clear();
            crowdAgentPrefabs.AddRange(crowdAgentData.crowdAgentPrefabs);
        }


        private void Awake()
        {
            maxAgentCount = crowdAgentData.maxAgentCount;
            spawnInterval = crowdAgentData.spawnInterval;
            minSpeed = crowdAgentData.minSpeed;
            maxSpeed = crowdAgentData.maxSpeed;
            turningRadius = crowdAgentData.turningRadius;
            stoppingDistance = crowdAgentData.stoppingDistance;
        }


        private void Start()
        {
            if (crowdAgentData == null) return;

            currentAgentCount = 0;
            StartCoroutine(SpawnRoutine());
        }

        IEnumerator SpawnRoutine()
        {
            while (gameObject.activeSelf)
            {
                if (currentAgentCount < maxAgentCount)
                {
                    SpawnCrowdAgent();
                    yield return new WaitForSeconds(spawnInterval);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void SpawnCrowdAgent()
        {
            speed = Random.Range(minSpeed, maxSpeed);

            var prefabIndex = Random.Range(0, crowdAgentPrefabs.Count);
            var crowdAgentPrefab = crowdAgentPrefabs[prefabIndex];
            var crowdAgentInstance =
                Instantiate(crowdAgentPrefabs[prefabIndex], GetSpawnPosition(), Quaternion.identity);
            crowdAgentInstance.transform.SetParent(transform);

            crowdAgentInstance.name = crowdAgentPrefab.name + currentAgentCount;

            crowdAgentInstance.AddComponent<AgentEntity>();

            var instAgentEntity = crowdAgentInstance.GetComponent<AgentEntity>();
            instAgentEntity.onAgentExited += OnCrowdAgentExited;
            instAgentEntity.SetSpeed(speed);
            instAgentEntity.SetTurningRadius(turningRadius);
            instAgentEntity.SetStoppingDistance(stoppingDistance);

            crowdAgentInstance.AddComponent<AgentTracker>();

            var instAgentTracker = crowdAgentInstance.GetComponent<AgentTracker>();
            instAgentTracker.SetAgentEntity(instAgentEntity);
            instAgentTracker.SetCrowdPath(crowdPath);
            instAgentTracker.SetTurningRadius(turningRadius);

            currentAgentCount++;
        }

        private Vector3 GetSpawnPosition()
        {
            var spawnPosition = crowdPath.waypoints[0].transform.position;
            return spawnPosition;
        }

        private void OnCrowdAgentExited(AgentEntity agent)
        {
            currentAgentCount--;
        }
    }
}