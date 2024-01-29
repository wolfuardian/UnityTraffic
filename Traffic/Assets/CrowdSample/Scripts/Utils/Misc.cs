using UnityEngine;

namespace CrowdSample.Scripts.Utils
{
    public static class Misc
    {
        public static string GenerateLicensePlateNumber()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            var rnd = new System.Random();

            var plate = "";
            for (var i = 0; i < 3; i++)
            {
                plate += letters[rnd.Next(letters.Length)];
            }

            plate += "-";
            for (var i = 0; i < 4; i++)
            {
                plate += numbers[rnd.Next(numbers.Length)];
            }

            return plate;
        }

        public static Vector3 GetRandomPointInRadius(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                return center;
            }

            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection += center;
            return new Vector3(randomDirection.x, center.y, randomDirection.z);
        }

        public static Vector3 GetScatterPosition(Vector3 position, float radius)
        {
            var spawnPosition = GetRandomPointInRadius(position, radius);
            return spawnPosition;
        }
    }
}