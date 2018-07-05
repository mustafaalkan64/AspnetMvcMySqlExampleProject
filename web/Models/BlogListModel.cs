using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using web.Areas.admin.Models;

namespace web.Models
{
    public class BlogModel
    {
        public BlogModel()
        {
            this.ArticleList = new List<Article>();
            this.PageList = new List<PageModel>();
        }
        public List<Article> ArticleList { get; set; }
        public int PageCount { get; set; }
        public int ActivePage { get; set; }
        public int TotalCount { get; set; }
        public string FirstPageUrl { get; set; }
        public string LastPageUrl { get; set; }
        public List<PageModel> PageList { get; set; }
    }

    public class PageModel
    {
        public string Url { get; set; }
        public int DisplayName { get; set; }
    }
}