using DAL.Filters;
using Domain.Models;

namespace DAL
{
    public interface ILogRepository
    {
        void Insert(Log log);
        IEnumerable<Log> Select(LogFilter filter);
        IEnumerable<LogRatingReportModel> GetPopularPagesRating();
    }
}
