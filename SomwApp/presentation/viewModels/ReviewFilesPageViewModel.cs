using SomwApp.data.FitnessApiDataSources;
using SomwApp.domain.commands;
using SomwApp.domain.models;
using SomwApp.domain.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SomwApp.presentation.viewModels
{
    public class ReviewFilesPageViewModel : INotifyPropertyChanged
    {
        private RelayCommand? _clearFiltersCommand = null;
        public RelayCommand ClearFiltersCommand => _clearFiltersCommand ??= new RelayCommand(obj => RemoveFilters(), obj => true);

        private RelayCommand<List<ReviewFile>>? _reviewsDeleteCommand = null;
        public RelayCommand<List<ReviewFile>> ReviewsDeleteCommand => _reviewsDeleteCommand ??= new RelayCommand<List<ReviewFile>>(DeleteReviewFiles, files => files.Any() && !files.Any(f => string.IsNullOrEmpty(f.ReviewName)));

        private RelayCommand<List<ReviewFile>>? _reviewsDownloadCommand = null;
        public RelayCommand<List<ReviewFile>> ReviewsDownloadCommand => _reviewsDownloadCommand ??= new RelayCommand<List<ReviewFile>>(DownloadReviews, files => files.Any());

        private RelayCommand<ReviewFile>? _reviewsCancelDownloadCommand = null;
        public RelayCommand<ReviewFile> ReviewsCancelDownloadCommand => _reviewsCancelDownloadCommand ??= new RelayCommand<ReviewFile>(CancelDownload, file => file != null);

        private RelayCommand<ReviewFileRequest>? _reviewAddCommand = null;
        public RelayCommand<ReviewFileRequest> ReviewAddCommand => _reviewAddCommand ??= new RelayCommand<ReviewFileRequest>(AddReviewFile, request => request != null && !string.IsNullOrEmpty(request.ReviewName));

        private IFilesService _filesService = FilesFitDataSource.GetInstance();

        public HttpClient _DownloadClient { get; private set; }
        public CancellationTokenSource cancelToken {  get; private set; }
        private TaskFactory downloadFileTask;

        public ObservableCollection<ReviewFile> ReviewFiles { get; set; } = new ObservableCollection<ReviewFile>();
        public ObservableCollection<ReviewFile> DownloadQueue { get; set; } = new ObservableCollection<ReviewFile>();
        public List<ReviewFile> ReviewsToOperate { get; set; } = new List<ReviewFile>();
        public List<ReviewType> ReviewTypes { get; private set; } = new List<ReviewType>()
        {
            ReviewType.Branch_Efficiency,
            ReviewType.Attendance,
            ReviewType.Income,
            ReviewType.Trainer_Business
        };

        private ReviewFileRequest _reviewFileRequest { get; set; } = new ReviewFileRequest();
        public ReviewFileRequest ReviewFileRequest
        {
            get => _reviewFileRequest;
            set
            {
                if (value != null && _reviewFileRequest != value)
                {
                    _reviewFileRequest = value;
                    onPropertyChanged();
                }
            }
        }

        private ReviewType? _reviewTypeFilter { get; set; } = null;
        public ReviewType? ReviewTypeFilter
        {
            get => _reviewTypeFilter;
            set
            {
                if (_reviewTypeFilter != value)
                {
                    onClearFilters();
                    _reviewTypeFilter = value;
                    onPropertyChanged();
                    ApplyFilters();
                }
            }
        }
        private DateTime? _dateFromFilter = null;
        public DateTime? DateFromFilter
        {
            get => _dateFromFilter;
            set
            {
                if (_dateFromFilter.HasValue && _dateFromFilter.Value.CompareTo(value) != 0 || !_dateFromFilter.HasValue)
                {
                    onClearFilters();
                    _dateFromFilter = value;
                    onPropertyChanged();
                    ApplyFilters();
                }
            }
        }
        private DateTime? _dateToFilter = null;
        public DateTime? DateToFilter
        {
            get => _dateToFilter;
            set
            {
                if (_dateToFilter.HasValue && _dateToFilter.Value.CompareTo(value) != 0 || !_dateToFilter.HasValue)
                {
                    onClearFilters();
                    _dateToFilter = value;
                    onPropertyChanged();
                    ApplyFilters();
                }
            }
        }


        public ReviewFilesPageViewModel()
        {
            UpdateData();
            downloadFileTask = new TaskFactory();

            object lockDownloads = new object();
            BindingOperations.EnableCollectionSynchronization(DownloadQueue, lockDownloads);
            cancelToken = new CancellationTokenSource();
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            _DownloadClient = new HttpClient(handler);
            downloadFileTask.StartNew(DownloadFile, cancelToken.Token);
        }

        ~ReviewFilesPageViewModel()
        {
            cancelToken.Cancel();
            _DownloadClient.Dispose();
        }


        /// <summary>
        /// Добавляет файл в очередь скачивания
        /// </summary>
        /// <param name="file">Файл отчёта</param>
        public void DownloadReview(ReviewFile? file)
        {
            if (file == null) return;
            if (!DownloadQueue.Contains(file))
            {
                DownloadQueue.Add(file);
            }
        }

        /// <summary>
        /// Добавляет файл в очередь скачивания
        /// </summary>
        /// <param name="files">Файлы отчётов</param>
        public void DownloadReviews(List<ReviewFile>? files)
        {
            if (files == null) return;
            foreach (ReviewFile file in files)
                if (!DownloadQueue.Contains(file))
                    DownloadQueue.Add(file);
        }

        /// <summary>
        /// Отправляет запрос на удаление файла отчёта
        /// </summary>
        /// <param name="files">Файл отчёта для удаления</param>
        private void DeleteReviewFiles(List<ReviewFile> files)
        {
            foreach (ReviewFile file in files)
                _filesService.DeleteFile(file.ReviewName);
            ApplyFilters();
        }

        /// <summary>
        /// Отпралвяет запрос на добавление файла отчёта
        /// </summary>
        /// <param name="request">Добавляемый файл отчёта</param>
        private void AddReviewFile(ReviewFileRequest request)
        {
            _filesService.CreateFile(request);
            ApplyFilters();
        }

        /// <summary>
        /// Проверяет, есть ли в очереди на скачивание файлов файлы. При их наличии запускает скачивание
        /// </summary>
        private async void DownloadFile()
        {
            while (true)
            {
                if (DownloadQueue.Any())
                {
                    if (!cancelToken.IsCancellationRequested)
                        try
                        {
                            var file = DownloadQueue.First();
                            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $@"\{file.ReviewName}.pdf";
                            if (File.Exists(filePath))
                                File.Delete(filePath);
                            using (var stream = await _DownloadClient.GetStreamAsync(file.ReviewAddress))
                            {
                                using (var createdFile = File.Create(filePath))
                                {
                                    await stream.CopyToAsync(createdFile);
                                }
                            }
                            DownloadQueue.RemoveAt(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("При загрузке отчёта возникла непредвиденная ошибка. Проверьте подключение к интернету или ссылку на скачиваемый файл", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                }
            }
        }

        /// <summary>
        /// Удаляет из очреди на скачивание файлов запись. Если файл скачивается в данный момент, скачивание приостанавливается, сохраненный файл удаляестя.
        /// </summary>
        /// <param name="file">Запись для удалоения из очереди</param>
        public void CancelDownload(ReviewFile? file)
        {
            if (file == null) return;
            if (DownloadQueue.IndexOf(file) == 0)
            {
                cancelToken.Cancel();
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $@"\{file.ReviewName}.pdf"))
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $@"\{file.ReviewName}.pdf");
                DownloadQueue.Remove(file);
                downloadFileTask.StartNew(DownloadFile, cancelToken.Token);
            }
            else
            {
                DownloadQueue.Remove(file);
            }
        }

        /// <summary>
        /// Загрузка данных массивов
        /// </summary>
        private void UpdateData()
        {
            Task.Run(() =>
            {
                ReviewFiles.Clear();

                foreach (var file in _filesService.GetFiles() ?? new List<ReviewFile>())
                    ReviewFiles.Add(file);
            }, cancelToken.Token);
        }

        /// <summary>
        /// Применение фильтров
        /// </summary>
        private void ApplyFilters()
        {
            Task.Run(() =>
            {
                if (ReviewTypeFilter.HasValue)
                {
                    ReviewFiles.Clear();
                    foreach (var file in _filesService.GetFilesByType(ReviewTypeFilter.Value) ?? new List<ReviewFile>())
                        ReviewFiles.Add(file);
                }
                else if (DateFromFilter.HasValue || DateToFilter.HasValue)
                {
                    ReviewFiles.Clear();
                    foreach (var file in _filesService.GetFilesInPeriod(
                        DateFromFilter.HasValue ? DateFromFilter.Value : DateTime.MinValue,
                        DateToFilter.HasValue ? DateToFilter.Value : DateTime.MaxValue) ?? new List<ReviewFile>())
                        ReviewFiles.Add(file);
                }
                else UpdateData();
            }, cancelToken.Token);
        }

        /// <summary>
        /// Очистка значения фильров в зависимости от значения выбранного.
        /// </summary>
        /// <param name="propertyName">Навзвание выбранного свойства фильтрации</param>
        private void onClearFilters([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(ReviewTypeFilter):
                    if (!ReviewTypeFilter.HasValue)
                    {
                        _dateFromFilter = null;
                        onPropertyChanged(nameof(DateFromFilter));
                        _dateToFilter = null;
                        onPropertyChanged(nameof(DateToFilter));
                    }
                    break;
                case nameof(DateFromFilter):
                    if (!DateFromFilter.HasValue)
                    {
                        _reviewTypeFilter = null;
                        onPropertyChanged(nameof(ReviewTypeFilter));
                    }
                    break;
                case nameof(DateToFilter):
                    if (!DateToFilter.HasValue)
                    {
                        _reviewTypeFilter = null;
                        onPropertyChanged(nameof(ReviewTypeFilter));
                    }
                    break;
            }
        }

        /// <summary>
        /// Полная очистка фильтров
        /// </summary>
        private void RemoveFilters()
        {
            _reviewTypeFilter = null;
            onPropertyChanged(nameof(ReviewTypeFilter));
            _dateFromFilter = null;
            onPropertyChanged(nameof(DateFromFilter));
            _dateToFilter = null;
            onPropertyChanged(nameof(DateToFilter));
            ApplyFilters();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
