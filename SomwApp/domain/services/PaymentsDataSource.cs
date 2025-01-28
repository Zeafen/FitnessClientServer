using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface IPaymentsDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение записей оплат
        /// </summary>
        /// <returns>Список записей оплат</returns>
        public List<Payment>? GetPayments();

        /// <summary>
        /// Отправляет запрос на получение записей оплат, отсортированных по идентификатору клиента
        /// </summary>
        /// <param name="customerID">Идентификатор клиента</param>
        /// <returns>Список записей оплат, отсортированных по идентификатору клиента</returns>
        public List<Payment>? GetPaymentsByCustomer(int customerID);
        /// <summary>
        /// Отправляет запрос на получение записей оплат, отсортированных по идентификатору абонемента
        /// </summary>
        /// <param name="subscriptionID">Идентификатор абонемента</param>
        /// <returns>Список записей оплат, отсортированных по идентификатору абонемента</returns>
        public List<Payment>? GetPaymentsBySubscription(int subscriptionID);

        /// <summary>
        /// Отправляет запрос на добавление оплаты
        /// </summary>
        /// <param name="payment">Добавляемая оплата</param>
        public void AddPayment(Payment payment);

        /// <summary>
        /// Отправляет запрос на изменение оплаты
        /// </summary>
        /// <param name="payment">Измененная оплата</param>
        public void EditPayment(Payment payment);

        /// <summary>
        /// Отправляет запрос на удаление оплатф по идентификатору
        /// </summary>
        /// <param name="paymentID">Идентификатор оплаты</param>
        public void DeletePayment(int paymentID);

        /// <summary>
        /// Отправляет запрос на получени еоплаты по идентификатору
        /// </summary>
        /// <param name="id">Идентификаор оплаты</param>
        /// <returns>Запись оплаты с ID , соответствующим переданному</returns>
        public Payment? GetPayment(int id);
    }
}
