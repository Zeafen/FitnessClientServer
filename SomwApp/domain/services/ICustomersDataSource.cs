using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface ICustomersDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение всех клиентов
        /// </summary>
        /// <returns>Список всех клиентов</returns>
        public List<Customer>? GetCustomers();

        /// <summary>
        /// Отправляет запрос на добавление клиента
        /// </summary>
        /// <param name="customer">Добавляемый клиент</param>
        public void AddCustomer(Customer customer);

        /// <summary>
        /// Отправляет запрос на изменение клиента
        /// </summary>
        /// <param name="customer">Измененный клиент</param>
        public void EditCustomer(Customer customer);

        /// <summary>
        /// Отправляет запрос на удаление клиента по идентификатору
        /// </summary>
        /// <param name="customerID">Идентификатор клиента</param>
        public void DeleteCustomer(int customerID);

        /// <summary>
        /// Отправляет запрос на получение клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <returns>Запись клиента с идентификатором, соответствующим переданному</returns>
        public Customer? GetCustomer(int id);
    }
}
