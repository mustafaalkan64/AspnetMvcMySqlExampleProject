using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Areas.admin.Models
{
    public class Article
    {
        public Article()
        {
            this.Categories = new List<Category>();
        }
        [Key]
        public int ID { get; set; }

        [AllowHtml]
        [Display(Name = "İçerik")]
        [StringLength(5000, ErrorMessage = "İçerik Alanı 5000 Karakterden Fazla Olamaz")]
        [Required(ErrorMessage = "İçerik Alanı Boş Bırakılamaz")]
        public string ArticleContent { get; set; }
        [Display(Name = "Oluşturma Tarihi")]
        public DateTime CreateDate { get; set; }
        [StringLength(250, ErrorMessage = "Başlık Alanı 250 Karakterden Fazla Olamaz")]
        [Required(ErrorMessage = "Başlık Alanı Boş Bırakılamaz")]
        [Display(Name = "Başlık")]
        public string Caption { get; set; }
        [Display(Name = "Makale Görseli")]
        public string ArticleImage { get; set; }
        [Display(Name = "Son Güncelleme Tarihi")]
        public DateTime UpdateDate { get; set; }
        [Display(Name = "Kategori")]
        [Required(ErrorMessage = "Kategori Seçilmelidir")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Category> Categories { get; set; }
    }
}