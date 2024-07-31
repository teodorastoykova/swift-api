using Microsoft.AspNetCore.Mvc;
using NLog;
using SwiftApi.Models;
using SwiftApi.Services;
using System.Data.SQLite;

namespace SwiftApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SwiftController : ControllerBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly SwiftMessageParser _swiftMessageParser;
        private readonly DatabaseService _databaseService;

        public SwiftController(SwiftMessageParser swiftMessageParser, DatabaseService databaseService)
        {
            _swiftMessageParser = swiftMessageParser;
            _databaseService = databaseService;
            InitializeDatabase();
        }

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream()))
                {
                    var messageContent = stream.ReadToEnd();
                    logger.Info("File content: {0}", messageContent); 
                    var parsedMessage = _swiftMessageParser.ParseSwiftMessage(messageContent);
                    var parsedMessageModel = new ParsedMessage
                    {
                        Recipients = parsedMessage["Recipients"],
                        Title = parsedMessage["Title"],
                        Description = parsedMessage["Description"]
                    };
                    _databaseService.SaveParsedMessage(parsedMessageModel);
                }

                return Ok("File processed successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error processing file.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("messages")]
        public IActionResult GetMessages()
        {
            try
            {
                var messages = _databaseService.GetAllMessages();
                return Ok(messages);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving messages from database.");
                return StatusCode(500, "Internal server error.");
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection("Data Source=swift.db"))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {

                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Messages (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Recipients TEXT NOT NULL,
                                Title TEXT NOT NULL,
                                Description TEXT NOT NULL
                            );
                        ";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error initializing database.");
                throw;
            }
        }
    }
}
