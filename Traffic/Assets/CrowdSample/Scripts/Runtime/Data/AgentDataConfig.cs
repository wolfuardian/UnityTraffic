using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "AgentDataConfig", menuName = "CrowdWizard/Agent Data Config")]
    public class AgentDataConfig : ScriptableObject
    {
        public PermissionID permissionID = PermissionID.None;

        [SerializeField] private GameObject[] agentPrefabs;
        [SerializeField] private float        minSpeed         = 4f;
        [SerializeField] private float        maxSpeed         = 6f;
        [SerializeField] private float        angularSpeed     = 100f;
        [SerializeField] private float        acceleration     = 5f;
        [SerializeField] private float        turningRadius    = 2f;
        [SerializeField] private float        stoppingDistance = 1f;

        public enum PermissionID
        {
            None,
            Approved,
            Rejected
        }

        public GameObject[] AgentPrefabs     => agentPrefabs;
        public float        MinSpeed         => minSpeed;
        public float        MaxSpeed         => maxSpeed;
        public float        AngularSpeed     => angularSpeed;
        public float        Acceleration     => acceleration;
        public float        TurningRadius    => turningRadius;
        public float        StoppingDistance => stoppingDistance;
    }
}