using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web_lab.Models
{
    public class CatalogItemEditGetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public int? StarRating { get; set; }
        public bool IsAllInclusive { get; set; }
        public string BeachType { get; set; }
        public IEnumerable<SelectListItem> CitiesList { get; set; }
        public IEnumerable<SelectListItem> BeachTypesList { get; set; }
    }
}
