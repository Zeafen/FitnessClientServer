using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class Customer
{
    public int IdCustomers { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly BirthDate { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<AppointmentsForClass> AppointmentsForClasses { get; set; } = new List<AppointmentsForClass>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
