using System;
using UnityEngine;

namespace TimetreeSample.Scripts.Timestage
{
    public class Timestage : MonoBehaviour
    {
        [SerializeField] private float        _time;
        [SerializeField] private StageGroup[] _stageGroups;

        private void Awake()
        {
            for (var i = 0; i < _stageGroups.Length; i++)
            {
                _stageGroups[i].Visualizer         = _stageGroups[i]._target.AddComponent<TimestageVisualizer>();
                _stageGroups[i].Visualizer._config = _stageGroups[i]._config;
            }
        }

        private void Start()
        {
            Evaluate();
        }

        public void SelectTime(float time)
        {
            _time = time;
            Evaluate();
        }

        public void Evaluate()
        {
            foreach (var stageGroup in _stageGroups)
            {
                var visualizer       = stageGroup.Visualizer;
                var displayStartTime = stageGroup._displayStartTime;
                var displayEndTime   = stageGroup._displayEndTime;
                if (_time >= displayStartTime && _time <= displayEndTime)
                {
                    visualizer.OnShow();
                }
                else
                {
                    visualizer.OnHide();
                }
            }
        }
    }

    [Serializable]
    internal struct StageGroup
    {
        internal TimestageVisualizer Visualizer;
        public   GameObject          _target;
        public   float               _displayStartTime;
        public   float               _displayEndTime;
        public   StageConfig         _config;
    }

    [Serializable]
    public struct StageConfig
    {
        [Header("Sequence")] public bool _useSequential;

        public float _sequentialDelta;

        [Header("Animation Settings")] public bool _animEachChildren;

        [Tooltip("啟用此選項時，物體會在被觸發時立即顯示/隱藏，將無視其他動畫設定。")]
        public bool _useImmediatelyVisible;

        public bool           _useDissolve;
        public DissolveConfig _dissolveConfig;
        public bool           _useMoGraph;
    }

    [Serializable]
    public struct DissolveConfig
    {
        public Material _dissolveMaterial;
        public float    _dissolveDuration;
        public float    _dissolveDelay;
    }
}