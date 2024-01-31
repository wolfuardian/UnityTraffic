using System.IO;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Crowd
{
    public class LicensePlate
    {
        public string plateNumber  { get; set; }
        public bool   isAuthorized { get; set; }
    }

    public class LicensePlateDataAccess
    {
        public static Dictionary<string, LicensePlate> LoadDataFromCsv(string filePath)
        {
            var dataMap = new Dictionary<string, LicensePlate>();

            using var reader = new StreamReader(filePath);
            while (!reader.EndOfStream)
            {
                var line   = reader.ReadLine();
                var values = line!.Split(',');
                var plate = new LicensePlate
                {
                    plateNumber  = values[0].Trim('\''),
                    isAuthorized = values[1].Trim() == "'1'"
                };
                dataMap[plate.plateNumber] = plate;
            }

            return dataMap;
        }
    }

    public class LicensePlateRegistry
    {
        private readonly Dictionary<string, LicensePlate> plateDataMap;

        public LicensePlateRegistry(string filePath)
        {
            var dataAccess = new LicensePlateDataAccess();
            plateDataMap = LicensePlateDataAccess.LoadDataFromCsv(filePath);
        }

        public bool? CheckPlateAuthorization(string plateNumber)
        {
            if (string.IsNullOrEmpty(plateNumber))
            {
                return null;
            }

            if (plateDataMap.TryGetValue(plateNumber, out var plate))
            {
                return plate.isAuthorized;
            }

            return null;
        }
    }
}