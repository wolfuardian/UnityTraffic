using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "AgentGenerationConfig", menuName = "CrowdWizard/Agent Generation Config")]
    public class AgentGenerationConfig : ScriptableObject
    {
        public GenerationMode generationMode = GenerationMode.InfinityFlow;

        [SerializeField] private bool  isSpawnAgentOnce = true;
        [SerializeField] private bool  isClosedPath     = true;
        [SerializeField] private bool  isUseSpacing     = true;
        [SerializeField] private int   instantCount   = 15;
        [SerializeField] private int   perSecondCount = 2;
        [SerializeField] private int   maxCount       = 100;
        [SerializeField] private float spacing        = 5f;
        [SerializeField] private float offset;

        public enum GenerationMode
        {
            InfinityFlow,
            MultipleCircle,
            SingleCircle,
            Custom
        }

        public bool IsSpawnAgentOnce
        {
            get => isSpawnAgentOnce;
            set
            {
                if (generationMode == GenerationMode.Custom) isSpawnAgentOnce = value;
            }
        }

        public bool IsClosedPath
        {
            get => isClosedPath;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.InfinityFlow ||
                    generationMode == GenerationMode.SingleCircle)
                    isClosedPath = value;
            }
        }

        public bool IsUseSpacing
        {
            get => isUseSpacing;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.MultipleCircle)
                    isUseSpacing = value;
            }
        }

        public int InstantCount
        {
            get => instantCount;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.MultipleCircle)
                    instantCount = value;
            }
        }

        public int PerSecondCount
        {
            get => perSecondCount;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.InfinityFlow)
                    perSecondCount = value;
            }
        }

        public int MaxCount
        {
            get => maxCount;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.InfinityFlow ||
                    generationMode == GenerationMode.MultipleCircle)
                    maxCount = value;
            }
        }

        public float Spacing
        {
            get => spacing;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.MultipleCircle)
                    spacing = value;
            }
        }

        public float Offset
        {
            get => offset;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.MultipleCircle ||
                    generationMode == GenerationMode.SingleCircle)
                    offset = value;
            }
        }

        public void ApplyPresetProperties()
        {
            switch (generationMode)
            {
                case GenerationMode.InfinityFlow:
                    isSpawnAgentOnce = false;
                    isClosedPath     = false;
                    instantCount   = 1;
                    offset         = 0;
                    break;
                case GenerationMode.MultipleCircle:
                    isSpawnAgentOnce = true;
                    isClosedPath     = true;
                    break;
                case GenerationMode.SingleCircle:
                    isSpawnAgentOnce = true;
                    instantCount   = 1;
                    maxCount       = 1;
                    break;
                case GenerationMode.Custom:
                    break;
            }
        }
    }
}