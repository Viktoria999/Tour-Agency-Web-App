using DAL.Filters;
using Domain.Models;
using Npgsql;

namespace DAL
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _connectionString;

        public CatalogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Delete(int itemId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "DELETE FROM catalog.items WHERE id = @id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", itemId);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Ошибка при удалении элемента каталога: ", ex);
                    }
                }
            }
        }

        public void Edit(CatalogItem item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT catalog.item_edit(@id, @name, @description, @url, @city, @starrating, @beachtype, @isallinclusive)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", item.Id);
                    command.Parameters.AddWithValue("@name", item.Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@description", item.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@url", item.Url ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@city", item.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@starrating", item.StarRating ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@beachtype", item.BeachType ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@isallinclusive", item.IsAllInclusive ?? (object)DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Ошибка при изменении элемента каталога: ", ex);
                    }
                }
            }
        }

        public void Insert(CatalogItem item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT catalog.item_insert(@name, @description, @url, @city, @starrating, @beachtype, @isallinclusive)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", item.Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@description", item.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@url", item.Url ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@city", item.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@starrating", item.StarRating ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@beachtype", item.BeachType ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@isallinclusive", item.IsAllInclusive ?? (object)DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Ошибка при добавлении элемента каталога: ", ex);
                    }
                }
            }
        }

        public IEnumerable<CatalogItem> Select(CatalogItemFilter filter)
        {
            var catalogItems = new List<CatalogItem>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM catalog.items_select(@id, @city, @beachtype, @starrating, @isallinclusive)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", filter.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@city", filter.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@beachtype", filter.BeachType ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@starrating", filter.StarRating ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@isallinclusive", filter.IsAllInclusive ?? (object)DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new CatalogItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("item_id")),
                                Name = reader.GetString(reader.GetOrdinal("item_name")),
                                Description = reader.GetString(reader.GetOrdinal("item_description")),
                                Url = reader.GetString(reader.GetOrdinal("item_url")),
                                City = reader.IsDBNull(reader.GetOrdinal("item_city")) ? null : reader.GetString(reader.GetOrdinal("item_city")),
                                BeachType = reader.IsDBNull(reader.GetOrdinal("item_beach_type")) ? null : reader.GetString(reader.GetOrdinal("item_beach_type")),
                                StarRating = reader.IsDBNull(reader.GetOrdinal("item_star_rating")) ? null : reader.GetInt32(reader.GetOrdinal("item_star_rating")),
                                IsAllInclusive = reader.IsDBNull(reader.GetOrdinal("item_is_all_inclusive")) ? null : reader.GetBoolean(reader.GetOrdinal("item_is_all_inclusive"))
                            };

                            catalogItems.Add(item);
                        }
                    }
                }
            }

            return catalogItems;
        }
    }
}
