using System;
using System.Collections.Generic;
using NLog;

namespace SwiftApi.Services
{
    public class SwiftMessageParser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Dictionary<string, string> ParseSwiftMessage(string messageContent)
        {
            var parsedMessage = new Dictionary<string, string>();
            try
            {
                messageContent = messageContent.Replace("\r\n", " ").Replace("\n", " ");

                var recipientsStartIndex = messageContent.IndexOf("NA VNIMANIETO NA:") + "NA VNIMANIETO NA:".Length;
                var recipientsEndIndex = messageContent.IndexOf("OTNOSNO:");
                if (recipientsStartIndex != -1 && recipientsEndIndex != -1)
                {
                    var recipients = messageContent.Substring(recipientsStartIndex, recipientsEndIndex - recipientsStartIndex).Trim();
                    parsedMessage["Recipients"] = recipients;
                }

                var titleStartIndex = messageContent.IndexOf("OTNOSNO:") + "OTNOSNO:".Length;
                var titleEndIndex = messageContent.IndexOf("UVAJAEMI KOLEGI,");
                if (titleStartIndex != -1 && titleEndIndex != -1)
                {
                    var title = messageContent.Substring(titleStartIndex, titleEndIndex - titleStartIndex).Trim();
                    parsedMessage["Title"] = title;
                }

                var descriptionStartIndex = messageContent.IndexOf("UVAJAEMI KOLEGI,");
                if (descriptionStartIndex != -1)
                {
                    var description = messageContent.Substring(descriptionStartIndex).Trim();
                    parsedMessage["Description"] = description;
                }

                logger.Info("Parsed message: {0}", parsedMessage); // Log the parsed message
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error parsing Swift message.");
                throw;
            }

            return parsedMessage;
        }
    }
}
