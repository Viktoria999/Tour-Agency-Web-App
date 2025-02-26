using System.ComponentModel.DataAnnotations;

namespace Web_lab.Models
{
    public class RestorePasswordModel
    {
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Придумайте новый пароль:")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Подтвердите пароль:")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
