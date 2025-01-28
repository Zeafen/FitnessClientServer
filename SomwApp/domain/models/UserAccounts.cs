using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class UserAccounts
    {
        public int ID_UserAccounts { get; set; }
        public string Password {  get; set; }
        public string Login { get; set; }
        public int ID_Roles { get; set; }
    }

    public class UserAccountModel
    {
        public int ID_UserAccounts { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public Role Role { get; set; }

        public UserAccountModel Copy()
        {
            return (UserAccountModel)MemberwiseClone();
        }

        public static explicit operator UserAccounts(UserAccountModel model)
        {
            return new UserAccounts()
            {
                ID_Roles = model.Role.ID_Role,
                Password = model.Password,
                Login = model.Login,
                ID_UserAccounts = model.ID_UserAccounts
            };
        }
    }
}
