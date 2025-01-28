using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface ILessonsDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение всех занятий
        /// </summary>
        /// <returns>Списрк всех занятий </returns>
        public List<Lesson>? GetLessons();

        /// <summary>
        /// Отправляет запрос на получение всех занятий, отсортированных по идентификатору тренера
        /// </summary>
        /// <param name="coachID">Идентификатор занятия</param>
        /// <returns>Список всех занятий, отсортированных по идентификатору тренера</returns>
        public List<Lesson>? GetLessonsByCoach(int coachID);

        /// <summary>
        /// Отправляет запрос на получение всех занятий, отсортированных по дате проведения
        /// </summary>
        /// <param name="date">Дата проведения</param>
        /// <returns>Список всех занятий, отсортированных по дате проведения</returns>
        public List<Lesson>? GetLessonsByDate(DateOnly date);

        /// <summary>
        /// Отправляет запрос на получение всех занятий, отсортированных по дате проведения
        /// </summary>
        /// <param name="dateFrom">Дата начала отсчёта периода проведения занятия</param>
        /// <param name="dateTo">Дата окончания отсчёта периода проведения занятия </param>
        /// <returns>Список всех занятий, отсортированных по дате проведения</returns>
        public List<Lesson>? GetLessonsInPeriod(DateOnly dateFrom, DateOnly dateTo);

        /// <summary>
        /// Отправляет запрос на получение всех занятий, отсортированных по времени проведения
        /// </summary>
        /// <param name="time">Время проведения</param>
        /// <returns>Список всех занятий, отсортированных по времени проведения</returns>
        public List<Lesson>? GetLessonsByTime(TimeOnly time);

        /// <summary>
        /// Отправляет запрос на получение всех занятий с датой провдения в пределах 1 дня
        /// </summary>
        /// <returns>Список всех занятий с датой провдения в пределах 1 дня</returns>
        public List<Lesson>? GetRecentLessons();

        /// <summary>
        /// Отправляет запрос на добавление занятия
        /// </summary>
        /// <param name="lesson">Добавляемое занятие</param>
        public void AddLesson(Lesson lesson);

        /// <summary>
        /// Отправляет запрос на изменение занятия
        /// </summary>
        /// <param name="lesson">Измененное занятие</param>
        public void EditLesson(Lesson lesson);

        /// <summary>
        /// Отправляет запрос на удаление занятия
        /// </summary>
        /// <param name="lessonID">удаленное занятие</param>
        public void DeleteLesson(int lessonID);

        /// <summary>
        /// Отправляет запрос на получение занятия по идентификатору
        /// </summary>
        /// <param name="id">Идентификаор занятия</param>
        /// <returns>Запись занятия с идентификатором, соответствующим переданному</returns>
        public Lesson? GetLesson(int id);
    }
}
