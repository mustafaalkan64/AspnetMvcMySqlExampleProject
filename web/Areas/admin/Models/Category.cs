using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Areas.admin.Models
{
    public class Category
    {
        public Category()
        {
            this.Articles = new List<Article>();
        }
        [Key]
        public int ID { get; set; }

        [Display(Name = "Kategori")]
        [StringLength(100, ErrorMessage = "Kategori Alanı 100 Karakterden Fazla Olamaz")]
        [Required(ErrorMessage = "Kategori Alanı Boş Bırakılamaz")]
        public string Name { get; set; }
        public List<Article> Articles { get; set; }
        [Display(Name = "Oluşturma Tarihi")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Son Güncelleme Tarihi")]
        public DateTime UpdateDate { get; set; }
    }
}