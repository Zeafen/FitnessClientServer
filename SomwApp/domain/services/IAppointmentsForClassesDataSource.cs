using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface IAppointmentsForClassesDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение всех посещений
        /// </summary>
        /// <returns>Список всех посещений</returns>
        public List<AppointmentsForClasses>? GetAppointments();

        /// <summary>
        /// Отправляет запрос на получение всех посещений, отсортированных по идентификатору клиента
        /// </summary>
        /// <param name="CustomerID">Идентификатор клиента</param>
        /// <returns>Список всех посещений, отсортированных по идентификатору клиента</returns>
        public List<AppointmentsForClasses>? GetAppointmentsByCustomer(int CustomerID);

        /// <summary>
        /// Отправляет запрос на получение всех посещений, отсортированных по идентификатору занятия
        /// </summary>
        /// <param name="LessonID">Идентификатор занятия</param>
        /// <returns>Список всех посещений, отсортированных по идентификатору занятия</returns>
        public List<AppointmentsForClasses>? GetAppointmentsByLessons(int LessonID);

        /// <summary>
        /// Отправляет запрос на получение всех посещений, отсортированных по идентификатору тренера
        /// </summary>
        /// <param name="coachID">Идентификатор тренера</param>
        /// <returns>Список всех посещений, отсортированных по идентификатору тренера</returns>
        public List<AppointmentsForClasses>? GetAppointmentsByCoach(int coachID);

        /// <summary>
        /// Отправляет запрос на получение всех посещений, отсортированных по дате внесения
        /// </summary>
        /// <param name="date">Дата внесения</param>
        /// <returns>Список всех посещений, отсортированных по дате внесения</returns>
        public List<AppointmentsForClasses>? GetAppointmentsByDate(DateOnly date);

        /// <summary>
        /// Отправляет запрос на получение всех посещений, внесенных во временной промежуток
        /// </summary>
        /// <param name="dateFrom">Дата начала временного промежутка</param>
        /// <param name="dateTo">Дата окончания временного промежутка</param>
        /// <returns>Список всех посещений, внесенных во временной промежуток</returns>
        public List<AppointmentsForClasses>? GetAppointmentsInPeriod(DateOnly dateFrom, DateOnly dateTo);

        /// <summary>
        /// Отправляет запрос на добавление посещения
        /// </summary>
        /// <param name="appointment">Добавляемое посещение</param>
        public void AddAppointment(AppointmentsForClasses appointment);

        /// <summary>
        /// Отправляет запрос на изменение посещения
        /// </summary>
        /// <param name="appointment">Измененное посещение</param>
        public void EditAppointment(AppointmentsForClasses appointment);

        /// <summary>
        /// Отправляет запрос на удаление посещения по идентификатору
        /// </summary>
        /// <param name="appointmentID">Идентификатор посещения</param>
        public void DeleteAppointment(int appointmentID);

        /// <summary>
        /// Отправляет запрос на получение записи посещения по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор записи посещения</param>
        /// <returns>Запись посещения с идентификатором, соответствующим переданному</returns>
        public AppointmentsForClasses? GetAppointment(int id);
    }
}
