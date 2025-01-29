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
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace SomwApp.presentation.viewModels
{
    public class AppointmentsPageViewModel : INotifyPropertyChanged
    {
        private IAppointmentsForClassesDataSource _appointmentsDataSource = AppointmentsFitDataSource.GetInstance();
        private ILessonsDataSource _lessonsDataSource = LessonsFitDataSource.GetInstance();
        private ICustomersDataSource _custsDataSource = CustomersFitDataSource.GetInstance();

        private RelayCommand<AppointmentsForClassesModel>? _appointmentEditCommand = null;
        public RelayCommand<AppointmentsForClassesModel> AppointmentEditCommand => _appointmentEditCommand ??= new RelayCommand<AppointmentsForClassesModel>(EditAppointment, CanEditAppointment);

        private RelayCommand<AppointmentsForClassesModel>? _appointmentAddCommand = null;
        public RelayCommand<AppointmentsForClassesModel> AppointmentAddCommand => _appointmentAddCommand ??= new RelayCommand<AppointmentsForClassesModel>(AddAppointment, CanAddAppointment);
            
        private RelayCommand<AppointmentsForClassesModel>? _appointmentDeleteCommand = null;
        public RelayCommand<AppointmentsForClassesModel> AppointmentDeleteCommand => _appointmentDeleteCommand ??= new RelayCommand<AppointmentsForClassesModel>(DeleteAppointment, model => model != null && Appointments.Any(p => p.ID_AppointmentsForClasses == model.ID_AppointmentsForClasses));


        public ObservableCollection<AppointmentsForClassesModel> Appointments { get; set; } = new ObservableCollection<AppointmentsForClassesModel>();
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        public ObservableCollection<Lesson> Lessons { get; set; } = new ObservableCollection<Lesson>();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private AppointmentsForClassesModel _selectedAppointment { get; set; } = new AppointmentsForClassesModel();
        public AppointmentsForClassesModel SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                if (_selectedAppointment != value)
                {
                    _selectedAppointment = value;
                    onPropertyChanged();
                }
            }
        }

        private Customer? _customerFilter { get; set; } = null;
        public Customer? CustomerFilter
        {
            get => _customerFilter;
            set
            {
                if(_customerFilter != value)
                {
                    onClearFilters();
                    _customerFilter = value;
                    onPropertyChanged();
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }

        private Lesson? _lessonFilter { get; set; } = null;
        public Lesson? LessonFilter
        {
            get => _lessonFilter;
            set
            {
                if(_lessonFilter != value)
                {
                    onClearFilters();
                    _lessonFilter = value;
                    onPropertyChanged();
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }

        private DateOnly? _dateFromFilter = null;
        public DateOnly? DateFromFilter
        {
            get => _dateFromFilter;
            set
            {
                if (_dateFromFilter.HasValue && _dateFromFilter.Value.CompareTo(value) != 0 || !_dateFromFilter.HasValue)
                {
                    onClearFilters();
                    _dateFromFilter = value;
                    onPropertyChanged();
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }
        private DateOnly? _dateToFilter = null;
        public DateOnly? DateToFilter
        {
            get => _dateToFilter;
            set
            {
                if (_dateToFilter.HasValue && _dateToFilter.Value.CompareTo(value) != 0 || !_dateToFilter.HasValue)
                {
                    onClearFilters();
                    _dateToFilter = value;
                    onPropertyChanged();
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }

        public AppointmentsPageViewModel()
        {

            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(Appointments, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Customers, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Lessons, lockDownloads);
            UpdateData();
        }
        ~AppointmentsPageViewModel()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// Очистка фильтров при выборе нового значения
        /// </summary>
        /// <param name="propertyName">Название свойства фильтрации</param>
        private void onClearFilters([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(CustomerFilter):
                    if(_customerFilter == null)
                    {
                        _dateFromFilter = null;
                        onPropertyChanged(nameof(DateFromFilter));
                        _dateToFilter = null;
                        onPropertyChanged(nameof(DateToFilter));
                        _lessonFilter = null;
                        onPropertyChanged(nameof(LessonFilter));
                    }
                    break;
                case nameof(LessonFilter):
                    if (_customerFilter == null)
                    {
                        _dateFromFilter = null;
                        onPropertyChanged(nameof(DateFromFilter));
                        _dateToFilter = null;
                        onPropertyChanged(nameof(DateToFilter));
                        _customerFilter = null;
                        onPropertyChanged(nameof(CustomerFilter));
                    }
                    break;
                case nameof(DateFromFilter):
                    if (!_dateFromFilter.HasValue)
                    {
                        _lessonFilter = null;
                        onPropertyChanged(nameof(LessonFilter));
                        _customerFilter = null;
                        onPropertyChanged(nameof(CustomerFilter));
                    }
                    break;
                case nameof(DateToFilter):
                    if (!_dateToFilter.HasValue)
                    {
                        _lessonFilter = null;
                        onPropertyChanged(nameof(LessonFilter));
                        _customerFilter = null;
                        onPropertyChanged(nameof(CustomerFilter));
                    }
                    break;
            }
        }

        /// <summary>
        /// Применение фильтров
        /// </summary>
        private void ApplyFilters()
        {
            Task.Run(() =>
            {
                    Appointments.Clear();
                if (CustomerFilter != null)
                {
                    foreach (var appointment in _appointmentsDataSource.GetAppointmentsByCustomer(CustomerFilter.ID_Customers) ?? new List<AppointmentsForClasses>())
                        Appointments.Add(new AppointmentsForClassesModel()
                        {
                            ID_AppointmentsForClasses = appointment.ID_AppointmentsForClasses,
                            Customer = Customers.FirstOrDefault(c => c.ID_Customers == appointment.ID_Customers) ?? new Customer(),
                            Lesson = Lessons.FirstOrDefault(l => l.ID_Lessons == appointment.ID_Lessons) ?? new Lesson(),
                            Date_recording = appointment.Date_recording,
                            Status_recording = appointment.Status_recording,
                        });
                }
                else if (LessonFilter != null)
                {
                    foreach (var appointment in _appointmentsDataSource.GetAppointmentsByLessons(LessonFilter.ID_Lessons) ?? new List<AppointmentsForClasses>())
                        Appointments.Add(new AppointmentsForClassesModel()
                        {
                            ID_AppointmentsForClasses = appointment.ID_AppointmentsForClasses,
                            Customer = Customers.FirstOrDefault(c => c.ID_Customers == appointment.ID_Customers) ?? new Customer(),
                            Lesson = _lessonsDataSource.GetLesson(appointment.ID_Lessons) ?? new Lesson(),
                            Date_recording = appointment.Date_recording,
                            Status_recording = appointment.Status_recording,
                        });
                }
                else if (DateFromFilter.HasValue || DateToFilter.HasValue)
                {
                    foreach (var appointment in _appointmentsDataSource.GetAppointmentsInPeriod(
                        DateFromFilter.HasValue ? DateFromFilter.Value : DateOnly.MinValue,
                        DateToFilter.HasValue ? DateToFilter.Value : DateOnly.MaxValue) ?? new List<AppointmentsForClasses>())
                        Appointments.Add(new AppointmentsForClassesModel()
                        {
                            ID_AppointmentsForClasses = appointment.ID_AppointmentsForClasses,
                            Customer = Customers.FirstOrDefault(c => c.ID_Customers == appointment.ID_Customers) ?? new Customer(),
                            Lesson = Lessons.FirstOrDefault(l => l.ID_Lessons == appointment.ID_Lessons) ?? new Lesson(),
                            Date_recording = appointment.Date_recording,
                            Status_recording = appointment.Status_recording,
                        });
                }
                else
                    foreach(var item in from appointment in _appointmentsDataSource.GetAppointments()
                                       select new AppointmentsForClassesModel()
                                       {
                                           ID_AppointmentsForClasses = appointment.ID_AppointmentsForClasses,
                                           Customer = Customers.FirstOrDefault(c => c.ID_Customers == appointment.ID_Customers) ?? new Customer(),
                                           Lesson = Lessons.FirstOrDefault(l => l.ID_Lessons == appointment.ID_Lessons) ?? new Lesson(),
                                           Date_recording = appointment.Date_recording,
                                           Status_recording = appointment.Status_recording,
                                       })
                    Appointments.Add(item);
            }, _cts.Token);
        }

        /// <summary>
        /// Загрузка данных о посещаемости, клиентах, занятиях
        /// </summary>
        private void UpdateData()
        {
            Task.Run(() =>
            {
                Appointments.Clear();
                Customers.Clear();
                Lessons.Clear();
                foreach (var item in _custsDataSource.GetCustomers() ?? new List<Customer>())
                    Customers.Add(item);
                foreach (var item in _lessonsDataSource.GetLessons() ?? new List<Lesson>())
                    Lessons.Add(item);

                foreach (var item in from appointment in _appointmentsDataSource.GetAppointments()
                                     select new AppointmentsForClassesModel()
                                     {
                                         ID_AppointmentsForClasses = appointment.ID_AppointmentsForClasses,
                                         Customer = Customers.FirstOrDefault(c => c.ID_Customers == appointment.ID_Customers) ?? new Customer(),
                                         Lesson = Lessons.FirstOrDefault(l => l.ID_Lessons == appointment.ID_Lessons) ?? new Lesson(),
                                         Date_recording = appointment.Date_recording,
                                         Status_recording = appointment.Status_recording,
                                     })
                    Appointments.Add(item);

            }, _cts.Token);
        }

        /// <summary>
        /// Проверка, может ли отметка о посещении быть добавлена
        /// </summary>
        /// <param name="model">Добавляемое занятие</param>
        /// <returns>True - запись может быть добавлена, false - не может</returns>
        private bool CanAddAppointment(AppointmentsForClassesModel model)
        {
            if (model == null || model.Customer == null || model.Lesson == null) return false;
            var hasCustAlready = Appointments.Any(ap => ap.Customer.ID_Customers == model.Customer.ID_Customers && ap.Lesson.ID_Lessons == model.Lesson.ID_Lessons);
            var maxAmountInLesson = Appointments.Any() && Appointments.Where(ap => ap.Lesson.ID_Lessons == model.Lesson.ID_Lessons).ToList().Count == model.Lesson.NumberOfPracticants;
            if (hasCustAlready || maxAmountInLesson)
                return false;
            return true;
        }

        /// <summary>
        /// Проверка, может ли отметка о посещении быть изменена. Приемлимы ли изменения
        /// </summary>
        /// <param name="model">Добавляемое изменена; изменения приемлимы, false - не может; изменения неприемлимы</returns>
        private bool CanEditAppointment(AppointmentsForClassesModel model)
        {
            if(model == null || model.Customer == null || model.Lesson == null || !Appointments.Any(ap => ap.ID_AppointmentsForClasses == model.ID_AppointmentsForClasses)) return false;
            var hasCustAlready = Appointments.Any(ap => ap.Customer.ID_Customers == model.Customer.ID_Customers && ap.Lesson.ID_Lessons == model.Lesson.ID_Lessons && ap.ID_AppointmentsForClasses != model.ID_AppointmentsForClasses);
            var maxAmountInLesson = Appointments.Where(ap => ap.Lesson.ID_Lessons == model.Lesson.ID_Lessons && ap.ID_AppointmentsForClasses != model.ID_AppointmentsForClasses).ToList().Count == model.Lesson.NumberOfPracticants;
            if (hasCustAlready || maxAmountInLesson)
                return false;
            return true;
        }

        /// <summary>
        /// Отправка запроса на добавление отметки о посещении
        /// </summary>
        /// <param name="model">Добавляемая отметка о посещении</param>
        private void AddAppointment(AppointmentsForClassesModel model)
        {
            _appointmentsDataSource.AddAppointment((AppointmentsForClasses)model);
            ApplyFilters();
        }

        /// <summary>
        /// Отправка запроса на изменение отметки о посещении
        /// </summary>
        /// <param name="model">Измененная отметка о посещении</param>
        private void EditAppointment(AppointmentsForClassesModel model)
        {
            _appointmentsDataSource.EditAppointment((AppointmentsForClasses)model);
            ApplyFilters();
        }

        /// <summary>
        /// Отправка запроса на удаление отметки о посещении
        /// </summary>
        /// <param name="model">Удаляемая отметка о посещении</param>
        private void DeleteAppointment(AppointmentsForClassesModel model)
        {
            _appointmentsDataSource.DeleteAppointment(model.ID_AppointmentsForClasses);
            ApplyFilters();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
