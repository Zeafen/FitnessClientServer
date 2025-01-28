using Newtonsoft.Json;
using SomwApp.presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class Customer
    {
        public int ID_Customers { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? MiddleName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string FullName
        {
            get
            {
                return $"{Surname} {Name}{(MiddleName == null ? String.Empty : $" {MiddleName}")}";
            }
        }
        public Customer Copy()
        {
            return (Customer)MemberwiseClone();
        }
    }

    public class CustomerModel
    {
        public int ID_Customer { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? MiddleName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public string FullName
        {
            get
            {
                return $"{Surname} {Name}{(MiddleName == null ? String.Empty : $" {MiddleName}")}";
            }
        }

        public CustomerModel Copy()
        {
            return (CustomerModel)MemberwiseClone();
        }

        public CustomerModel()
        {
            Surname = String.Empty;
            Name = String.Empty;
            MiddleName = String.Empty;
            BirthDate = DateOnly.FromDateTime(DateTime.Now);
            PhoneNumber = String.Empty;
        }

        public static explicit operator Customer(CustomerModel model)
        {
            return new Customer()
            {
                ID_Customers = model.ID_Customer,
                Surname = model.Surname,
                Name = model.Name,
                MiddleName = model.MiddleName,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber,
            };
        }
    }
}
