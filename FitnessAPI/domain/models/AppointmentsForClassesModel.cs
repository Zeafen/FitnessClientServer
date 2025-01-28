using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class AppointmentsForClassesModel
    {
        public int ID_AppointmentsForClasses { get; set; }
        public int ID_Customers { get; set; }
        public int ID_Lessons { get; set; }
        public DateOnly Date_recording { get; set; }
        public string Status_recording { get; set; }
    }
}
