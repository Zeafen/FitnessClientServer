using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface IAuthService
    {
        /// <summary>
        /// Отправляет запрос на авторизацию
        /// </summary>
        /// <param name="request">Модель запроса на авторизацию (логин, пароль)</param>
        /// <returns>Результат авторизации (токен, роль)</returns>
        public AuthResponse? LogIn(AuthRequest request);
    }
}
