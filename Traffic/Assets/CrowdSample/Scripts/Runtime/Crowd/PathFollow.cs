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
        [SerializeField] private NavigateMode  navigateMode = NavigateMode.Loop;
        [SerializeField] private int           currentIndex;
        [SerializeField] private int           targetIndex;
        [SerializeField] private bool          reverse;
        [SerializeField] private float         radius;

        private NavMeshAgent navMeshAgent;

        #endregion

        #region Properties

        public List<Vector3> Points
        {
            get => points ??= new List<Vector3>();
            set => points = value;
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

        public bool Reverse
        {
            get => reverse;
            set => reverse = value;
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
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
            if (points.Count == 0 || navMeshAgent.pathPending ||
                navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return;
            }

            if (navigateMode != NavigateMode.Custom)
            {
                UpdateIndexBasedOnDirection();
            }

            UpdateCurrentIndex();
            currentIndex = Mathf.Clamp(currentIndex, 0, points.Count - 1);

            var destination = ScatterDestination(points[currentIndex], radius);
            SetDestination(destination);
        }

        #endregion

        #region Public Methods

        // 如果有公共方法，放在這裡

        #endregion

        #region Private Methods

        private void UpdateIndexBasedOnDirection()
        {
            currentIndex += reverse ? -1 : 1;
        }

        private void UpdateCurrentIndex()
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
            if (currentIndex >= points.Count)
            {
                currentIndex = 0;
            }
            else if (currentIndex < 0)
            {
                currentIndex = points.Count - 1;
            }
        }

        private void PingPong()
        {
            if (currentIndex >= points.Count - 1)
            {
                reverse = true;
            }
            else if (currentIndex <= 0)
            {
                reverse = false;
            }
        }

        private void Once()
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, points.Count - 1);
        }

        private void Custom()
        {
            if (currentIndex == targetIndex)
            {
                return;
            }

            var direction = currentIndex < targetIndex ? 1 : -1; // 根據目標索引的位置決定方向
            currentIndex += direction; // 根據方向更新索引

            if ((direction == 1 && currentIndex > targetIndex) ||
                (direction == -1 && currentIndex < targetIndex))
            {
                currentIndex = targetIndex; // 如果超過目標索引，則設置為目標索引
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