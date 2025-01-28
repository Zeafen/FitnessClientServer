using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class Lesson
    {
        public int ID_Lessons { get; set; }
        public string Title { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public double DurationClasses { get; set; }
        public int NumberOfPracticants { get; set; }
        public int ID_Coaches { get; set; }
        public int ID_Branches { get; set; }

        public Lesson Copy()
        {
            return (Lesson)MemberwiseClone();
        }
    }
    public class LessonsModel
    {
        public int ID_Lessons { get; set; }
        public string Title { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public double Hours { get; set; }
        public int NumberOfPracticants { get; set; }
        public Coach Coach { get; set; }
        public Branches Branch { get; set; }

        public LessonsModel Copy()
        {
            return (LessonsModel)MemberwiseClone();
        }

        public static explicit operator Lesson(LessonsModel model)
        {
            return new Lesson()
            {
                ID_Lessons = model.ID_Lessons,
                Title = model.Title,
                Date = model.Date,
                Time = model.Time,
                NumberOfPracticants = model.NumberOfPracticants,
                DurationClasses = model.Hours,
                ID_Coaches = model.Coach.ID_Coaches,
                ID_Branches = model.Branch.ID_Branches,
            };
        }
    }
}
