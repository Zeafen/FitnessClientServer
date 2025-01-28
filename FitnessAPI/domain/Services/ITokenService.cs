using FitnessAPI.domain;

namespace Recipes_API.Domain.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Генерирует токен
        /// </summary>
        /// <param name="conf">Настройка токена</param>
        /// <param name="claims">Дополнительные параметры токена</param>
        /// <returns></returns>
        public string Generate(TokenConfig conf, List<TokenClaim> claims);

    }
}
