using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class UserAccount
{
    public int IdUserAccounts { get; set; }

    public string Password { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public int IdRoles { get; set; }

    public virtual Coach? Coach { get; set; }

    public virtual Role IdRolesNavigation { get; set; } = null!;
}
