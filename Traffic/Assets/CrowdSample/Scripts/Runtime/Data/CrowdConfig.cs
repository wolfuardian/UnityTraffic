using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Data
{
    [CreateAssetMenu(fileName = "CrowdGenerationConfig", menuName = "CrowdWizard/Crowd Generation Config")]
    public class CrowdConfig : ScriptableObject
    {
        #region Field Declarations

        // Generation
        [SerializeField] private GenerationMode m_generationMode = GenerationMode.InfinityFlow;
        [SerializeField] private float          m_spawnInterval  = 2f;
        [SerializeField] private int            m_instantCount   = 15;
        [SerializeField] private int            m_maxSpawnCount  = 100;
        [SerializeField] private float          m_spacing        = 5f;
        [SerializeField] private float          m_offset         = 0f;
        [SerializeField] private bool           m_spawnOnce      = true;
        [SerializeField] private bool           m_reverse        = false;
        [SerializeField] private bool           m_pathClosed     = true;
        [SerializeField] private bool           m_useSpacing     = true;

        // Resources
        [SerializeField] private List<GameObject> m_agentPrefabs;

        // NavMeshAgentData
        [SerializeField] private float m_minSpeed         = 4f;
        [SerializeField] private float m_maxSpeed         = 5f;
        [SerializeField] private float m_angularSpeed     = 100f;
        [SerializeField] private float m_acceleration     = 5f;
        [SerializeField] private float m_stoppingDistance = 1f;
        [SerializeField] private bool  m_autoBraking      = true;

        // AgentUserData
        [SerializeField] private string m_agentID = "No Data";
        [SerializeField] private string m_type    = "No Data";
        [SerializeField] private string m_category;
        [SerializeField] private string m_alias;
        [SerializeField] private string m_model;
        [SerializeField] private string m_time;
        [SerializeField] private string m_noted;

        public enum GenerationMode
        {
            InfinityFlow,
            MultipleCircle,
            SingleCircle,
            Custom
        }

        #endregion

        #region Properties

        // Generation

        public GenerationMode generationMode
        {
            get => m_generationMode;
            set => m_generationMode = value;
        }

        public int instantCount
        {
            get => m_instantCount;
            set => SetFieldValue(ref m_instantCount, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        public float spawnInterval
        {
            get => m_spawnInterval;
            set => SetFieldValue(ref m_spawnInterval, value, GenerationMode.Custom, GenerationMode.InfinityFlow);
        }

        public int maxSpawnCount
        {
            get => m_maxSpawnCount;
            set => SetFieldValue(ref m_maxSpawnCount, value, GenerationMode.Custom, GenerationMode.InfinityFlow,
                GenerationMode.MultipleCircle);
        }

        public float spacing
        {
            get => m_spacing;
            set => SetFieldValue(ref m_spacing, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }

        public float offset
        {
            get => m_offset;
            set => SetFieldValue(ref m_offset, value, GenerationMode.Custom, GenerationMode.MultipleCircle,
                GenerationMode.SingleCircle);
        }

        public bool spawnOnce
        {
            get => m_spawnOnce;
            set => SetFieldValue(ref m_spawnOnce, value, GenerationMode.Custom);
        }

        public bool reverse
        {
            get => m_reverse;
            set => SetFieldValue(ref m_reverse, value, GenerationMode.Custom, GenerationMode.MultipleCircle,
                GenerationMode.SingleCircle);
        }

        public bool pathClosed
        {
            get => m_pathClosed;
            set => SetFieldValue(ref m_pathClosed, value, GenerationMode.Custom, GenerationMode.InfinityFlow,
                GenerationMode.SingleCircle);
        }

        public bool useSpacing
        {
            get => m_useSpacing;
            set => SetFieldValue(ref m_useSpacing, value, GenerationMode.Custom, GenerationMode.MultipleCircle);
        }
        // Resources

        public List<GameObject> agentPrefabs
        {
            get => m_agentPrefabs;
            set => m_agentPrefabs = value;
        }
        // NavMeshAgentData

        public float minSpeed
        {
            get => m_minSpeed;
            set => m_minSpeed = value;
        }

        public float maxSpeed
        {
            get => m_maxSpeed;
            set => m_maxSpeed = value;
        }

        public float angularSpeed
        {
            get => m_angularSpeed;
            set => m_angularSpeed = value;
        }

        public float acceleration
        {
            get => m_acceleration;
            set => m_acceleration = value;
        }

        public float stoppingDistance
        {
            get => m_stoppingDistance;
            set => m_stoppingDistance = value;
        }

        public bool autoBraking
        {
            get => m_autoBraking;
            set => m_autoBraking = value;
        }
        // AgentUserData

        public string agentID
        {
            get => m_agentID;
            set => m_agentID = value;
        }

        public string type
        {
            get => m_type;
            set => m_type = value;
        }

        public string category
        {
            get => m_category;
            set => m_category = value;
        }

        public string alias
        {
            get => m_alias;
            set => m_alias = value;
        }

        public string model
        {
            get => m_model;
            set => m_model = value;
        }

        public string time
        {
            get => m_time;
            set => m_time = value;
        }

        public string noted
        {
            get => m_noted;
            set => m_noted = value;
        }

        #endregion

        #region Public Methods

        public void ApplyPresetProperties()
        {
            switch (m_generationMode)
            {
                case GenerationMode.InfinityFlow:
                    m_spawnOnce    = false;
                    m_pathClosed   = false;
                    m_instantCount = 1;
                    m_offset       = 0;
                    break;
                case GenerationMode.MultipleCircle:
                    m_spawnOnce  = true;
                    m_pathClosed = true;
                    break;
                case GenerationMode.SingleCircle:
                    m_spawnOnce     = true;
                    m_instantCount  = 1;
                    m_maxSpawnCount = 1;
                    break;
                case GenerationMode.Custom:
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void SetFieldValue<T>(ref T field, T value, params GenerationMode[] modes)
        {
            if (modes.Contains(m_generationMode)) field = value;
        }

        #endregion
    }
}