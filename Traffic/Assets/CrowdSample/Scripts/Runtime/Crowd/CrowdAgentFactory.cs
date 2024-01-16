using System.Collections;
using System.Collections.Generic;
using CrowdSample.Scripts.Data;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class CrowdAgentFactory : MonoBehaviour
    {
        public CrowdAgentData crowdAgentData;
        public CrowdPath      crowdPath;

        [SerializeField] private List<GameObject> crowdAgentPrefabs = new List<GameObject>();

        public  int       currentAgentCount;
        private float     _speed;
        private float     _angularSpeed;
        private float     _acceleration;
        private Coroutine _spawnRoutineCoroutine;

        private void OnValidate()
        {
            if (crowdAgentData == null) return;

            crowdAgentPrefabs.Clear();
            crowdAgentPrefabs.AddRange(crowdAgentData.crowdAgentPrefabs);
        }


        private void Start()
        {
            if (crowdAgentData == null) return;
            currentAgentCount      =   0;
            _spawnRoutineCoroutine ??= StartCoroutine(SpawnRoutine());
        }

        private void OnEnable()
        {
            if (crowdAgentData == null) return;
        }

        private void OnDisable()
        {
            if (crowdAgentData == null) return;
            if (_spawnRoutineCoroutine == null) return;
            StopCoroutine(_spawnRoutineCoroutine);
            _spawnRoutineCoroutine = null;
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
            _speed        = Random.Range(crowdAgentData.minSpeed, crowdAgentData.maxSpeed);
            _angularSpeed = crowdAgentData.angularSpeed;
            _acceleration = crowdAgentData.acceleration;

            var prefabIndex      = Random.Range(0, crowdAgentPrefabs.Count);
            var crowdAgentPrefab = crowdAgentPrefabs[prefabIndex];
            var crowdAgentInstance =
                Instantiate(crowdAgentPrefabs[prefabIndex], GetSpawnPosition(), Quaternion.identity);
            crowdAgentInstance.transform.SetParent(transform);
            crowdAgentInstance.name = crowdAgentPrefab.name + currentAgentCount;
            crowdAgentInstance.AddComponent<AgentEntity>();

            var entity = crowdAgentInstance.GetComponent<AgentEntity>();
            entity.onAgentExited += OnCrowdAgentExited;
            entity.SetSpeed(_speed);
            entity.SetAgentID(crowdAgentData.permissionID);
            entity.SetAngularSpeed(_angularSpeed);
            entity.SetAcceleration(_acceleration);
            entity.SetTurningRadius(crowdAgentData.turningRadius);
            entity.SetStoppingDistance(crowdAgentData.stoppingDistance);
            entity.SetLicensePlateNumber(GenerateLicensePlate());

            crowdAgentInstance.AddComponent<AgentTracker>();

            var tracker = crowdAgentInstance.GetComponent<AgentTracker>();
            tracker.SetAgentEntity(entity);
            tracker.SetCrowdPath(crowdPath);
            tracker.InitializeWaypoint();
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
                var radius          = crowdPathPoint.allowableRadius;
                var randomDirection = Random.insideUnitSphere * radius;
                var position        = point.transform.position;
                randomDirection   += position;
                randomDirection.y =  position.y;
                return randomDirection;
            }

            return point.transform.position;
        }

        private static string GenerateLicensePlateNumber()
        {
            var licensePlateNumber = "";
            for (var i = 0; i < 3; i++)
            {
                licensePlateNumber += Random.Range(0, 10);
            }

            licensePlateNumber += " ";

            for (var i = 0; i < 3; i++)
            {
                licensePlateNumber += Random.Range(0, 10);
            }

            return licensePlateNumber;
        }

        private static string GenerateLicensePlate()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            var rnd = new System.Random();

            var plate = "";
            for (var i = 0; i < 3; i++)
            {
                plate += letters[rnd.Next(letters.Length)];
            }

            plate += "-";
            for (var i = 0; i < 4; i++)
            {
                plate += numbers[rnd.Next(numbers.Length)];
            }

            return plate;
        }
    }
}