using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomwApp.domain.models
{
    public class ReviewFile
    {
        public string ReviewName { get; set; }
        public string ReviewAddress { get; set; }
        public ReviewType ReviewType { get; set; }
        public DateTime ReviewDate { get; set; }
    }

    public class ReviewFileRequest
    {
        public string ReviewName { get; set; }
        public ReviewType ReviewType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
