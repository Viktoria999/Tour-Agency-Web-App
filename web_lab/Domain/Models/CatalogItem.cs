namespace Domain.Models
{
    public class CatalogItem
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string? City { get; set; }
        public int? StarRating { get; set; }
        public bool? IsAllInclusive { get; set; }
        public string? BeachType { get; set; }
    }
}
