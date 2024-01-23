using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    [DisallowMultipleComponent]
    public class PathFollow : MonoBehaviour
    {
        #region Field Declarations

        public enum NavigateMode
        {
            Loop,
            PingPong,
            Once,
            Custom
        }

        [SerializeField] private List<Vector3> points;
        [SerializeField] private List<float>   ranges;
        [SerializeField] private NavigateMode  navigateMode = NavigateMode.Loop;
        [SerializeField] private int           currentIndex;
        [SerializeField] private int           targetIndex;
        [SerializeField] private int           forceTargetIndex;
        [SerializeField] private bool          stopOnNextPoint;
        [SerializeField] private bool          reverse;

        private NavMeshAgent navMeshAgent;
        private float        radius;

        #endregion

        #region Properties

        public List<Vector3> Points
        {
            get => points ??= new List<Vector3>();
            set => points = value;
        }

        public List<float> Ranges
        {
            get => ranges ??= new List<float>();
            set => ranges = value;
        }

        public int CurrentIndex
        {
            get => currentIndex;
            set => currentIndex = value;
        }

        public int TargetIndex
        {
            get => targetIndex;
            set => targetIndex = value;
        }

        public int ForceTargetIndex
        {
            get => forceTargetIndex;
            set => forceTargetIndex = value;
        }

        public bool StopOnNextPoint
        {
            get => stopOnNextPoint;
            set => stopOnNextPoint = value;
        }

        public bool Reverse
        {
            get => reverse;
            set => reverse = value;
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                Debug.LogError($"[錯誤] 物件 '{name}' 上找不到 NavMeshAgent 組件。請確認物件上已正確添加 NavMeshAgent 組件。", this);
            }
        }

        private void Update()
        {
            if (stopOnNextPoint)
            {
                return;
            }

            if (points.Count == 0 || navMeshAgent.pathPending ||
                navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return;
            }

            if (navigateMode != NavigateMode.Custom)
            {
                UpdateIndexBasedOnDirection();
            }

            UpdateTargetIndex();

            targetIndex = Mathf.Clamp(targetIndex, 0, points.Count - 1);

            UpdateCurrentIndex();

            UpdateRadius();

            var destination = ScatterDestination(points[targetIndex], radius);
            SetDestination(destination);
        }

        private void UpdateRadius()
        {
            if (ranges.Count == 0)
            {
                return;
            }

            radius = ranges[targetIndex];
        }

        private void UpdateCurrentIndex()
        {
            currentIndex = targetIndex - 1;
            if (currentIndex < 0)
            {
                currentIndex = points.Count - 1;
            }
        }

        #endregion

        #region Public Methods

        public void SetNavigateMode(NavigateMode mode)
        {
            navigateMode = mode;
        }

        #endregion

        #region Private Methods

        private void UpdateIndexBasedOnDirection()
        {
            targetIndex += reverse ? -1 : 1;
        }

        private void UpdateTargetIndex()
        {
            switch (navigateMode)
            {
                case NavigateMode.Loop:
                    Loop();
                    break;
                case NavigateMode.PingPong:
                    PingPong();
                    break;
                case NavigateMode.Once:
                    Once();
                    break;
                case NavigateMode.Custom:
                    Custom();
                    break;
            }
        }

        private static Vector3 ScatterDestination(Vector3 dest, float r)
        {
            return dest + new Vector3(Random.insideUnitCircle.x * r, 0f, Random.insideUnitCircle.y * r);
        }

        private void Loop()
        {
            if (targetIndex >= points.Count)
            {
                targetIndex = 0;
            }
            else if (targetIndex < 0)
            {
                targetIndex = points.Count - 1;
            }
        }

        private void PingPong()
        {
            if (targetIndex >= points.Count - 1)
            {
                reverse = true;
            }
            else if (targetIndex <= 0)
            {
                reverse = false;
            }
        }

        private void Once()
        {
            targetIndex = Mathf.Clamp(targetIndex, 0, points.Count - 1);
        }

        private void Custom()
        {
            if (targetIndex == forceTargetIndex)
            {
                return;
            }

            var direction = targetIndex < forceTargetIndex ? 1 : -1;
            targetIndex += direction;

            if ((direction == 1 && targetIndex > forceTargetIndex) ||
                (direction == -1 && targetIndex < forceTargetIndex))
            {
                targetIndex = forceTargetIndex;
            }
        }

        private void SetDestination(Vector3 destination)
        {
            navMeshAgent.destination = destination;
        }

        #endregion

        #region Debug and Visualization Methods

        private void OnDrawGizmos()
        {
            if (navMeshAgent == null)
            {
                return;
            }

            Debug.DrawLine(transform.position, navMeshAgent.destination, Color.red);
        }

        #endregion
    }
}