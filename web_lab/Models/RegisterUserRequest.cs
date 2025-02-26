using System.ComponentModel.DataAnnotations;

namespace Web_lab.Models
{
    public class RegisterUserRequest
    {
        [Required]
        [Display(Name = "Логин:")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль:")]
        public string Password { get; set; }

        [Display(Name = "Подтвердите пароль:")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Имя:")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия:")]
        public string LastName { get; set; }

        [Display(Name = "Отчество:")]
        public string MiddleName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Адрес электронной почты:")]
        public string Email { get; set; }

        public string ProfilePictureUrl { get; set; } = "/assets/profile_pictures/default.jpeg";
    }
}
