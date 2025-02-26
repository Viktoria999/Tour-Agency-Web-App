using System.ComponentModel.DataAnnotations;

namespace Logging
{
    public enum LoggingLevel
    {
        [Display(Name = "Успешно (Success)")]
        Success,
        [Display(Name = "Информация (Information)")]
        Information,
        [Display(Name = "Предупреждение (Warning)")]
        Warning,
        [Display(Name = "Ошибка (Error)")]
        Error
    }
}
