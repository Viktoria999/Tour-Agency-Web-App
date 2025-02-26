namespace Domain.Models
{
    public class Log
    {
        public int? Id { get; set; }
        public string UserName { get; set; } = "Unauthorized User";
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime LogTime { get; set; } = DateTime.Now;
    }
}
