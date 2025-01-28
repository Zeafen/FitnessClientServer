using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface ISubscriptionDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение абонементов
        /// </summary>
        /// <returns>Список всех абонементов</returns>
        public List<Subscription>? GetSubscriptions();

        /// <summary>
        /// Отправляет запрос на добавление абонемента
        /// </summary>
        /// <param name="subscription">Добавляемый абонемент</param>
        public void AddSubscription(Subscription subscription);

        /// <summary>
        /// Отправляет запрос на изменение абонемента
        /// </summary>
        /// <param name="subscription">Измененный абонемент</param>
        public void EditSubscription(Subscription subscription);

        /// <summary>
        /// Отправляет запрос на удаление абонемента по идентификатору
        /// </summary>
        /// <param name="subscriptionID">Идентификатор абонемента</param>
        public void DeleteSubscription(int subscriptionID);

        /// <summary>
        /// Отправляет запрос на получение аобнемента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор абонемента</param>
        /// <returns>Запись абонемента с идентификатором, соответствующим переданному</returns>
        public Subscription? GetSubscription(int id);
    }
}
