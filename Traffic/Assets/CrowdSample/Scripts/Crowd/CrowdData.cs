using UnityEngine;

namespace CrowdSample.Scripts.Crowd
{
    public enum LicensePlateStates
    {
        Vip   = 0,
        Guest = 1,
        Deny  = 2
    }

    public struct CarData
    {
        public enum TransitStates
        {
            Moving,
            Arrived,
            Congested,
            Halted,
            Stopped
        }

        public enum RouteStates
        {
            Validating,
            Clear,
            Obstructed,
            Rerouting
        }
    }


    [System.Serializable]
    public struct SpawnPoint
    {
        [SerializeField] private Vector3 m_position;
        [SerializeField] private Vector3 m_direction;
        [SerializeField] private float   m_pathInterp;
        [SerializeField] private float   m_pathLocation;

        public SpawnPoint(Vector3 position, Vector3 direction, float pathInterp, float pathLocation)
        {
            m_position     = position;
            m_direction    = direction;
            m_pathInterp   = pathInterp;
            m_pathLocation = pathLocation;
        }

        public Vector3 position
        {
            get => m_position;
            set => m_position = value;
        }

        public Vector3 direction
        {
            get => m_direction;
            set => m_direction = value;
        }

        public float pathInterp
        {
            get => m_pathInterp;
            set => m_pathInterp = value;
        }

        public float pathLocation
        {
            get => m_pathLocation;
            set => m_pathLocation = value;
        }

        public int targetPointNum => Mathf.Clamp(Mathf.FloorToInt(m_pathLocation) + 1, 0, int.MaxValue);
    }
}