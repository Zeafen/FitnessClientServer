using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class Lesson
{
    public int IdLessons { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public decimal DurationClasses { get; set; }

    public int NumberOfPracticants { get; set; }

    public int IdCoaches { get; set; }

    public int IdBranches { get; set; }

    public virtual ICollection<AppointmentsForClass> AppointmentsForClasses { get; set; } = new List<AppointmentsForClass>();

    public virtual Branch IdBranchesNavigation { get; set; } = null!;

    public virtual Coach IdCoachesNavigation { get; set; } = null!;
}
