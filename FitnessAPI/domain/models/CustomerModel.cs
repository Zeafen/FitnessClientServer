using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class CustomerModel
    {
        public int ID_Customers { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? MiddleName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string PhoneNumber { get; set; }
    }
}
