// using UnityEngine;
//
// namespace CrowdSample.Scripts.Runtime.Crowd
// {
//     public class AgentEntityController : MonoBehaviour
//     {
//         public CrowdPath crowdPathToGo;
//
//         public void GotoCrowdPath(AgentEntity agentEntity)
//         {
//             var agentTracker = agentEntity.GetComponent<AgentTracker>();
//             agentTracker.SetCrowdPath(crowdPathToGo);
//         }
//
//         public void TemporaryWaiting(AgentEntity agentEntity)
//         {
//             agentEntity.TemporaryDeceleration(1f, 2f);
//         }
//         
//
//     }
//     
// }