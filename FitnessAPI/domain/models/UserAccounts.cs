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
}
