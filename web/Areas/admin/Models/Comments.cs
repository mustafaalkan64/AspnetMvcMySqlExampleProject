using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Areas.admin.Models
{
    public class Comments
    {
        public Comments()
        {
            this.Article = new Article();
        }
        [Key]
        public int ID { get; set; }

        [Display(Name = "Yorum")]
        [StringLength(5000, ErrorMessage = "Yorum Alanı 250 Karakterden Fazla Olamaz")]
        [Required(ErrorMessage = "Yorum Alanı Boş Bırakılamaz")]
        public string Comment { get; set; }

        [Display(Name = "Yorum Yapan")]
        [Required]
        public string CommentByUserName { get; set; }

        [Display(Name = "Yorum Yapanın IP Adresi")]
        [Required]
        public string IpAddress { get; set; }

        [Display(Name = "Oluşturma Tarihi")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Onaylanma")]
        public bool IsAccepted { get; set; }

        [Display(Name = "Makale")]
        [Required(ErrorMessage = "Kategori Seçilmelidir")]
        public int ArticleId { get; set; }

        public Article Article { get; set; }

        [Display(Name = "Makale")]
        public string Caption { get; set; }

        [Display(Name = "İçerik")]
        public string ArticleContent { get; set; }
    }
}