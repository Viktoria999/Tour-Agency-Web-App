using System.ComponentModel.DataAnnotations;

namespace Domain.Enum
{
    public enum BeachTypeEnum
    {
        [Display(Name = "Песок")]
        Sand,
        [Display(Name = "Галька")]
        Rocks
    }
}
