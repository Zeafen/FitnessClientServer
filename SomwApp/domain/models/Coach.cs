using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class Coach
    {
        public int ID_Coaches { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? MiddleName { get; set; }
        public string Specialization { get; set; }
        public string PhoneNumber { get; set; }
        public double LessonsSchedule { get; set; }
        public int? ID_UserAccounts { get; set; }

        [JsonIgnore]
        public string FullName
        {
            get => $"{Surname} {Name}{(MiddleName == null ? String.Empty : $" {MiddleName}")}";
        }

        public Coach Copy()
        {
            return (Coach)MemberwiseClone();
        }
    }

    public class CoachModel
    {
        public int ID_Coaches { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? MiddleName { get; set; }
        public string Specialization { get; set; }
        public string PhoneNumber { get; set; }
        public double LessonsSchedule { get; set; }
        public UserAccounts? UserAccounts { get; set; }
        public string FullName
        {
            get => $"{Surname} {Name}{(MiddleName == null ? String.Empty : $" {MiddleName}")}";
        }

        public CoachModel Copy()
        {
            return (CoachModel)MemberwiseClone();
        }

        public static explicit operator Coach(CoachModel model)
        {
            return new Coach()
            {
                ID_Coaches = model.ID_Coaches,
                Surname = model.Surname,
                Name = model.Name,
                MiddleName = model.MiddleName,
                Specialization = model.Specialization,
                ID_UserAccounts = model.UserAccounts?.ID_UserAccounts,
                LessonsSchedule = model.LessonsSchedule,
                PhoneNumber = model.PhoneNumber
            };
        }
    }
}
