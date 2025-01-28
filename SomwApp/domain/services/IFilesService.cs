using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface IFilesService
    {
        /// <summary>
        /// Отправляет запрос на получение всех отчётов
        /// </summary>
        /// <returns>Список всех отчётов</returns>
        public List<ReviewFile>? GetFiles();

        /// <summary>
        /// Отправляет запрос на получение всех отчётов, отсортированных по типу отчёта
        /// </summary>
        /// <param name="type">Тип отчёта</param>
        /// <returns>Список всех отчётов, отсортированных по типу отчёта</returns>
        public List<ReviewFile>? GetFilesByType(ReviewType type);

        /// <summary>
        /// Отправляет запрос на получение всех отчётов, отсортированных дате создания
        /// </summary>
        /// <param name="date">Дата создания</param>
        /// <returns>Список всех отчётов, отсортированных по дате создания</returns>
        public List<ReviewFile>? GetFilesByDate(DateTime date);

        /// <summary>
        /// Отправляет запрос на получение всех отчётов созданных в период времени
        /// </summary>
        /// <param name="dateFrom">Дата начала периода времени, в который создан отчёт</param>
        /// <param name="dateTo">Дата окончания периода времени, в который создан отчёт</param>
        /// <returns>Список всех отчётов созданных в период времени</returns>
        public List<ReviewFile>? GetFilesInPeriod(DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Отправляет запрос на добавление отчёта
        /// </summary>
        /// <param name="fileRequest">Запрос на добавление отчёта</param>
        public void CreateFile(ReviewFileRequest fileRequest);

        /// <summary>
        /// Отправляет запрос на удаление отчёта
        /// </summary>
        /// <param name="fileName">Название отчёта</param>
        public void DeleteFile(string fileName);
    }
}
