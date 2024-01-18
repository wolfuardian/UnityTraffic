using System.Collections.Generic;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "AgentData", menuName = "CrowdWizard/Agent Data")]
    public class AgentData : ScriptableObject
    {
        [Tooltip("代理預制件列表。")] public List<GameObject> agentPrefabs = new List<GameObject>();

        [Header("代理許可權")] [Tooltip("代理的許可權ID。")]
        public PermissionID permissionID = PermissionID.None;

        [Header("代理行為設置")] [Tooltip("代理的最小速度。")]
        public float minSpeed = 4f;

        [Tooltip("代理的最大速度。")] public float maxSpeed = 6f;

        [Tooltip("代理的角速度。")] public float angularSpeed = 100f;

        [Tooltip("代理的加速度。")] public float acceleration = 5f;

        [Tooltip("代理的轉彎半徑。")] public float turningRadius = 2f;

        [Tooltip("代理的停止距離。")] public float stoppingDistance = 1f;

        public enum PermissionID
        {
            None,
            Approved,
            Rejected
        }
    }
}