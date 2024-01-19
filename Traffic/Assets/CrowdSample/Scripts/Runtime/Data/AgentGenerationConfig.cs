using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "AgentGenerationConfig", menuName = "CrowdWizard/Agent Generation Config")]
    public class AgentGenerationConfig : ScriptableObject
    {
        public GenerationMode generationMode = GenerationMode.InfinityFlow;

        [SerializeField] private bool  spawnAgentOnce = true;
        [SerializeField] private bool  closedPath     = true;
        [SerializeField] private bool  useSpacing     = true;
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

        public bool SpawnAgentOnce
        {
            get => spawnAgentOnce;
            set
            {
                if (generationMode == GenerationMode.Custom) spawnAgentOnce = value;
            }
        }

        public bool ClosedPath
        {
            get => closedPath;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.InfinityFlow ||
                    generationMode == GenerationMode.SingleCircle)
                    closedPath = value;
            }
        }

        public bool UseSpacing
        {
            get => useSpacing;
            set
            {
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.MultipleCircle)
                    useSpacing = value;
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
                    spawnAgentOnce = false;
                    closedPath     = false;
                    break;
                case GenerationMode.MultipleCircle:
                    spawnAgentOnce = true;
                    closedPath     = true;
                    break;
                case GenerationMode.SingleCircle:
                    spawnAgentOnce = true;
                    instantCount   = 1;
                    maxCount       = 1;
                    break;
                case GenerationMode.Custom:
                    break;
            }
        }
    }
}