using System.Collections.Generic;
using System.Data.SQLite;
using SwiftApi.Models;

namespace SwiftApi.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Data Source=swift.db";

        public void SaveParsedMessage(ParsedMessage message)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Messages (Recipients, Title, Description)
                        VALUES (@Recipients, @Title, @Description)";
                    command.Parameters.AddWithValue("@Recipients", message.Recipients);
                    command.Parameters.AddWithValue("@Title", message.Title);
                    command.Parameters.AddWithValue("@Description", message.Description);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<ParsedMessage> GetAllMessages()
        {
            var messages = new List<ParsedMessage>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Messages";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new ParsedMessage
                            {
                                Recipients = reader["Recipients"].ToString(),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString()
                            };
                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }
    }
}
