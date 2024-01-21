// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using CrowdSample.Scripts.Data;
//
// namespace CrowdSample.Scripts.Runtime.Crowd
// {
//     public class CrowdAgentFactory : MonoBehaviour
//     {
//         public           CrowdAgentData   crowdAgentData;
//         public           CrowdPath        crowdPath;
//         private readonly List<GameObject> _crowdAgentPrefabs = new List<GameObject>();
//         private          int              _currentAgentCount;
//         private          Coroutine        _spawnRoutineCoroutine;
//         private          AgentSpawner     _agentSpawner;
//
//         private void OnValidate()
//         {
//             if (crowdAgentData == null) return;
//
//             _crowdAgentPrefabs.Clear();
//             _crowdAgentPrefabs.AddRange(crowdAgentData.crowdAgentPrefabs);
//         }
//
//
//         private void Start()
//         {
//             if (crowdAgentData == null) return;
//             _agentSpawner          = new AgentSpawner(crowdAgentData, crowdPath);
//             _spawnRoutineCoroutine = StartCoroutine(SpawnRoutine());
//         }
//
//         private void OnDisable()
//         {
//             if (_spawnRoutineCoroutine != null)
//             {
//                 StopCoroutine(_spawnRoutineCoroutine);
//                 _spawnRoutineCoroutine = null;
//             }
//         }
//
//         private IEnumerator SpawnRoutine()
//         {
//             while (gameObject.activeSelf)
//             {
//                 if (_currentAgentCount < crowdAgentData.maxAgentCount)
//                 {
//                     SpawnCrowdAgent();
//                     yield return new WaitForSeconds(crowdAgentData.spawnInterval);
//                 }
//                 else
//                 {
//                     yield return null;
//                 }
//             }
//         }
//
//         private void SpawnCrowdAgent()
//         {
//             var prefabIndex        = Random.Range(0, _crowdAgentPrefabs.Count);
//             var crowdAgentInstance = _agentSpawner.SpawnAgent(_crowdAgentPrefabs[prefabIndex], transform);
//             crowdAgentInstance.name += _currentAgentCount;
//
//             var entity = crowdAgentInstance.GetComponent<AgentEntity>();
//             entity.onAgentExited += OnCrowdAgentExited;
//
//             _currentAgentCount++;
//         }
//
//         private void OnCrowdAgentExited(AgentEntity agent)
//         {
//             _currentAgentCount--;
//         }
//     }
// }