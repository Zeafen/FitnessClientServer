using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class AppointmentsForClass
{
    public int IdAppointmentsForClasses { get; set; }

    public int IdCustomers { get; set; }

    public int IdLessons { get; set; }

    public DateOnly DateRecording { get; set; }

    public string StatusRecording { get; set; } = null!;

    public virtual Customer IdCustomersNavigation { get; set; } = null!;

    public virtual Lesson IdLessonsNavigation { get; set; } = null!;
}
