using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class PaymentModel
    {
        public int ID_Payment { get; set; }
        public DateOnly PaymentDate { get; set; }
        public DateOnly ValidityEndDate { get; set; }
        public double Amount { get; set; }
        public int ID_Customers { get; set; }
        public int ID_Subscription { get; set; }
    }
}
