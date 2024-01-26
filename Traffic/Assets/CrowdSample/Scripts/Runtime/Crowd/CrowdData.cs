using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [System.Serializable]
    public struct NavMeshAgentData
    {
        public float m_stoppingDistance;
        public float m_speed;
        public float m_angularSpeed;
        public float m_acceleration;
        public bool  m_autoBraking;

        public NavMeshAgentData(
            float stoppingDistance,
            float speed,
            float angularSpeed,
            float acceleration,
            bool  autoBraking
        )
        {
            m_stoppingDistance = stoppingDistance;
            m_speed            = speed;
            m_angularSpeed     = angularSpeed;
            m_acceleration     = acceleration;
            m_autoBraking      = autoBraking;
        }
    }

    [System.Serializable]
    public struct AgentData
    {
        public string m_agentID;
        public string m_type;
        public string m_category;
        public string m_alias;
        public string m_model;
        public string m_time;
        public string m_noted;

        public AgentData(
            string agentID,
            string type,
            string category,
            string alias,
            string model,
            string time,
            string noted
        )
        {
            m_agentID  = agentID;
            m_type     = type;
            m_category = category;
            m_alias    = alias;
            m_model    = model;
            m_time     = time;
            m_noted    = noted;
        }
    }

    [System.Serializable]
    public struct PathFollowData
    {
        public SpawnPointData[]             m_spawnPointsData;
        public bool                         m_shouldDestroyOnGoal;
        public bool                         m_reverse;
        public bool                         m_closedLoop;
        public int                          m_targetIndex;
        public CrowdPathFollow.NavigateMode m_navigateMode;

        public PathFollowData(
            SpawnPointData[]             spawnPointsData,
            bool                         shouldDestroyOnGoal,
            bool                         reverse,
            bool                         closedLoop,
            int                          targetIndex,
            CrowdPathFollow.NavigateMode navigateMode
        )
        {
            m_spawnPointsData     = spawnPointsData;
            m_navigateMode        = navigateMode;
            m_shouldDestroyOnGoal = shouldDestroyOnGoal;
            m_reverse             = reverse;
            m_closedLoop          = closedLoop;
            m_targetIndex         = targetIndex;
        }
    }

    [System.Serializable]
    public struct CrowdFactoryConfig
    {
        public NavMeshAgentData m_navMeshAgentData;
        public AgentData        m_agentData;
        public PathFollowData   m_pathFollowData;

        public CrowdFactoryConfig(
            NavMeshAgentData navMeshAgentData,
            AgentData        agentData,
            PathFollowData   pathFollowData
        )
        {
            m_navMeshAgentData = navMeshAgentData;
            m_agentData        = agentData;
            m_pathFollowData   = pathFollowData;
        }
    }

    [System.Serializable]
    public struct SpawnPointData
    {
        public Vector3 m_position;
        public Vector3 m_direction;
        public float   m_radius;
        public float   m_pathLocation;

        public SpawnPointData(Vector3 position, Vector3 direction, float radius, float pathLocation)
        {
            m_position     = position;
            m_direction    = direction;
            m_radius       = radius;
            m_pathLocation = pathLocation;
        }
    }
}