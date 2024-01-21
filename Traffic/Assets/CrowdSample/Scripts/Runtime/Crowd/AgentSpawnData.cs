using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [System.Serializable]
    public struct AgentSpawnData
    {
        public Vector3 position;
        public Vector3 direction;
        public float   curvePos;

        public AgentSpawnData(Vector3 position, Vector3 direction, float curvePos)
        {
            this.position  = position;
            this.direction = direction;
            this.curvePos  = curvePos;
        }
    }
}