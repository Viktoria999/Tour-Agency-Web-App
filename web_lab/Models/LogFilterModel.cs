using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Web_lab.Models
{
    public class LogFilterModel
    {
        [Display(Name = "Уровень логирования:")]
        public string Level { get; set; }
        [Display(Name = "Имя пользователя:")]
        public string UserName { get; set; }
        [Display(Name = "Время от:")]
        public DateTime? TimeFrom { get; set; }
        [Display(Name = "Время до:")]
        public DateTime? TimeTo { get; set; }
        public IEnumerable<SelectListItem> LevelList { get; set; }
    }
}
