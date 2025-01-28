using System.Text;

namespace FitnessAPI.Data.Helpers
{
    public static class RandomHelper
    {
        private static Random _random = new Random();
        /// <summary>
        /// Генерирует случайну строку передаваемой длины
        /// </summary>
        /// <param name="length">Длина строки</param>
        /// <returns>Случайную строку передаваемой длины</returns>
        public static string RandomString(int length)
        {
            int randVal;
            char letter;
            var builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                randVal = _random.Next(0, 26);
                letter = Convert.ToChar(randVal + 65);
                builder.Append(letter);
            }
            return builder.ToString();
        }
    }
}
