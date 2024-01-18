using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "AgentGenerationConfig", menuName = "CrowdWizard/Agent Generation Config")]
    public class AgentGenerationConfig : ScriptableObject
    {
        public GenerationMode generationMode = GenerationMode.InfinityFlow;

        [SerializeField] private bool spawnAgentOnce = true;
        [SerializeField] private bool closedPath     = true;
        [SerializeField] private int  instantCount   = 15;
        [SerializeField] private int  perSecondCount = 2;
        [SerializeField] private int  maxCount       = 60;

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
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.SingleCircle)
                    closedPath = value;
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
                if (generationMode == GenerationMode.Custom || generationMode == GenerationMode.InfinityFlow)
                    maxCount = value;
            }
        }

        private void OnValidate()
        {
            switch (generationMode)
            {
                case GenerationMode.InfinityFlow:
                    spawnAgentOnce = false;
                    closedPath     = true;
                    instantCount   = 0;
                    break;
                case GenerationMode.MultipleCircle:
                    spawnAgentOnce = true;
                    closedPath     = true;
                    perSecondCount = 0;
                    maxCount       = 0;
                    break;
                case GenerationMode.SingleCircle:
                    spawnAgentOnce = true;
                    closedPath     = true; // 可修改
                    instantCount   = 1;
                    perSecondCount = 0;
                    maxCount       = 0;
                    break;
                case GenerationMode.Custom:
                    // 所有參數可自定義
                    break;
            }
        }
    }
}