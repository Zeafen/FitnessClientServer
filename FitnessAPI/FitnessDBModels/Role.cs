using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class Role
{
    public int IdRoles { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}
