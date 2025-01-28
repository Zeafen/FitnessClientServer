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
    public class CoachesPageViewModel : INotifyPropertyChanged
    {
        private ICoachesDataSource _coachesDataSource = CoachesFitDataSource.GetInstance();
        private IAccountsDataSource _accsDataSource = UserAccountsFitDataSource.GetInstance();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private RelayCommand<CoachModel>? _coachEditCommand = null;
        public RelayCommand<CoachModel> CoachEditCommand => _coachEditCommand ??= new RelayCommand<CoachModel>(EditCoach, model => model != null && Coaches.Any(c => c.ID_Coaches == model.ID_Coaches));

        private RelayCommand<CoachModel>? _coachAddCommand = null;
        public RelayCommand<CoachModel> CoachAddCommand => _coachAddCommand ??= new RelayCommand<CoachModel>(AddCoach, model => model != null);

        private RelayCommand<CoachModel>? _coachDeleteCommand = null;
        public RelayCommand<CoachModel> CoachDeleteCommand => _coachDeleteCommand ??= new RelayCommand<CoachModel>(DeleteCoach, model => model != null && Coaches.Any(c => c.ID_Coaches == model.ID_Coaches));

        public ObservableCollection<CoachModel> Coaches { get; set; } = new ObservableCollection<CoachModel>();
        public ObservableCollection<UserAccounts> Accounts { get; set; } = new ObservableCollection<UserAccounts>();

        public CoachesPageViewModel()
        {

            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(Coaches, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Accounts, lockDownloads);
            UpdateData();
        }
        ~CoachesPageViewModel()
        {
            _cts.Cancel();
        }
        private CoachModel _selectedCoach { get; set; } = new CoachModel();
        public CoachModel SelectedCoach
        {
            get => _selectedCoach;
            set
            {
                if (_selectedCoach != value)
                {
                    _selectedCoach = value;
                    onPropertyChanged();
                }
            }
        }

        private string? _specializationFilter { get; set; } = null;
        public string? SpecializationFilter
        {
            get => _specializationFilter;
            set
            {
                if (_specializationFilter != value)
                {
                    _specializationFilter = value;
                    onPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Загрузка данных о тренерах, учётных записях
        /// </summary>
        public void UpdateData()
        {
            Task.Run(() =>
            {
                Coaches.Clear();
                Accounts.Clear();

                foreach (var coach in _coachesDataSource.GetCoaches()??new List<Coach>())
                    Coaches.Add(new CoachModel()
                    {
                        ID_Coaches = coach.ID_Coaches,
                        Name = coach.Name,
                        UserAccounts = coach.ID_UserAccounts.HasValue ? _accsDataSource.GetAccountByID(coach.ID_UserAccounts.Value) : null,
                        LessonsSchedule = coach.LessonsSchedule,
                        MiddleName = coach.MiddleName,
                        PhoneNumber = coach.PhoneNumber,
                        Specialization = coach.Specialization,
                        Surname = coach.Surname,
                    });
                foreach (var account in _accsDataSource.GetAccounts()?? new List<UserAccounts>())
                    Accounts.Add(account);
            }, _cts.Token);
        }

        /// <summary>
        /// Применение фильтров
        /// </summary>
        public void ApplyFilters()
        {
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(SpecializationFilter))
                {
                    Coaches.Clear();
                    foreach (var coach in _coachesDataSource.GetCoachesBySpecialization(SpecializationFilter))
                        Coaches.Add(new CoachModel()
                        {
                            ID_Coaches = coach.ID_Coaches,
                            Name = coach.Name,
                            UserAccounts = coach.ID_UserAccounts.HasValue ? _accsDataSource.GetAccountByID(coach.ID_UserAccounts.Value) : null,
                            LessonsSchedule = coach.LessonsSchedule,
                            MiddleName = coach.MiddleName,
                            PhoneNumber = coach.PhoneNumber,
                            Specialization = coach.Specialization,
                            Surname = coach.Surname,
                        });

                }
                else UpdateData();
            }, _cts.Token);
        }

        /// <summary>
        /// Отправка запроса на удаление тренера
        /// </summary>
        /// <param name="model">Удаляемаый тренер</param>
        private void DeleteCoach(CoachModel model)
        {
            _coachesDataSource.DeleteCoach(model.ID_Coaches);
            ApplyFilters();
        }

        /// <summary>
        /// Отправка запроса на изменение тренера
        /// </summary>
        /// <param name="model">Измененный тренер</param>
        private void EditCoach(CoachModel model)
        {
            _coachesDataSource.EditCoach((Coach)model);
            ApplyFilters();
        }

        /// <summary>
        /// Отправка запроса на добавление тренера
        /// </summary>
        /// <param name="model">Добавляемый тренер</param>
        private void AddCoach(CoachModel model)
        {
            _coachesDataSource.AddCoach((Coach)model);
            ApplyFilters();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(SpecializationFilter))
                ApplyFilters();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
