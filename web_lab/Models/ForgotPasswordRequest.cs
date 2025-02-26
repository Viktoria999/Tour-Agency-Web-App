using System.ComponentModel.DataAnnotations;

namespace Web_lab.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [Display(Name = "Логин:")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Адрес электронной почты:")]
        public string Email { get; set; }
    }
}
