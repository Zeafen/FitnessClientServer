using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface ICoachesDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение всех тренеров
        /// </summary>
        /// <returns>Список всех тренеров</returns>
        public List<Coach>? GetCoaches();

        /// <summary>
        /// Отправляет запрос на получение всех тренеров, отсортированных по специализации
        /// </summary>
        /// <param name="specialization">Специализация</param>
        /// <returns>Список всех тренеров, отсортированных по специализации</returns>
        public List<Coach>? GetCoachesBySpecialization(string specialization);

        /// <summary>
        /// Отправляет запрос на добавление тренера
        /// </summary>
        /// <param name="coach">Добавляемый тренер</param>
        public void AddCoach(Coach coach);

        /// <summary>
        /// Отправляет запрос на изменение тренера
        /// </summary>
        /// <param name="coach">Измененный тренер</param>
        public void EditCoach(Coach coach);

        /// <summary>
        /// Отправляет запрос на удаление тренера по идентификатору
        /// </summary>
        /// <param name="coachID">Идентификатор тренера</param>
        public void DeleteCoach(int coachID);

        /// <summary>
        /// Отправляет запрос на получение тренера по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор тренера</param>
        /// <returns>Запись тренера с идентификатором, соответствующим переданному</returns>
        public Coach? GetCoach(int id);
    }
}
