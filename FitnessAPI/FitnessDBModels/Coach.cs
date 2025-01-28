using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class Coach
{
    public int IdCoaches { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Specialization { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public decimal LessonsSchedule { get; set; }

    public int? IdUserAccounts { get; set; }

    public virtual UserAccount? IdUserAccountsNavigation { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
