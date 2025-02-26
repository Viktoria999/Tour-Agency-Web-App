namespace DAL.Filters
{
    public class CatalogItemFilter
    {
        public int? Id { get; set; }
        public string City { get; set; }
        public int? StarRating { get; set; }
        public bool? IsAllInclusive { get; set; }
        public string BeachType { get; set; }
    }
}
