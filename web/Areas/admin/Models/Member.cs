using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Areas.admin.Models
{
    public class Member
    {
        public int ID { get; set; }

        public string AdSoyad { get; set; }

        public string Mobil { get; set; }

        public string IpAddress { get; set; }

        public string MemberType { get; set; }

        public DateTime CreateDate { get; set; }
        public bool IsBlocked { get; set; }
    }
}