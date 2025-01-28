using FitnessAPI.domain;

namespace Recipes_API.Domain.Services
{
    public interface IHashingService
    {
        /// <summary>
        /// Генерирует хешированный пароль, состоящий хеша пароля, а также значения случайной строки
        /// </summary>
        /// <param name="value">Пароль</param>
        /// <param name="saltLength">Длина случайной строки</param>
        /// <returns>Сгенерированный хешированный пароль</returns>
        public SaltedHash GenerateHash(string value, int saltLength = 32);

        /// <summary>
        /// Проверяет соответствие хешированного пароля и введенного
        /// </summary>
        /// <param name="value">веденный пароль</param>
        /// <param name="hash">хешированный пароль</param>
        /// <returns>True -ппароли совпаают, false - нет</returns>
        public bool VerifyHash(string value, SaltedHash hash);
    }
}
