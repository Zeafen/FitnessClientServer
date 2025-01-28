using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class LessonModel
    {
        public int ID_Lessons { get; set; }
        public string Title { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public double DurationClasses { get; set; }
        public int NumberOfPracticants { get; set; }
        public int ID_Coaches { get; set; }
        public int ID_Branches { get; set; }
    }
}
