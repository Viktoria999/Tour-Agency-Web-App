using DAL.Filters;
using Domain.Models;
using Npgsql;

namespace DAL
{
    public class LogRepository : ILogRepository
    {
        private readonly string _connectionString;

        public LogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<LogRatingReportModel> GetPopularPagesRating()
        {
            var report = new List<LogRatingReportModel>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM log.logs_get_popular_pages_rating()";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new LogRatingReportModel
                            {
                                Controller = reader.GetString(reader.GetOrdinal("log_controller")),
                                Action = reader.GetString(reader.GetOrdinal("log_action")),
                                RequestCount = reader.GetInt32(reader.GetOrdinal("log_request_count"))
                            };

                            report.Add(item);
                        }
                    }
                }
            }

            return report;
        }

        public void Insert(Log log)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "INSERT INTO log.logs (username, controller, action, message, level, log_time) VALUES(@username, @controller, @action, @message, @level, @log_time)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", log.UserName);
                    command.Parameters.AddWithValue("@controller", log.Controller);
                    command.Parameters.AddWithValue("@action", log.Action);
                    command.Parameters.AddWithValue("@message", log.Message);
                    command.Parameters.AddWithValue("@level", log.Level);
                    command.Parameters.AddWithValue("@log_time", log.LogTime);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Ошибка при добавлении лога: ", ex);
                    }
                }
            }
        }

        public IEnumerable<Log> Select(LogFilter filter)
        {
            var logs = new List<Log>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM log.logs_select(@id, @controller, @action, @message, @level, @time_from, @time_to, @username, @period)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", filter.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@controller", filter.LogController ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@action", filter.LogAction ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@message", filter.Message ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@level", filter.Level ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@time_from", filter.TimeFrom ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@time_to", filter.TimeTo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@username", filter.UserName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@period", filter.Period ?? (object)DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new Log
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("log_id")),
                                Controller = reader.GetString(reader.GetOrdinal("log_controller")),
                                Action = reader.GetString(reader.GetOrdinal("log_action")),
                                Message = reader.GetString(reader.GetOrdinal("log_message")),
                                Level = reader.GetString(reader.GetOrdinal("log_level")),
                                LogTime = reader.GetDateTime(reader.GetOrdinal("log_log_time")),
                                UserName = reader.GetString(reader.GetOrdinal("log_username"))
                            };

                            logs.Add(item);
                        }
                    }
                }
            }

            return logs;
        }
    }
}
