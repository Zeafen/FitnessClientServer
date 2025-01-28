using SomwApp.data.FitnessApiDataSources;
using SomwApp.domain.commands;
using SomwApp.domain.models;
using SomwApp.domain.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SomwApp.presentation.viewModels
{
    public class AccountsPageViewModel : INotifyPropertyChanged
    {
        private IAccountsDataSource _accsDataSource = UserAccountsFitDataSource.GetInstance();
        private IRolesDataSource _rolesDataSource = RolesFitDataSource.GetInstance();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private RelayCommand<UserAccountModel>? _accountAddCommand = null;
        public RelayCommand<UserAccountModel> AccountAddCommand => _accountAddCommand ??= new RelayCommand<UserAccountModel>(AddUserAccount, CanAddAccount);

        private RelayCommand<UserAccountModel>? _accountEditCommand = null;
        public RelayCommand<UserAccountModel> AccountEditCommand => _accountEditCommand ??= new RelayCommand<UserAccountModel>(EditUserAccount, CanEditAccount);

        private RelayCommand? _clearFiltersCommand = null;
        public RelayCommand ClearFiltersCommand => _clearFiltersCommand ??= new RelayCommand(obj => { RoleFilter = null; }, obj => true);

        private RelayCommand<UserAccountModel>? _accountDeleteCommand = null;
        public RelayCommand<UserAccountModel> AccountDeleteCommand => _accountDeleteCommand ??= new RelayCommand<UserAccountModel>(DeleteUserAccount, model => model != null && UserAccounts.Any(ac => ac.ID_UserAccounts == model.ID_UserAccounts));


        public ObservableCollection<UserAccountModel> UserAccounts { get; private set; } = new ObservableCollection<UserAccountModel>();
        public ObservableCollection<Role> Roles { get; private set; } = new ObservableCollection<Role>();

        private UserAccountModel _selectedAccount { get; set; } = new UserAccountModel();
        public UserAccountModel SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if(value != null && _selectedAccount != value)
                {
                    _selectedAccount = value;
                    onPropertyChanged();
                }
            }
        }

        private Role? _roleFilter { get; set; } = null;
        public Role? RoleFilter
        {
            get => _roleFilter;
            set
            {
                if (_roleFilter != value)
                {
                    _roleFilter = value;
                    onPropertyChanged();
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }

        /// <summary>
        /// Применение фильтров
        /// </summary>
        private void ApplyFilters()
        {
            Task.Run(() =>
            {
                UserAccounts.Clear();
                if (RoleFilter != null)
                {
                    foreach (var account in _accsDataSource.GetAccountsByRole(RoleFilter.ID_Role) ?? new List<UserAccounts>())
                        UserAccounts.Add(new UserAccountModel()
                        {
                            ID_UserAccounts = account.ID_UserAccounts,
                            Login = account.Login,
                            Password = account.Password,
                            Role = Roles.FirstOrDefault(r => r.ID_Role == account.ID_Roles) ?? new Role()
                        });
                }
                else foreach (var account in _accsDataSource.GetAccounts() ?? new List<UserAccounts>())
                        UserAccounts.Add(new UserAccountModel()
                        {
                            ID_UserAccounts = account.ID_UserAccounts,
                            Login = account.Login,
                            Password = account.Password,
                            Role = Roles.FirstOrDefault(r => r.ID_Role == account.ID_Roles) ?? new Role()
                        });
            }, _cts.Token);
        }

        public AccountsPageViewModel()
        {

            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(UserAccounts, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Roles, lockDownloads);
            UpdateData();
        }


        /// <summary>
        /// Подгрузка данных 
        /// </summary>
        public void UpdateData()
        {
            Task.Run(() =>
            {
                UserAccounts.Clear();
                Roles.Clear();
                foreach (var role in _rolesDataSource.GetRoles() ?? new List<Role>())
                    Roles.Add(role);

                foreach (var account in _accsDataSource.GetAccounts() ?? new List<UserAccounts>())
                    UserAccounts.Add(new UserAccountModel()
                    {
                        ID_UserAccounts = account.ID_UserAccounts,
                        Login = account.Login,
                        Password = account.Password,
                        Role = Roles.FirstOrDefault(r => r.ID_Role == account.ID_Roles) ?? new Role()
                    });
            }, _cts.Token);
        }

        /// <summary>
        /// Проверяет, можно ли добавить переданную учётную запись
        /// </summary>
        /// <param name="model">Добавляемая учётная запись</param>
        /// <returns>True - значение допустимо для добавления, false - недопустимо</returns>
        private bool CanAddAccount(UserAccountModel model)
        {
            if (model == null || model.Role == null || !checkLogin(model.Login) || !checkPassword(model.Password) || UserAccounts.Any(ac => ac.Password == model.Password && ac.Login == model.Login) || !Roles.Any(r => r.ID_Role == model.Role.ID_Role)) return false;
            return true;
        }
        /// <summary>
        /// Проверяет, можно ли изменить переданную учётную запись
        /// </summary>
        /// <param name="model">Измененная учётная запись</param>
        /// <returns>True - значение допустимо для изменения, false - недопустимо</returns>
        private bool CanEditAccount(UserAccountModel model)
        {
            if (model == null || model.Role == null || !Roles.Any(r => r.ID_Role == model.Role.ID_Role) || !checkLogin(model.Login) || !checkPassword(model.Password) || !UserAccounts.Any(ac => ac.ID_UserAccounts == model.ID_UserAccounts)) return false;
            return true;
        }

        /// <summary>
        /// Отправляет запрос на добавление учётной записи
        /// </summary>
        /// <param name="model">Добавляемая учётная запись</param>
        private void AddUserAccount(UserAccountModel model)
        {
            _accsDataSource.AddAccount((UserAccounts)model);
            ApplyFilters();
        }

        /// <summary>
        /// Отправляет запрос на изменение учётной записи
        /// </summary>
        /// <param name="model">Измененная учетная запись</param>
        private void EditUserAccount(UserAccountModel model)
        {
            _accsDataSource.EditAccount((UserAccounts)model);
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ApplyFilters();
        }

        /// <summary>
        /// Отправляет запрос на удаление учётной записи
        /// </summary>
        /// <param name="model">Удаляемая учетная запись</param>
        private void DeleteUserAccount(UserAccountModel model)
        {
            _accsDataSource.DeleteAccount(model.ID_UserAccounts);
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ApplyFilters();
        }


        /// <summary>
        /// Проверка соответствия пароля требованиям
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <returns>True - пароль соответствует требованиям, false - не соответствует</returns>
        private bool checkPassword(string password)
        {
            var hasEnoughLetters = password.Length > 8;
            var hasCapital = password.Any("QWERTYUIOPASDFGHJKLZXCVBNM".Contains);
            var hasNumber = password.Any("1234567890".Contains);
            var hasSpecial = password.Any(" !\"\'@#$%^(){}*-_=+<>.,\\/;:\'\"|?~`".Contains);

            if (hasCapital && hasEnoughLetters && hasNumber && hasSpecial)
                return true;
            return false;
        }

        /// <summary>
        /// Проверка соответствия логина требованиям
        /// </summary>
        /// <param name="login"></param>
        /// <returns>True - логин соответствует требованиям, false - не соответствует</returns>
        private bool checkLogin(string login)
        {
            var hasEnoughLetters = login.Length >= 5 && login.Length <= 20;
            var hasSpecials = login.Any(" !\"\'@#$%^(){}*-_=+<>.,\\/;:\'\"|?~`".Contains);
            if (hasEnoughLetters && !hasSpecials)
                return true;
            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
