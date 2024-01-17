namespace CrowdSample.Scripts.Utils
{
    public static class GeneratorUtils
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
    }
}