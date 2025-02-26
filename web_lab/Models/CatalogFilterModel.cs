using Microsoft.AspNetCore.Mvc.Rendering;

namespace web_lab.Models
{
    public class CatalogFilterModel
    {
        public string City { get; set; }
        public int? StarRating { get; set; }
        public bool IsAllInclusive { get; set; }
        public string BeachType { get; set; }
        public IEnumerable<SelectListItem> CitiesList { get; set; }
        public IEnumerable<SelectListItem> BeachTypesList { get; set; }
        public bool IsClearFilter { get; set; }
    }
}
