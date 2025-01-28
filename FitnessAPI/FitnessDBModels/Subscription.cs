using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class Subscription
{
    public int IdSubscription { get; set; }

    public string TypeofSubscriptuion { get; set; } = null!;

    public decimal Cost { get; set; }

    public int NumberofVisits { get; set; }

    public int ValidityPeriod { get; set; }

    public string Conditions { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
