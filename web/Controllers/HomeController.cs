using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using web.Areas.admin.Models;
using web.Models;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string myConnectionString = "Server=localhost;Port=3306;Database=webdb;Uid=root;Pwd=Malkan06*-fb-;";
        public ActionResult Index()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(myConnectionString))
                {
                    connection.Open();
                    var ip = GetUserIP();
                    using (MySqlCommand cmd = new MySqlCommand("select AdSoyad from webdb.member where IpAddress='" + ip + "' LIMIT 1;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            var nameSurname = "";
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    nameSurname = reader.GetString("AdSoyad");
                                    ViewBag.NameSurname = nameSurname;
                                    ViewBag.HasUser = true;
                                    return View();
                                }
                            }
                        }
                        ViewBag.HasUser = false;
                        return View();
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        [OutputCache(Duration = 60)]
        public ActionResult Blog(int CategoryId = 0, int Month = 0, int Page = 0)
        {
            try
            {
                var pagesize = 5;
                var limit = pagesize;
                var offset = Page * pagesize;
                var listArticle = new List<Article>();
                var pagecount = 0;
                //Creating instance of DatabaseContext class  
                using (var connection = new MySqlConnection(myConnectionString))
                {
                    connection.Open();
                    string sql = String.Empty;
                    if(CategoryId == 0 && Month == 0)
                    {
                        sql = "select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl, comment.commentcount as _count from webdb.article as a " +
                        " join articleımages as aı on a.Id = aı.ArticleId " +
                        " left join (select count(ID) as commentcount, articleId from webdb.comments group by articleId) as comment on a.ID = comment.articleId " +
                        " join category as c on a.CategoryId = c.ID order by a.ID desc ";

                    }

                    else if(CategoryId > 0)
                    {
                        sql = "select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl, comment.commentcount as _count from webdb.article as a " +
                         " join articleımages as aı on a.Id = aı.ArticleId " +
                         " join category as c on a.CategoryId = c.ID " +
                         " left join (select count(ID) as commentcount, articleId from webdb.comments group by articleId) as comment on a.ID = comment.articleId " +
                         " where c.ID = @categoryId order by a.ID desc ";
                    }
                    else if (Month > 0)
                    {
                        sql = "select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl, comment.commentcount as _count from webdb.article as a " +
                         " join articleımages as aı on a.Id = aı.ArticleId " +
                         " join category as c on a.CategoryId = c.ID " +
                         " left join (select count(ID) as commentcount, articleId from webdb.comments group by articleId) as comment on a.ID = comment.articleId " +
                         " where MONTH(a.CreateDate) = @month order by a.ID desc ";
                    }

                    var articlelistsql = sql + " limit " + limit + " offset " + offset;
                    var pagingsql = "select count(*) as pagingcount from (" + sql + ") as result";
                    using (MySqlCommand cmd = new MySqlCommand(articlelistsql, connection))
                    {
                        cmd.Parameters.AddWithValue("?categoryId", CategoryId);
                        cmd.Parameters.AddWithValue("?month", Month);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    var article = new Article();
                                    article.ID = Convert.ToInt32(reader["ID"]);
                                    article._count = Convert.ToInt32(reader["_count"] == DBNull.Value ? 0 : reader["_count"]);
                                    article.ArticleContent = reader["ArticleContent"].ToString();
                                    article.Caption = reader["Caption"].ToString();
                                    article.CategoryName = reader["CategoryName"].ToString();
                                    article.ArticleImage = reader["imageurl"].ToString();
                                    article.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
                                    article.UpdateDate = Convert.ToDateTime(reader["UpdateDate"] == DBNull.Value ? DateTime.MinValue : reader["UpdateDate"]);
                                    listArticle.Add(article);
                                }                             
                            }
                        }
                        cmd.Dispose();
                    }

                    using (MySqlCommand cmd = new MySqlCommand(pagingsql, connection))
                    {
                        cmd.Parameters.AddWithValue("?categoryId", CategoryId);
                        cmd.Parameters.AddWithValue("?month", Month);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    pagecount = Convert.ToInt32(reader["pagingcount"]);
                                }
                            }
                        }
                        cmd.Dispose();
                    }
                    connection.Close();
                    connection.Dispose();
                }
                var blogModel = new BlogModel();
                var pageList = new List<PageModel>();
                blogModel.ArticleList = listArticle;
                blogModel.ActivePage = Page + 1;
                blogModel.PageCount = (pagecount / pagesize) + (pagecount % pagesize > 0 ? 1 : 0);

                for(int i = 0; i < blogModel.PageCount; i++)
                {
                    var pagemodel = new PageModel();
                    pagemodel.Url = "/Home/Blog?CategoryId=" + CategoryId + "&Month=" + Month + "&Page=" + i;
                    pagemodel.DisplayName = i + 1;
                    pageList.Add(pagemodel);
                }
                if(pageList.Count > 0)
                    blogModel.FirstPageUrl = pageList.First().Url;
                else
                    blogModel.FirstPageUrl = "/Home/Blog?CategoryId=" + CategoryId + "&Month=" + Month + "&Page=" + 0;

                if (pageList.Count > 0)
                    blogModel.LastPageUrl = pageList.Last().Url;
                else
                    blogModel.FirstPageUrl = "/Home/Blog?CategoryId=" + CategoryId + "&Month=" + Month + "&Page=" + 0;

                blogModel.PageList = pageList;
                return View(blogModel);
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public ActionResult BlogPager(int CategoryId = 0, int Month = 0)
        {
            try
            {
                var listArticle = new List<Article>();
                //Creating instance of DatabaseContext class  
                using (var connection = new MySqlConnection(myConnectionString))
                {
                    connection.Open();
                    string sql = "select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl, comment.commentcount as _count from webdb.article as a " +
                        " join articleımages as aı on a.Id = aı.ArticleId " +
                        " left join (select count(ID) as commentcount, articleId from webdb.comments group by articleId) as comment on a.ID = comment.articleId " +
                        " join category as c on a.CategoryId = c.ID order by ID desc ";

                    if (CategoryId > 0)
                    {
                        sql = "select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl, comment.commentcount as _count from webdb.article as a " +
                         " join articleımages as aı on a.Id = aı.ArticleId " +
                         " join category as c on a.CategoryId = c.ID " +
                         " left join (select count(ID) as commentcount, articleId from webdb.comments group by articleId) as comment on a.ID = comment.articleId " +
                         " where c.ID = @categoryId order by ID desc ";
                    }
                    if (Month > 0)
                    {
                        sql = "select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl, comment.commentcount as _count from webdb.article as a " +
                         " join articleımages as aı on a.Id = aı.ArticleId " +
                         " join category as c on a.CategoryId = c.ID " +
                         " left join (select count(ID) as commentcount, articleId from webdb.comments group by articleId) as comment on a.ID = comment.articleId " +
                         " where MONTH(a.CreateDate) = @month order by ID desc ";
                    }
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("?categoryId", CategoryId);
                        cmd.Parameters.AddWithValue("?month", Month);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    var article = new Article();
                                    article.ID = Convert.ToInt32(reader["ID"]);
                                    article._count = Convert.ToInt32(reader["_count"] == DBNull.Value ? 0 : reader["_count"]);
                                    article.ArticleContent = reader["ArticleContent"].ToString();
                                    article.Caption = reader["Caption"].ToString();
                                    article.CategoryName = reader["CategoryName"].ToString();
                                    article.ArticleImage = reader["imageurl"].ToString();
                                    article.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
                                    article.UpdateDate = Convert.ToDateTime(reader["UpdateDate"] == DBNull.Value ? DateTime.MinValue : reader["UpdateDate"]);
                                    listArticle.Add(article);
                                }
                            }
                        }
                        cmd.Dispose();
                    }
                    connection.Close();
                }
                return View(listArticle);
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public ActionResult BlogDetail(int id = 0)
        {
            try
            {
                if (id == 0)
                    return View(new Article());
                //Creating instance of DatabaseContext class  
                using (var connection = new MySqlConnection(myConnectionString))
                {
                    connection.Open();
                    string sql = "select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl from webdb.article as a " +
                        " join articleımages as aı on a.Id = aı.ArticleId " +
                        " join category as c on a.CategoryId = c.ID where a.ID = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("?id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    var article = new Article();
                                    article.ID = Convert.ToInt32(reader["ID"]);
                                    article.ArticleContent = reader["ArticleContent"].ToString();
                                    article.Caption = reader["Caption"].ToString();
                                    article.CategoryName = reader["CategoryName"].ToString();
                                    article.ArticleImage = reader["imageurl"].ToString();
                                    article.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
                                    article.UpdateDate = Convert.ToDateTime(reader["UpdateDate"] == DBNull.Value ? DateTime.MinValue : reader["UpdateDate"]);
                                    return View(article);
                                }
                            }
                        }
                        cmd.Dispose();
                    }
                    connection.Close();
                    connection.Dispose();
                }
                return View(new Article());
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public ActionResult BlogCategories()
        {
            using (MySqlConnection connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select Name, ID from webdb.category order by Name asc", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var categoryList = new List<CategoryModel>();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                var categoryModel = new CategoryModel();
                                categoryModel.Name = reader.GetString("Name");
                                categoryModel.ID = Convert.ToInt32(reader.GetString("ID"));
                                categoryModel.URL = "/Home/Blog?CategoryId=" + categoryModel.ID;
                                categoryList.Add(categoryModel);
                                
                            }
                        }
                        connection.Close();
                        connection.Dispose();
                        return View(categoryList);
                    }
                }
            }
        }

        public ActionResult BlogArchiev()
        {
            using (MySqlConnection connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) as Count, MONTH(CreateDate) as MonthID, year(CreateDate) as Year FROM webdb.article" +
                    " GROUP BY MONTH(CreateDate), YEAR(CreateDate) " +
                    " ORDER BY YEAR(CreateDate) DESC, MONTH(CreateDate) DESC", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var categoryList = new List<ArticleGroupByMonth>();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                var categoryModel = new ArticleGroupByMonth();
                                categoryModel.Count = Convert.ToInt32(reader.GetString("Count"));
                                categoryModel.Month = Convert.ToInt32(reader.GetString("MonthID"));
                                categoryModel.Year = Convert.ToInt32(reader.GetString("Year"));

                                switch (categoryModel.Month)
                                {
                                    case 1:
                                        categoryModel.MonthName = "Ocak";
                                        break;
                                    case 2:
                                        categoryModel.MonthName = "Şubat";
                                        break;
                                    case 3:
                                        categoryModel.MonthName = "Mart";
                                        break;
                                    case 4:
                                        categoryModel.MonthName = "Nisan";
                                        break;
                                    case 5:
                                        categoryModel.MonthName = "Mayıs";
                                        break;
                                    case 6:
                                        categoryModel.MonthName = "Haziran";
                                        break;
                                    case 7:
                                        categoryModel.MonthName = "Temmuz";
                                        break;
                                    case 8:
                                        categoryModel.MonthName = "Ağustos";
                                        break;
                                    case 9:
                                        categoryModel.MonthName = "Eylül";
                                        break;
                                    case 10:
                                        categoryModel.MonthName = "Ekim";
                                        break;
                                    case 11:
                                        categoryModel.MonthName = "Kasım";
                                        break;
                                    case 12:
                                        categoryModel.MonthName = "Aralık";
                                        break;

                                }
                                categoryModel.DisplayName = categoryModel.MonthName + "-" + categoryModel.Year + " (" + categoryModel.Count + ")";
                                categoryModel.Url = "/Home/Blog?Month=" + categoryModel.Month;
                                categoryList.Add(categoryModel);
                            }
                        }
                        connection.Close();
                        connection.Dispose();
                        return View(categoryList);
                    }
                }
            }
        }

        public ActionResult Articles()
        {
            var article = new Article();
            using (MySqlConnection connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) as Count, MONTH(CreateDate) as MonthID FROM webdb.article " +
                    " WHERE YEAR(CreateDate) = YEAR(CURDATE()) " +
                    " GROUP BY MONTH(CreateDate)", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var categoryList = new List<ArticleGroupByMonth>();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                var categoryModel = new ArticleGroupByMonth();
                                categoryModel.Count = Convert.ToInt32(reader.GetString("Count"));
                                categoryModel.Month = Convert.ToInt32(reader.GetString("MonthID"));
                                switch (categoryModel.Month)
                                {
                                    case 1:
                                        categoryModel.MonthName = "Ocak";
                                        break;
                                    case 2:
                                        categoryModel.MonthName = "Şubat";
                                        break;
                                    case 3:
                                        categoryModel.MonthName = "Mart";
                                        break;
                                    case 4:
                                        categoryModel.MonthName = "Nisan";
                                        break;
                                    case 5:
                                        categoryModel.MonthName = "Mayıs";
                                        break;
                                    case 6:
                                        categoryModel.MonthName = "Haziran";
                                        break;
                                    case 7:
                                        categoryModel.MonthName = "Temmuz";
                                        break;
                                    case 8:
                                        categoryModel.MonthName = "Ağustos";
                                        break;
                                    case 9:
                                        categoryModel.MonthName = "Eylül";
                                        break;
                                    case 10:
                                        categoryModel.MonthName = "Ekim";
                                        break;
                                    case 11:
                                        categoryModel.MonthName = "Kasım";
                                        break;
                                    case 12:
                                        categoryModel.MonthName = "Aralık";
                                        break;

                                }
                                categoryModel.DisplayName = categoryModel.MonthName + "(" + categoryModel.Count + ")";
                                categoryModel.Url = "/Home/Blog?Month" + categoryModel.Month;
                                categoryList.Add(categoryModel);

                            }
                        }
                        connection.Close();
                        connection.Dispose();
                        return View(categoryList);
                    }
                }
            }
        }

        public ActionResult RecentComments()
        {
            using (MySqlConnection connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT commentbyusername, comment, createdate FROM webdb.comments where isaccepted = 1 order by ID desc  LIMIT 3 ;", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var commentList = new List<Comments>();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                var categoryModel = new Comments();
                                categoryModel.CommentByUserName = reader.GetString("comment");
                                categoryModel.Comment = reader.GetString("commentbyusername");
                                categoryModel.CreateDate = Convert.ToDateTime(reader["createdate"] == DBNull.Value ? DateTime.MinValue : reader["createdate"]);
                                commentList.Add(categoryModel);

                            }
                        }
                        connection.Close();
                        connection.Dispose();
                        return View(commentList);
                    }
                }
            }
        }

        public ActionResult ArticleList()
        {
            var listArticle = new List<Article>();
            //Creating instance of DatabaseContext class  
            using (var connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select a.*, c.Name as CategoryName, aı.ArticleImageUrl as imageurl from webdb.article as a " +
                    " join articleımages as aı on a.Id = aı.ArticleId " +
                    " join category as c on a.CategoryId = c.ID order by ID desc ", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                var article = new Article();
                                article.ID = Convert.ToInt32(reader["ID"]);
                                article.ArticleContent = reader["ArticleContent"].ToString();
                                article.Caption = reader["Caption"].ToString();
                                article.CategoryName = reader["CategoryName"].ToString();
                                article.ArticleImage = reader["imageurl"].ToString();
                                article.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
                                article.UpdateDate = Convert.ToDateTime(reader["UpdateDate"] == DBNull.Value ? DateTime.MinValue : reader["UpdateDate"]);
                                listArticle.Add(article);
                            }
                        }
                    }
                    cmd.Dispose();
                }
                connection.Close();
            }
            return View(listArticle);
        }

        public ActionResult Comments(int articleId = 0)
        {
            var commentList = new List<Comments>();
            //Creating instance of DatabaseContext class  
            using (var connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select * from webdb.comments where articleId = @articleId and isaccepted = 1 order by ID desc ", connection))
                {
                    cmd.Parameters.AddWithValue("?articleId", articleId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                var comment = new Comments();
                                comment.ID = Convert.ToInt32(reader["ID"]);
                                comment.Comment = reader["comment"].ToString();
                                comment.CommentByUserName = reader["commentbyusername"].ToString();
                                comment.CreateDate = Convert.ToDateTime(reader["createdate"] == DBNull.Value ? DateTime.MinValue : reader["createdate"]);
                                commentList.Add(comment);
                            }
                        }
                    }
                    cmd.Dispose();
                }
                connection.Close();
                connection.Dispose();
            }
            return View(commentList);
        }

        public ActionResult PostComment(int articleId = 0)
        {
            var ip = GetUserIP();
            var comment = new PostCommentModel();
            comment.ArticleId = articleId;
            //Creating instance of DatabaseContext class  
            using (var connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select * from webdb.member where IpAddress = @ip ", connection))
                {
                    cmd.Parameters.AddWithValue("?ip", ip);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                comment.UserName = reader["AdSoyad"].ToString();
                                comment.Phone = reader["Mobil"].ToString();
                            }
                            ViewBag.HasUser = true;
                        }
                        else
                        {
                            ViewBag.HasUser = false;
                        }
                    }
                    cmd.Dispose();
                }
                connection.Close();
                connection.Dispose();
            }
            return View(comment);
        }

        [HttpPost]
        public ActionResult PostComment(PostCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage));

                ModelState.AddModelError("", message);
                return View(model);
            }

            model.Email.ToUpper().Replace("1=1", "");
            model.Comment.ToUpper().Replace("1=1", "");

            try
            {
                var ip = GetUserIP();
                using (var conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();
                    using (var comm = conn.CreateCommand())
                    {
                        comm.CommandText = "insert into webdb.comments(comment, commentbyusername, isaccepted, createdate, articleId, IpAddress, email) values(@comment, @commentbyusername, @isaccepted, @createdate, @articleId, @IpAddress, @email)";
                        comm.Parameters.AddWithValue("?comment", model.Comment);
                        comm.Parameters.AddWithValue("?isaccepted", false);
                        comm.Parameters.AddWithValue("?createdate", DateTime.Now);
                        comm.Parameters.AddWithValue("?articleId", model.ArticleId);
                        comm.Parameters.AddWithValue("?IpAddress", ip);
                        comm.Parameters.AddWithValue("?email", model.Email);
                        comm.Parameters.AddWithValue("?commentbyusername", model.UserName);
                        comm.ExecuteNonQuery();

                        ViewBag.SuccessCreateComment = true;
                        conn.Close();
                        conn.Dispose();
                        //return RedirectToAction("Index");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult RegisterUser()
        {
                return View(new RegisterUserModel());
        }

        [HttpPost]
        public ActionResult RegisterUser(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage));

                ModelState.AddModelError("", message);
                return View(model);
            }

            model.NameSurname.ToUpper().Replace("1=1", "");
            model.Phone.ToUpper().Replace("1=1", "");
            

            
            try
            {
                var ip = GetUserIP();
                using (var conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();
                    using (var comm = conn.CreateCommand())
                    {
                        comm.CommandText = "insert into webdb.member(Mobil, AdSoyad, IpAddress) values(@phone, @adsoyad, @ipaddress)";
                        comm.Parameters.AddWithValue("?phone", model.Phone);
                        comm.Parameters.AddWithValue("?adsoyad", model.NameSurname);
                        comm.Parameters.AddWithValue("?ipaddress", ip);
                        comm.ExecuteNonQuery();
                        conn.Close();
                        ViewBag.SuccessRegisterUser = true;
                        conn.Close();
                        conn.Dispose();
                        //return RedirectToAction("Index");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private string GetUserIP()
        {
            //string ipaddress;
            //ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (ipaddress == "" || ipaddress == null)
            //    ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            //return ipaddress;
            string strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            string ip = addr[1].ToString();
            return ip;
        }
    }
}