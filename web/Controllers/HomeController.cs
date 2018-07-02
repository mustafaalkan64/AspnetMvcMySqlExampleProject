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
                    using (MySqlCommand cmd = new MySqlCommand("select AdSoyad from webdb.member where IpAddress='"+ip+"' LIMIT 1;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            var nameSurname = "";
                            if(reader.HasRows == true)
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

        public ActionResult Blog()
        {
            try
            {
                return View();
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
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