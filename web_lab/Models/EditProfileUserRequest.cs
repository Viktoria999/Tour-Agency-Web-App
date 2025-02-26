using System.ComponentModel.DataAnnotations;

namespace Web_lab.Models
{
    public class EditProfileUserRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Логин:")]
        public string UserName { get; set; }
        [Display(Name = "Имя:")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия:")]
        public string LastName { get; set; }
        [Display(Name = "Отчество:")]
        public string MiddleName { get; set; }
        [Display(Name = "Адрес электронной почты:")]
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
