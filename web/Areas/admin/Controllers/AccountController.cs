using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Areas.admin.Models;
using System.Web.Security;
using System.Configuration;

namespace web.Areas.admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly string myConnectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        // GET: admin/Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }


        [Authorize]
        public ActionResult Home()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (MySqlConnection connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select * from webdb.users where email=@email and password = @password AND usertype = 'admin' LIMIT 1;", connection))
                {
                    cmd.Parameters.AddWithValue("?email", model.Email);
                    cmd.Parameters.AddWithValue("?password", model.Password);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                FormsAuthentication.SetAuthCookie("admin", model.RememberMe);
                                return RedirectToAction("Home");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Kullanıcı Adı ve Şifre Hatalıdır");
                            return View(model);
                        }
                    }
                    return null;
                }
            }
        }
    }
}