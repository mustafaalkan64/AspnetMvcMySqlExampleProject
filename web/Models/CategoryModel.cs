using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class CategoryModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }

    public class ArticleGroupByMonth
    {
        public int Count { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public string Url { get; set; }
        public string DisplayName { get; set; }
    }
}