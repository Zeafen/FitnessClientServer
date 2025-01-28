using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class Payment
    {
        public int ID_Payment { get; set; }
        public DateOnly PaymentDate { get; set; }
        public DateOnly ValidityEndDate { get; set; }
        public double Amount { get; set; }
        public int ID_Customers { get; set; }
        public int ID_Subscription { get; set; }

        public Payment Copy()
        {
            return (Payment)MemberwiseClone();
        }
    }

    public class PaymentModel
    {
        public int ID_Payment { get; set; }
        public DateOnly PaymentDate { get; set; }
        public DateOnly ValidityEndDate { get; set; }
        public double Amount { get; set; }
        public Customer? Customer { get; set; }
        public Subscription? Subscription { get; set; }

        public PaymentModel Copy()
        {
            return (PaymentModel)MemberwiseClone();
        }

        public static explicit operator Payment(PaymentModel model)
        {
            return new Payment() { ID_Payment = model.ID_Payment, PaymentDate = model.PaymentDate, ValidityEndDate = model.ValidityEndDate, Amount = model.Amount, ID_Customers = model.Customer.ID_Customers, ID_Subscription = model.Subscription.ID_Subscription };
        }
    }
}
