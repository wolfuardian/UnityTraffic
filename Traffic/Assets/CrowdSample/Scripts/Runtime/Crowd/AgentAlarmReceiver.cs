using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentAlarmReceiver : MonoBehaviour
    {
        public UnityEvent<bool, AgentEntity> onAlarmTriggered;
        public List<AgentEntity>             agentEntities = new List<AgentEntity>();

        public void OnAlarmRaise(AgentEntity agentEntity)
        {
            if (agentEntity == null || agentEntities.Contains(agentEntity)) return;

            agentEntities.Add(agentEntity);
            onAlarmTriggered?.Invoke(true, agentEntity);
        }

        public void OnAlarmStop(AgentEntity agentEntity)
        {
            if (agentEntity != null && agentEntities.Remove(agentEntity))
            {
                onAlarmTriggered?.Invoke(false, agentEntity);
            }
        }
    }
}