using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SomwApp.domain.services
{
    public interface IAccountsDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение всех учётных записей
        /// </summary>
        /// <returns>Список учётных записей</returns>
        public List<UserAccounts>? GetAccounts();

        /// <summary>
        /// Отправляет запрос на получение учётных записей, отсортированных по идентификатору роли
        /// </summary>
        /// <param name="roleID">Идентификатор роли</param>
        /// <returns>Список Список учётных записей, отсортированных по роли</returns>
        public List<UserAccounts>? GetAccountsByRole(int roleID);

        /// <summary>
        /// Отправляет запрос на получение учётной записи по идентификатору
        /// </summary>
        /// <param name="accountID">Идентификаор учётной записи</param>
        /// <returns>Учётную запись с ID, соответствующим переданному</returns>
        public UserAccounts? GetAccountByID(int accountID);

        /// <summary>
        /// Отправляет запрос на добавление новой учётной записи
        /// </summary>
        /// <param name="account">Добавляемая учётная запись</param>
        public void AddAccount(UserAccounts account);

        /// <summary>
        /// Отправляет запрос на удаление учётной записи по идентификатору
        /// </summary>
        /// <param name="accountID">Идентификатор учётной записи</param>
        public void DeleteAccount(int accountID);

        /// <summary>
        /// Отправляет запрос на изменение учётной записи по идентификатору
        /// </summary>
        /// <param name="account">Измененнная учётная запись</param>
        public void EditAccount(UserAccounts account);
    }
}
