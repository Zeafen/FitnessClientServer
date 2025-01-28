using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface IBranchesDataSource
    {
        /// <summary>
        /// Отправляет запрос на получение всех филиалов
        /// </summary>
        /// <returns>Список всех филиалов</returns>
        public List<Branches>? GetBranches();

        /// <summary>
        /// Отправляет запрос на получение филиала по идентификатору
        /// </summary>
        /// <param name="branchID">Идентификатор филиала</param>
        /// <returns>Щапись филиала с идентификаторм, соответствующим переданному</returns>
        public Branches? GetBranch(int branchID);

        /// <summary>
        /// Отправляет запрос на добавление филиала
        /// </summary>
        /// <param name="branch">Добавляемый филиал</param>
        public void AddBranch(Branches branch);

        /// <summary>
        /// Отправляет запрос на  изенение филиала
        /// </summary>
        /// <param name="branch">Измененный филиал</param>
        public void EditBranch(Branches branch);

        /// <summary>
        /// Отправляет запрос на удаление филиала по идентификатору
        /// </summary>
        /// <param name="branchID">Идентификатор филиала</param>
        public void DeleteBranch(int branchID);
    }
}
