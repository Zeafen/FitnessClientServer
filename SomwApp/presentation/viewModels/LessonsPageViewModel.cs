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
    public class LessonsPageViewModel : INotifyPropertyChanged
    {
        private RelayCommand<LessonsModel>? _lessonEditCommand = null;
        public RelayCommand<LessonsModel> LessonEditCommand => _lessonEditCommand ??= new RelayCommand<LessonsModel>(EditLesson, lesson => 
        lesson != null && Lessons.FirstOrDefault(l => l.ID_Lessons == lesson.ID_Lessons) != null
        && lesson.Coach != null && lesson.NumberOfPracticants > 0
        && !string.IsNullOrEmpty(lesson.Title)
        && Branches.Any(b => b.ID_Branches == lesson.Branch.ID_Branches));

        private RelayCommand<LessonsModel>? _lessonAddCommand = null;
        public RelayCommand<LessonsModel> LessonAddCommand => _lessonAddCommand ??= new RelayCommand<LessonsModel>(AddLesson, CanAddLesson);

        private RelayCommand<LessonsModel>? _lessonDeleteCommand = null;
        public RelayCommand<LessonsModel> LessonDeleteCommand => _lessonDeleteCommand ??= new RelayCommand<LessonsModel>(DeleteLesson, model => model != null && Lessons.FirstOrDefault(p => p.ID_Lessons == model.ID_Lessons) != null);

        private RelayCommand? _clearFiltersCommand = null;
        public RelayCommand ClearFiltersCommand => _clearFiltersCommand ??= new RelayCommand(obj => RemoveFilters(), obj => true);

        public ObservableCollection<LessonsModel> Lessons { get; set; } = new ObservableCollection<LessonsModel>();
        public ObservableCollection<Coach> Coaches { get; set; } = new ObservableCollection<Coach>();
        public ObservableCollection<Branches> Branches { get; set; } = new ObservableCollection<Branches>();

        private ICoachesDataSource _coachesDataSource = CoachesFitDataSource.GetInstance();
        private ILessonsDataSource _lessonsDataSource = LessonsFitDataSource.GetInstance();
        private IBranchesDataSource _branchesDataSource = BranchesFitDataSource.GetInstance();
        private CancellationTokenSource _cts = new CancellationTokenSource();


        public LessonsPageViewModel()
        {

            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(Coaches, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Lessons, lockDownloads);
            BindingOperations.EnableCollectionSynchronization(Branches, lockDownloads);
            UpdateData();
        }
        ~LessonsPageViewModel()
        {
            _cts.Cancel();
        }

        private LessonsModel _selectedLesson { get; set; } = new LessonsModel();
        public LessonsModel SelectedLesson
        {
            get => _selectedLesson;
            set
            {
                if(_selectedLesson != value)
                {
                    _selectedLesson = value;
                    onPropertyChanged();
                }
            }
        }

        private Coach? _coachFilter = null;
        public Coach? CoachFilter
        {
            get => _coachFilter;
            set
            {
                if (_coachFilter != value)
                {
                    onClearFilters();
                    _coachFilter = value;
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

        private TimeOnly? _timeFilter = null;
        public TimeOnly? TimeFilter
        {
            get => _timeFilter;
            set
            {
                if (_timeFilter.HasValue && _timeFilter.Value.CompareTo(value) != 0 || !_timeFilter.HasValue)
                {
                    onClearFilters();
                    _timeFilter = value;
                    onPropertyChanged();
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    ApplyFilters();
                }
            }
        }


        /// <summary>
        /// Очистка фильтров
        /// </summary>
        private void RemoveFilters()
        {
            _dateFromFilter = null;
            onPropertyChanged(nameof(DateFromFilter));
            _dateToFilter = null;
            onPropertyChanged(nameof(DateToFilter));
            _timeFilter = null;
            onPropertyChanged(nameof(TimeFilter));
            _coachFilter = null;
            onPropertyChanged(nameof(CoachFilter));
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ApplyFilters();
        }

        /// <summary>
        /// Очистка фильтров при выборе нового значения
        /// </summary>
        /// <param name="propertyName">Название свойства фильтрации</param>
        private void onClearFilters([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(CoachFilter):
                    if (_coachFilter == null)
                    {

                        _dateFromFilter = null;
                        onPropertyChanged(nameof(DateFromFilter));
                        _dateToFilter = null;
                        onPropertyChanged(nameof(DateToFilter));
                        _timeFilter = null;
                        onPropertyChanged(nameof(TimeFilter));
                    }
                    break;
                case nameof(TimeFilter):
                    if (!_timeFilter.HasValue)
                    {
                        _dateFromFilter = null;
                        onPropertyChanged(nameof(DateFromFilter));
                        _dateToFilter = null;
                        onPropertyChanged(nameof(DateToFilter));
                        _coachFilter = null;
                        onPropertyChanged(nameof(CoachFilter));
                    }
                    break;
                case nameof(DateFromFilter):
                    if (!_dateFromFilter.HasValue)
                    {
                        _coachFilter = null;
                        onPropertyChanged(nameof(CoachFilter));
                        _timeFilter = null;
                        onPropertyChanged(nameof(TimeFilter));
                    }
                    break;
                case nameof(DateToFilter):
                    if (!_dateToFilter.HasValue)
                    {
                        _coachFilter = null;
                        onPropertyChanged(nameof(CoachFilter));
                        _timeFilter = null;
                        onPropertyChanged(nameof(TimeFilter));
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
                Lessons.Clear();

                if (CoachFilter != null)
                {
                    foreach (var lesson in _lessonsDataSource.GetLessonsByCoach(CoachFilter.ID_Coaches)??new List<Lesson>())
                        Lessons.Add(new LessonsModel()
                        {
                            ID_Lessons = lesson.ID_Lessons,
                            Coach = Coaches.FirstOrDefault(c => c.ID_Coaches == lesson.ID_Coaches) ?? new Coach(),
                            Branch = Branches.FirstOrDefault(b => b.ID_Branches == lesson.ID_Branches) ?? new Branches(),
                            Date = lesson.Date,
                            NumberOfPracticants = lesson.NumberOfPracticants,
                            Time = lesson.Time,
                            Title = lesson.Title,
                        });
                }
                else if (DateFromFilter.HasValue || DateToFilter.HasValue)
                {
                    foreach (var lesson in _lessonsDataSource.GetLessonsInPeriod(
                        DateFromFilter.HasValue ? DateFromFilter.Value : DateOnly.MinValue,
                        DateToFilter.HasValue ? DateToFilter.Value : DateOnly.MaxValue) ?? new List<Lesson>())
                        Lessons.Add(new LessonsModel()
                        {
                            ID_Lessons = lesson.ID_Lessons,
                            Coach = Coaches.FirstOrDefault(c => c.ID_Coaches == lesson.ID_Coaches) ?? new Coach(),
                            Branch = Branches.FirstOrDefault(b => b.ID_Branches == lesson.ID_Branches) ?? new Branches(),
                            Date = lesson.Date,
                            NumberOfPracticants = lesson.NumberOfPracticants,
                            Time = lesson.Time,
                            Title = lesson.Title,
                        });
                }
                else if (TimeFilter.HasValue)
                {
                    foreach (var lesson in _lessonsDataSource.GetLessonsByTime(TimeFilter.Value) ?? new List<Lesson>())
                        Lessons.Add(new LessonsModel()
                        {
                            ID_Lessons = lesson.ID_Lessons,
                            Coach = Coaches.FirstOrDefault(c => c.ID_Coaches == lesson.ID_Coaches) ?? new Coach(),
                            Branch = Branches.FirstOrDefault(b => b.ID_Branches == lesson.ID_Branches) ?? new Branches(),
                            Date = lesson.Date,
                            NumberOfPracticants = lesson.NumberOfPracticants,
                            Time = lesson.Time,
                            Title = lesson.Title,
                        });
                }
                else foreach (LessonsModel l in from lesson in _lessonsDataSource.GetLessons()
                                                select new LessonsModel()
                                                {
                                                    ID_Lessons = lesson.ID_Lessons,
                                                    Coach = Coaches.FirstOrDefault(c => c.ID_Coaches == lesson.ID_Coaches) ?? new Coach(),
                                                    Branch = Branches.FirstOrDefault(b => b.ID_Branches == lesson.ID_Branches) ?? new Branches(),
                                                    Date = lesson.Date,
                                                    NumberOfPracticants = lesson.NumberOfPracticants,
                                                    Time = lesson.Time,
                                                    Title = lesson.Title,
                                                    Hours = lesson.DurationClasses,
                                                })
                        Lessons.Add(l);
            }, _cts.Token);
        }

        /// <summary>
        /// Загрузка данных о занятиях, тренерах, филиалах
        /// </summary>
        private void UpdateData()
        {
            Task.Run(() =>
            {
                Lessons.Clear();
                Coaches.Clear();
                foreach (var c in _coachesDataSource.GetCoaches()??new List<Coach>())
                    Coaches.Add(c);
                foreach (var b in _branchesDataSource.GetBranches()??new List<Branches>())
                    Branches.Add(b);

                foreach (LessonsModel l in from lesson in _lessonsDataSource.GetLessons()
                                           select new LessonsModel()
                                           {
                                               ID_Lessons = lesson.ID_Lessons,
                                               Coach = Coaches.FirstOrDefault(c => c.ID_Coaches == lesson.ID_Coaches) ?? new Coach(),
                                               Branch = Branches.FirstOrDefault(b => b.ID_Branches == lesson.ID_Branches) ?? new Branches(),
                                               Date = lesson.Date,
                                               NumberOfPracticants = lesson.NumberOfPracticants,
                                               Time = lesson.Time,
                                               Title = lesson.Title,
                                               Hours = lesson.DurationClasses,
                                           })
                    Lessons.Add(l);
            }, _cts.Token);
        }

        /// <summary>
        /// Отправка запроса на изменение занятия
        /// </summary>
        /// <param name="model">Измененное занятиет</param>
        private void EditLesson(LessonsModel model)
        {
            _lessonsDataSource.EditLesson((Lesson)model);
            ApplyFilters();
        }
        /// <summary>
        /// Проверка, может ли занятие быть добавлено
        /// </summary>
        /// <param name="model">Добавляемое занятие</param>
        /// <returns>True - запись модет быть добавлена, false - не может</returns>
        private bool CanAddLesson(LessonsModel model)
        {
            if (model == null || model.Coach == null || model.Branch == null || string.IsNullOrEmpty(model.Title) || model.NumberOfPracticants < 0 || model.Date.ToDateTime(model.Time).CompareTo(DateTime.Now) < 0)
                return false;
            if (!Branches.Any(b => b.ID_Branches == model.Branch?.ID_Branches))
                return false;
            return !Lessons.Any(l => l.Coach == model.Coach && l.Date.ToDateTime(l.Time).CompareTo(model.Date.ToDateTime(model.Time)) == 0);
        }

        /// <summary>
        /// Отправка запроса на добавление занятия
        /// </summary>
        /// <param name="model">Добавляемое занятие</param>
        private void AddLesson(LessonsModel model)
        {
            _lessonsDataSource.AddLesson((Lesson)model);
            ApplyFilters();
        }

        /// <summary>
        /// Отправка запроса на удаление занятия
        /// </summary>
        /// <param name="model">Удаляемое занятие</param>
        private void DeleteLesson(LessonsModel model)
        {
            _lessonsDataSource.DeleteLesson(model.ID_Lessons);
            ApplyFilters();
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
