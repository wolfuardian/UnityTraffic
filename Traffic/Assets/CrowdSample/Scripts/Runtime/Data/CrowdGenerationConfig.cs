using System.Linq;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "CrowdGenerationConfig", menuName = "CrowdWizard/Crowd Generation Config")]
    public class CrowdGenerationConfig : ScriptableObject
    {
        #region Field Declarations

        public GenerationMode generationMode = GenerationMode.InfinityFlow;

        [SerializeField] private bool  isSpawnAgentOnce = true;
        [SerializeField] private bool  isReverseDirection;
        [SerializeField] private bool  isClosedPath  = true;
        [SerializeField] private bool  isUseSpacing  = true;
        [SerializeField] private int   instantCount  = 15;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int   maxCount      = 100;
        [SerializeField] private float spacing       = 5f;
        [SerializeField] private float offset;

        public enum GenerationMode
        {
            InfinityFlow,
            MultipleCircle,
            SingleCircle,
            Custom
        }

        #endregion

        #region Properties

        public bool IsSpawnAgentOnce
        {
            get => isSpawnAgentOnce;
            set => SetFieldValue(ref isSpawnAgentOnce, value, GenerationMode.Custom);
        }

        public bool IsReverseDirection
        {
            get => isReverseDirection;
            set => SetFieldValue(ref isReverseDirection, value, GenerationMode.Custom, GenerationMode.MultipleCircle,
                GenerationMode.SingleCircle);
        }

        public bool IsClosedPath
        {
            get => isClosedPath;
            set => SetFieldValue(ref isClosedPath, value, GenerationMode.Custom, GenerationMode.InfinityFlow,
                GenerationMode.SingleCircle);
        }


        public bool IsUseSpacing
        {
            get => isUseSpacing;
            set => SetFieldValue(ref isUseSpacing, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        public int InstantCount
        {
            get => instantCount;
            set => SetFieldValue(ref instantCount, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        public float SpawnInterval
        {
            get => spawnInterval;
            set => SetFieldValue(ref spawnInterval, value, GenerationMode.Custom, GenerationMode.InfinityFlow);
        }

        public int MaxCount
        {
            get => maxCount;
            set => SetFieldValue(ref maxCount, value, GenerationMode.Custom, GenerationMode.InfinityFlow,
                GenerationMode.MultipleCircle);
        }

        public float Spacing
        {
            get => spacing;
            set => SetFieldValue(ref spacing, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        public float Offset
        {
            get => offset;
            set => SetFieldValue(ref offset, value, GenerationMode.Custom, GenerationMode.MultipleCircle,
                GenerationMode.SingleCircle);
        }

        #endregion


        #region Public Methods

        public void ApplyPresetProperties()
        {
            switch (generationMode)
            {
                case GenerationMode.InfinityFlow:
                    isSpawnAgentOnce = false;
                    isClosedPath     = false;
                    instantCount     = 1;
                    offset           = 0;
                    break;
                case GenerationMode.MultipleCircle:
                    isSpawnAgentOnce = true;
                    isClosedPath     = true;
                    break;
                case GenerationMode.SingleCircle:
                    isSpawnAgentOnce = true;
                    instantCount     = 1;
                    maxCount         = 1;
                    break;
                case GenerationMode.Custom:
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void SetFieldValue<T>(ref T field, T value, params GenerationMode[] modes)
        {
            if (modes.Contains(generationMode)) field = value;
        }

        #endregion
    }
}