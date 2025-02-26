using Domain.Models;
using Npgsql;

namespace DAL
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Edit(Account account)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT auth.user_edit(@id, @username, @password, @first_name, @last_name, @middle_name, @email, @profile_picture_url)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", account.Id);
                    command.Parameters.AddWithValue("@username", account.UserName);
                    command.Parameters.AddWithValue("@password", account.PasswordHash);
                    command.Parameters.AddWithValue("@first_name", (object)account.FirstName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@last_name", (object)account.LastName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@middle_name", (object)account.MiddleName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", (object)account.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@profile_picture_url", (object)account.ProfilePictureUrl ?? DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Ошибка при редактировании пользователя: ", ex);
                    }
                }
            }
        }

        public Account? GetByUserName(string userName)
        {
            Account? account = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM auth.user_get(@username) LIMIT 1";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", userName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            account = new Account();
                            account.Id = reader.GetGuid(reader.GetOrdinal("user_id"));
                            account.UserName = reader.GetString(reader.GetOrdinal("user_username"));
                            account.FirstName = reader.IsDBNull(reader.GetOrdinal("user_first_name")) ? null : reader.GetString(reader.GetOrdinal("user_first_name"));
                            account.LastName = reader.IsDBNull(reader.GetOrdinal("user_last_name")) ? null : reader.GetString(reader.GetOrdinal("user_last_name"));
                            account.MiddleName = reader.IsDBNull(reader.GetOrdinal("user_middle_name")) ? null : reader.GetString(reader.GetOrdinal("user_middle_name"));
                            account.Email = reader.IsDBNull(reader.GetOrdinal("user_email")) ? null : reader.GetString(reader.GetOrdinal("user_email"));
                            account.ProfilePictureUrl = reader.IsDBNull(reader.GetOrdinal("user_profile_picture_url")) ? null : reader.GetString(reader.GetOrdinal("user_profile_picture_url"));
                            account.PasswordHash = reader.GetString(reader.GetOrdinal("user_password"));
                            account.IsAdmin = reader.IsDBNull(reader.GetOrdinal("user_isadmin")) ? null : reader.GetBoolean(reader.GetOrdinal("user_isadmin"));
                        }
                    }
                }
            }

            return account;
        }

        public void Insert(Account account)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT auth.user_insert(@id, @username, @password, @first_name, @last_name, @middle_name, @email, @profile_picture_url)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", account.Id);
                    command.Parameters.AddWithValue("@username", account.UserName);
                    command.Parameters.AddWithValue("@password", account.PasswordHash);
                    command.Parameters.AddWithValue("@first_name", (object)account.FirstName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@last_name", (object)account.LastName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@middle_name", (object)account.MiddleName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", (object)account.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@profile_picture_url", (object)account.ProfilePictureUrl ?? DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Ошибка при добавлении пользователя: ", ex);
                    }
                }
            }
        }
    }
}
