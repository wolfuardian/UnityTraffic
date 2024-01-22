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

        public enum NavigateMode
        {
            Loop,
            PingPong,
            Once
        }

        public List<Vector3> Points
        {
            get => points ??= new List<Vector3>();
            set => points = value;
        }

        public int CurrentIndex
        {
            get => Mathf.Clamp(currentIndex, 0, points.Count - 1);
            set => currentIndex = Mathf.Clamp(value, 0, points.Count - 1);
        }

        public bool Reverse
        {
            get => reverse;
            set => reverse = value;
        }

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

            if (navMeshAgent.pathPending)
            {
                return;
            }

            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return;
            }

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

            SetDestination(points[currentIndex]);
        }

        private void Loop()
        {
            CurrentIndex = (CurrentIndex + 1) % points.Count;
        }

        private void PingPong()
        {
            if (reverse)
            {
                CurrentIndex -= 1;
            }
            else
            {
                CurrentIndex += 1;
            }

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
            CurrentIndex = Mathf.Clamp(CurrentIndex + 1, 0, points.Count - 1);
        }


        private void SetDestination(Vector3 destination)
        {
            navMeshAgent.destination = destination;
        }
    }
}