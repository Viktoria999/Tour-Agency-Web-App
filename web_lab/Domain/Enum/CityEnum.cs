using System.ComponentModel.DataAnnotations;

namespace Domain.Enum
{
    public enum CityEnum
    {
        [Display(Name = "Дивноморское")]
        Divnomorskoe,
        [Display(Name = "Сочи")]
        Sochi,
        [Display(Name = "Туапсе")]
        Tuapse,
        [Display(Name = "Джемете")]
        Djemete,
        [Display(Name = "Адлер")]
        Adler,
        [Display(Name = "Ейск")]
        Eisk,
        [Display(Name = "Дагомыс")]
        Dagomis,
        [Display(Name = "Сукко")]
        Sykko,
        [Display(Name = "Новоросийск")]
        Novorosisk,
        [Display(Name = "Джанхот")]
        Dganxot
    }
}
