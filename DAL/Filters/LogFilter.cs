namespace DAL.Filters
{
    public class LogFilter
    {
        public int? Id { get; set; }
        public string LogController { get; set; }
        public string LogAction { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public string UserName { get; set; }
        public string Period { get; set; }
    }
}
