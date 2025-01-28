using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class AppointmentsForClasses
    {
        public int ID_AppointmentsForClasses { get; set; }
        public int ID_Customers { get; set; }
        public int ID_Lessons { get; set; }
        public DateOnly Date_recording { get; set; }
        public string Status_recording { get; set; }

        public AppointmentsForClasses Copy()
        {
            return (AppointmentsForClasses)MemberwiseClone();
        }
    }
    public class AppointmentsForClassesModel
    {
        public int ID_AppointmentsForClasses { get; set; }
        public Customer Customer{ get; set; }
        public Lesson Lesson { get; set; }
        public DateOnly Date_recording { get; set; }
        public string Status_recording { get; set; }

        public AppointmentsForClassesModel Copy()
        {
            return (AppointmentsForClassesModel)MemberwiseClone();
        }

        public static explicit operator AppointmentsForClasses(AppointmentsForClassesModel model)
        {
            return new AppointmentsForClasses()
            {
                ID_AppointmentsForClasses = model.ID_AppointmentsForClasses,
                ID_Customers = model.Customer.ID_Customers,
                ID_Lessons = model.Lesson.ID_Lessons,
                Date_recording = model.Date_recording,
                Status_recording = model.Status_recording,
            };
        }
    }
}
