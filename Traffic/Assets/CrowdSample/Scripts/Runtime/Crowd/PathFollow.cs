using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class PathFollow : MonoBehaviour
    {
        [SerializeField] public  List<Vector3> points;
        [SerializeField] private NavigateMode  navigateMode = NavigateMode.Loop;
        [SerializeField] private int           currentIndex;
        [SerializeField] private bool          reverse;
        [SerializeField] private float         radius;

        public enum NavigateMode
        {
            Loop,
            PingPong,
            Once
        }

        # region Properties

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

        private NavMeshAgent navMeshAgent;


        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            }
        }

        private void Update()
        {
            if (points.Count == 0)
            {
                return;
            }

            if (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return;
            }

            UpdateIndexBasedOnDirection();

            UpdateCurrentIndex();

            currentIndex = Mathf.Clamp(currentIndex, 0, points.Count - 1);

            var destination = ScatterDestination(points[currentIndex], radius);
            SetDestination(destination);
        }

        private void UpdateIndexBasedOnDirection()
        {
            if (reverse)
            {
                CurrentIndex -= 1;
            }
            else
            {
                CurrentIndex += 1;
            }
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
            }
        }

        private static Vector3 ScatterDestination(Vector3 destination, float r)
        {
            return destination + new Vector3(Random.insideUnitCircle.x * r, 0f, Random.insideUnitCircle.y * r);
        }

        private void Loop()
        {
            if (CurrentIndex >= points.Count)
            {
                CurrentIndex = 0;
            }
            else if (CurrentIndex < 0)
            {
                CurrentIndex = points.Count - 1;
            }
        }

        private void PingPong()
        {
            if (CurrentIndex >= points.Count - 1)
            {
                reverse = true;
            }
            else if (CurrentIndex <= 0)
            {
                reverse = false;
            }
        }

        private void Once()
        {
            CurrentIndex = Mathf.Clamp(CurrentIndex, 0, points.Count - 1);
        }


        private void SetDestination(Vector3 destination)
        {
            navMeshAgent.destination = destination;
        }

        private void OnDrawGizmos()
        {
            if (navMeshAgent == null)
            {
                return;
            }

            Debug.DrawLine(transform.position, navMeshAgent.destination, Color.red);
        }
    }
}