using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class RegisterUserModel
    {
        [StringLength(100, ErrorMessage = "Ad Soyad Alanı 100 Karakteri Geçemez")]
        [Required(ErrorMessage = "İsim Soyisim Boş Geçilemez")]
        [Display(Name = "İsim Soyisim")]
        public string NameSurname { get; set; }

        [Required(ErrorMessage = "Telefon Numarası Zorunlu")]
        [Display(Name = "Telefon Numarası")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Lütfen Geçerli Bir Telefon Numarası Format Giriniz")]
        public string Phone { get; set; }

        public string MemberType { get; set; }
    }

    public class PostCommentModel
    {
        public int ArticleId { get; set; }
        [Display(Name = "Ad Soyad")]
        public string UserName { get; set; }
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email Formatı Yanlış")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yorum Zorunludur")]
        [Display(Name = "Yorum")]
        [StringLength(250, ErrorMessage = "Yorum 500 Karakteri Geçemez")]
        public string Comment { get; set; }

        public bool IsBlocked { get; set; }
    }
}