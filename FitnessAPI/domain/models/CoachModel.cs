using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class CoachModel
    {
        public int ID_Coaches { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? MiddleName { get; set; }
        public string Specialization { get; set; }
        public string PhoneNumber { get; set; }
        public double LessonsSchedule { get; set; }
        public int? ID_UserAccounts { get; set; }
    }
}
