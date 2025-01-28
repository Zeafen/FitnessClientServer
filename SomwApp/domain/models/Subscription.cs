using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class Subscription
    {
        public int ID_Subscription { get; set; }
        public string TypeofSubscription { get; set; }
        public double Cost { get; set; }
        public int NumberofVisits { get; set; }
        public int ValidityDaysNumber { get; set; } 
        public string Conditions { get; set; }
    }
}
