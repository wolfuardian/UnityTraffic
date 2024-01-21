using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "AgentDataConfig", menuName = "CrowdWizard/Agent Data Config")]
    public class AgentDataConfig : ScriptableObject
    {
        public GameObject[] agentPrefabs;

        [Header("代理許可權")] public PermissionID permissionID = PermissionID.None;

        [Header("代理行為設置")] public float minSpeed = 4f;

        public float maxSpeed = 6f;

        public float angularSpeed = 100f;

        public float acceleration = 5f;

        public float turningRadius = 2f;

        public float stoppingDistance = 1f;

        public enum PermissionID
        {
            None,
            Approved,
            Rejected
        }
    }
}