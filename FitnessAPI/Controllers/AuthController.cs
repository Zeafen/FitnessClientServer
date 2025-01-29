using FitnessAPI.domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes_API.Domain.Services;
using System.IdentityModel.Tokens.Jwt;
using FitnessAPI.FitnessDB;
using FitnessAPI.domain.models;
using SomwApp.domain.models;

namespace FitnessAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IHashingService _hashingService;
        private readonly GymContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly TokenConfig _conf;
        public AuthController(IHashingService hashingService, GymContext dbContext, ITokenService service, TokenConfig conf)
        {
            _hashingService = hashingService;
            _dbContext = dbContext;
            _tokenService = service;
            _conf = conf;
        }

        /// <summary>
        /// Обрабатывает запрос на аторизацию
        /// </summary>
        /// <param name="request">Запрос на авторизацию</param>
        /// <returns>Http-ответ, содержащий результат авторизации (токен и роль)</returns>
        [HttpPost("signIn")]
        public async Task<ActionResult<AuthResponse>> SignIn([FromBody]AuthRequest request)
        {
            try
            {
                var user = _dbContext.UserAccounts.FirstOrDefault(ac => ac.Login == request.Login);
                if (user == null)
                    return Conflict("Логин не найден");

                var isValidPassw = _hashingService.VerifyHash(
                    value: request.Password,
                    hash: new SaltedHash(user.Password, user.Salt));

                if (!isValidPassw)
                    return Conflict("Неправильный пароль");

                var role = _dbContext.Roles.FirstOrDefault(r => r.IdRoles == user.IdRoles);
                if (role == null)
                    return Conflict("Нет данных о роли");

                var token = _tokenService.Generate(
                    _conf, new List<TokenClaim>
                    {
                        new TokenClaim("Role", role.RoleName),
                        new TokenClaim("UserID", user.IdUserAccounts.ToString())
                    });
                return new AuthResponse($"Bearer {token}", role.RoleName);
            }
            catch (Exception ex)
            {
                return Conflict("Непредвиденная ошибка сервера");
            }

        }
    }
}
