using System.ComponentModel.DataAnnotations;

namespace Web_lab.Models
{
    public class LoginUserRequest
    {
        [Display(Name = "Логин:")]
        public string UserName { get; set; }
        [Display(Name = "Пароль:")]
        public string Password { get; set; }
    }
}
