using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Areas.admin.Models
{
    public class UserAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Request.Cookies["webadmin"] != null)
            {
                //httpContext.Response.Redirect("/admin/Account/Home");
                return true;
            }
            else
            {
                httpContext.Response.Redirect("/admin/Account/Login");
                return false;
            }

        }
    }
}