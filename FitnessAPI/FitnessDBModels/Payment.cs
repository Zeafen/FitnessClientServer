using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class Payment
{
    public int IdPayment { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public int IdCustomers { get; set; }

    public int IdSubscription { get; set; }

    public DateOnly ValidityEndDate { get; set; }

    public virtual Customer IdCustomersNavigation { get; set; } = null!;

    public virtual Subscription IdSubscriptionNavigation { get; set; } = null!;
}
