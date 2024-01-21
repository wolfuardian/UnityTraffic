// using System;
// using UnityEngine;
// using UnityEngine.Events;
//
// namespace CrowdSample.Scripts.Runtime.Crowd
// {
//     public class AgentIDReader : MonoBehaviour
//     {
//         public UnityEvent<AgentEntity> onAgentEnterAccess;
//         public UnityEvent<AgentEntity> onAgentEnterDenied;
//         public UnityEvent<AgentEntity> onAgentExitDetect;
//
//         public enum PermissionID
//         {
//             Authorized,
//             Denied
//         }
//
//         private void OnTriggerEnter(Collider other)
//         {
//             var agentEntity = other.GetComponent<AgentEntity>();
//             if (agentEntity == null) return;
//
//             if (!Enum.TryParse(agentEntity.AgentID, out PermissionID permissionId)) return;
//
//             switch (permissionId)
//             {
//                 case PermissionID.Authorized:
//                     onAgentEnterAccess?.Invoke(agentEntity);
//                     break;
//                 case PermissionID.Denied:
//                     onAgentEnterDenied?.Invoke(agentEntity);
//                     break;
//             }
//         }
//
//         private void OnTriggerExit(Collider other)
//         {
//             var agentEntity = other.GetComponent<AgentEntity>();
//             onAgentExitDetect?.Invoke(agentEntity);
//         }
//     }
// }