using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class AgentTracker : MonoBehaviour
    {
        #region Private Variables

        [SerializeField]                    private CrowdPath        crowdPath;
        [SerializeField]                    private AgentEntity _agentEntity;
        [SerializeField]                    private Transform   _previousTarget;
        [SerializeField]                    private Transform   _target;
        [SerializeField]                    private Vector3     _desiredPosition;
        [SerializeField]                    private int         _targetIndex;
        [SerializeField, Range(0.0f, 1.0f)] private float       _globalJourney;
        [SerializeField, Range(0.0f, 1.0f)] private float       _localJourney;
        [SerializeField]                    private float       _localDistance;
        [SerializeField]                    private float       _remainingDistance;
        [SerializeField]                    private bool        _isTrackable;
        [SerializeField]                    private float       _turningRadius;

        /// <summary> 獲取路徑實例。 </summary>
        public CrowdPath CrowdPathInstance => crowdPath ??= transform.parent.GetComponent<CrowdPath>();

        /// <summary> 獲取或設置代理實體。 </summary>
        public AgentEntity AgentEntityInstance
        {
            get => _agentEntity;
            set => _agentEntity = value;
        }

        /// <summary> 獲取或設置之前的目標轉換。 </summary>
        public Transform PreviousTarget
        {
            get => _previousTarget;
            set => _previousTarget = value;
        }

        /// <summary> 獲取或設置當前目標轉換。 </summary>
        public Transform CurrentTarget
        {
            get => _target;
            set => _target = value;
        }

        /// <summary> 獲取或設置代理的期望位置。 </summary>
        public Vector3 DesiredPosition
        {
            get => _desiredPosition;
            set => _desiredPosition = value;
        }

        /// <summary> 獲取或設置目標索引。 </summary>
        public int TargetIndex
        {
            get => _targetIndex;
            set => _targetIndex = value % crowdPath.Waypoints.Count;
        }

        /// <summary> 獲取或設置全局旅程的進度。 </summary>
        public float GlobalJourney
        {
            get => _globalJourney;
            set => _globalJourney = value;
        }

        /// <summary> 獲取或設置局部旅程的進度。 </summary>
        public float LocalJourney
        {
            get => _localJourney;
            set => _localJourney = Mathf.Clamp01(value);
        }

        /// <summary> 獲取或設置局部距離。 </summary>
        public float LocalDistance
        {
            get => _localDistance;
            set => _localDistance = Mathf.Max(value, 0f);
        }

        /// <summary> 獲取或設置剩餘距離。 </summary>
        public float RemainingDistance
        {
            get => _remainingDistance;
            set => _remainingDistance = Mathf.Max(value, 0f);
        }

        /// <summary>
        /// 指示代理是否可被追蹤。
        /// </summary>
        public bool IsTrackable
        {
            get => _isTrackable;
            set => _isTrackable = value;
        }

        /// <summary>
        /// 獲取或設置轉向半徑。
        /// </summary>
        public float TurningRadius
        {
            get => _turningRadius;
            set => _turningRadius = Mathf.Max(value, 0f);
        }

        #endregion

        private void Start()
        {
            _previousTarget = _target;
            Initialize();
        }

        private void Update()
        {
            if (!_isTrackable) return;

            Debug.Log($"GlobalJourney: {_globalJourney}");

            _targetIndex = GlobalJourneyToTargetIndex(_globalJourney);
            Debug.Log($"TargetIndex: {_targetIndex}");

            _target = crowdPath.Waypoints[_targetIndex];
            Debug.Log($"Target: {_target.name}");

            _desiredPosition = GetRandomPointInRadius(_target);
            Debug.Log($"DesiredPosition: {_desiredPosition}");

            SetAgentDestination();
            Debug.Log($"AgentDestination: {_agentEntity.NavMeshAgent.destination}");

            if (!_agentEntity.NavMeshAgent.pathPending && _agentEntity.NavMeshAgent.remainingDistance < 0.5f)
            {
                _agentEntity.NavMeshAgent.destination = crowdPath.Waypoints[_targetIndex].position;

                _targetIndex = (_targetIndex + 1) % crowdPath.Waypoints.Count;
            }

            Debug.Log($"RemainingDistance: {_agentEntity.NavMeshAgent.remainingDistance}");
        }

        private void LateUpdate()
        {
            _remainingDistance = _agentEntity.NavMeshAgent.remainingDistance;
            _localDistance     = Vector3.Distance(_previousTarget.transform.position, _desiredPosition);
            _localJourney      = Mathf.Clamp(1 - _remainingDistance / _localDistance, 0f, 1f);
            _globalJourney     = (_targetIndex + _localJourney) / crowdPath.Waypoints.Count;
            Debug.Log($"GlobalJourneyLate: {_globalJourney}");
        }

        public void Construct(CrowdPath crowdPath)
        {
            this.crowdPath = crowdPath;
        }

        public void Initialize()
        {
            // _targetIndex    = GlobalJourneyToTargetIndex(_globalJourney);
            _target         = crowdPath.Waypoints[_targetIndex];
            _previousTarget = _target;
            _localDistance  = 0f;
            _isTrackable    = true;
        }


        public void SetAgentDestination()
        {
            if (_agentEntity == null)
            {
                Debug.LogWarning("AgentEntity is null.");
                return;
            }

            _agentEntity.SetDestination(_desiredPosition);
        }


        private int GlobalJourneyToTargetIndex(float journey)
        {
            if (crowdPath == null)
            {
                Debug.LogWarning("Path is null.");
            }

            if (crowdPath.Waypoints == null)
            {
                Debug.LogWarning("Path.Waypoints is null.");
            }

            if (crowdPath.Waypoints.Count == 0)
            {
                Debug.LogWarning("Path.Waypoints.Count is 0.");
            }

            var index = (int)(journey * crowdPath.Waypoints.Count);
            index %= crowdPath.Waypoints.Count;
            return index;
        }

        private static Vector3 GetRandomPointInRadius(Component point)
        {
            var crowdPathPoint = point.GetComponent<Waypoint>();
            if (crowdPathPoint == null)
            {
                Debug.LogWarning($"CrowdPathPoint component is missing on the GameObject: {point.name}");
                return point.transform.position;
            }

            var radius          = crowdPathPoint.Radius;
            var randomDirection = Random.insideUnitSphere * radius;
            var position        = point.transform.position;
            randomDirection   += position;
            randomDirection.y =  position.y;
            return randomDirection;
        }

        private void OnDrawGizmos()
        {
            if (_target == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _desiredPosition);
        }
    }
}