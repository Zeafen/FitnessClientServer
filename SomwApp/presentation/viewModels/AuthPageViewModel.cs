using SomwApp.data.FitnessApiDataSources;
using SomwApp.domain.commands;
using SomwApp.domain.models;
using SomwApp.domain.services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.presentation.viewModels
{
    public class AuthPageViewModel : INotifyPropertyChanged
    {

        private IAuthService _authService = AuthFitDataSource.GetInstance();

        private AuthRequest _authRequest {  get; set; } = new AuthRequest();
        public AuthRequest AuthRequest
        {
            get => _authRequest; 
            set
            {
                if(_authRequest != value)
                {
                    _authRequest = value;
                    onPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Отправка запроса на авторизацию
        /// </summary>
        /// <param name="request">Модель запроса на авторизацию (логин, пароль)</param>
        /// <returns>Результат авторизации (токен, роль)</returns>
        public AuthResponse? ConfirmAuth(AuthRequest request)
        {
            if (string.IsNullOrEmpty(request.Password) && string.IsNullOrEmpty(request.Login))
                return null;
            return _authService.LogIn(request);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
