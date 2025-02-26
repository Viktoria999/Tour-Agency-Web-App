namespace Domain.Models
{
    public class LogRatingReportModel
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public int RequestCount { get; set; }
    }
}
