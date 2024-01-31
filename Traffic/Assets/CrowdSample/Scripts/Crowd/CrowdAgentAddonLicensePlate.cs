using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Random = System.Random;

namespace CrowdSample.Scripts.Crowd
{
    public class LicensePlate
    {
        public string plateNumber { get; set; }
        public string authState   { get; set; }
    }

    public class CrowdAgentAddonLicensePlate : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private CrowdSpawner       m_crowdSpawner;
        [SerializeField] private string             m_licensePlateNumber;
        [SerializeField] private LicensePlateStates m_licensePlateAuthStates;

        private          Dictionary<string, LicensePlate> plateDataMap;
        private readonly Random                           random = new Random();

        #endregion

        #region Properties

        public CrowdSpawner crowdSpawner
        {
            get => m_crowdSpawner;
            set => m_crowdSpawner = value;
        }

        public string licensePlateNumber
        {
            get => m_licensePlateNumber;
            set => m_licensePlateNumber = value;
        }

        public LicensePlateStates licensePlateAuthStates
        {
            get => m_licensePlateAuthStates;
            set => m_licensePlateAuthStates = value;
        }

        #endregion

        #region Public Methods

        public void RegistryDataFromCsv(string filePath)
        {
            plateDataMap = LoadDataFromCsv(filePath);
        }

        public static Dictionary<string, LicensePlate> LoadDataFromCsv(string filePath)
        {
            var dataMap = new Dictionary<string, LicensePlate>();

            using var reader = new StreamReader(filePath);
            if (!reader.EndOfStream)
            {
                reader.ReadLine(); // 讀取第一行 (Header) 並忽略
            }

            while (!reader.EndOfStream)
            {
                var line   = reader.ReadLine();
                var values = line!.Split(',');
                var plate = new LicensePlate
                {
                    plateNumber = values[0].Trim('\''),
                    authState   = values[1].Trim('\''),
                };
                dataMap[plate.plateNumber] = plate;
            }

            return dataMap;
        }

        public string GetRandomPlateNumber()
        {
            if (plateDataMap.Count == 0)
            {
                return null;
            }

            var index             = random.Next(0, plateDataMap.Count);
            var randomPlateNumber = plateDataMap.Keys.ElementAt(index);
            return randomPlateNumber;
        }

        public string GetAuthState(string plateNumber)
        {
            return plateDataMap[plateNumber].authState;
        }

        #endregion

        #region Debug and Visualization Methods

        private void OnDrawGizmos()
        {
            var position = transform.position + Vector3.up * 3f;
            var style = new GUIStyle
            {
                normal    = { textColor = Color.yellow },
                alignment = TextAnchor.UpperLeft,
                fontSize  = 13
            };
            var text = "" +
                       "    PID: " + licensePlateNumber + "\n" +
                       "    AUTH: " + licensePlateAuthStates;

            Handles.Label(position, text, style);
        }

        #endregion
    }
}