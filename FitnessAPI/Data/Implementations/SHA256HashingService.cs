using FitnessAPI.Data.Helpers;
using FitnessAPI.domain;
using Recipes_API.Domain.Services;
using System.Security.Cryptography;
using System.Text;

namespace Recipes_API.Domain.Implementations
{
    public class SHA256HashingService : IHashingService
    {
        /// <summary>
        /// Генерирует хешированный пароль, состоящий хеша пароля, а также значения случайной строки
        /// </summary>
        /// <param name="value">Пароль</param>
        /// <param name="saltLength">Длина случайной строки</param>
        /// <returns>Сгенерированный хешированный пароль</returns>
        public SaltedHash GenerateHash(string value, int saltLength = 32)
        {
            using (SHA256 sha256hash = SHA256.Create())
            {
                var salt = RandomHelper.RandomString(saltLength);
                byte[] hashBytes = sha256hash.ComputeHash(Encoding.Default.GetBytes($"{salt}{value}"));
                var hash = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hash.Append(hashBytes[i].ToString("x2"));
                }

                return new SaltedHash(hash.ToString(), salt);
            }
        }

        /// <summary>
        /// Проверяет соответствие хешированного пароля и введенного
        /// </summary>
        /// <param name="value">веденный пароль</param>
        /// <param name="hash">хешированный пароль</param>
        /// <returns>True -ппароли совпаают, false - нет</returns>
        public bool VerifyHash(string value, SaltedHash hash)
        {
            using (SHA256 sha256hash = SHA256.Create())
            {
                var comparer = StringComparer.Ordinal;
                var usersHash = sha256hash.ComputeHash(Encoding.Default.GetBytes($"{hash.salt}{value}"));
                var compareHash = new StringBuilder();
                for (int i = 0; i < usersHash.Length; i++)
                {
                    compareHash.Append(usersHash[i].ToString("x2"));
                }
                return comparer.Compare(hash.hash, compareHash.ToString()) == 0;
            }
        }
    }
}
