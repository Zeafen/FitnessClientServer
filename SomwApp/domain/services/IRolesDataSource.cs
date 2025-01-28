using SomwApp.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.services
{
    public interface IRolesDataSource
    {
        /// <summary>
        /// Отправляет запрос на получени евсех ролей
        /// </summary>
        /// <returns>Список всех ролей</returns>
        public List<Role>? GetRoles();

        /// <summary>
        /// Отправляет запрос на получение записи роли по идентификатору
        /// </summary>
        /// <param name="roleID">Идентификаор роли</param>
        /// <returns>Запись роли с идентификатором, соответствующим переданному</returns>
        public Role? GetRoleByID(int roleID);
    }
}
