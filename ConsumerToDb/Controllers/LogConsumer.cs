namespace ConsumerToDb.Controllers
{
    using System;

    using ConsumerToDb.Model.Database;
    using JsonHelper.Model;
    using Newtonsoft.Json;
    using QueueDatabase.Model;
    using QueueDatabase.Model.Rabbit;
    using RabbitMQ.Client;

    /// <summary>
    /// This class consumes log entries from a message queue broker
    /// and persists it on a different database. 
    /// </summary>
    public class LogConsumer : IConsumerListener
    {
        private DatabaseConnection DbConn;
        private readonly string TargetDatabase = "application";
        private readonly string TargetTable = "logs";

        public LogConsumer(DatabaseConnection dbConn)
        {
            // To avoid problems with element names.
            TargetDatabase = TargetDatabase.ToLower();
            TargetTable = TargetTable.ToLower();

            // Prepare database to receive data.
            dbConn.PrepareDatabase(TargetDatabase);
            dbConn.PrepareTable(TargetDatabase, TargetTable);

            this.DbConn = dbConn;
        }

        /// <inheritdoc/>
        public void Consume(string queueName, string message)
        {
            try
            {
                var dynamicObj = new LogEntryList().Deserialize(message);
                if (dynamicObj == null)
                {
                    var errorMsg = string.Format(
                        "Invalid JSON format read while accessing log queue.\nQueue message dump: '{0}'",
                        message);
                    LogMessage(MessageType.Error, errorMsg);
                    return;
                }

                int amount = 0;
                foreach (var log in dynamicObj)
                {
                    var serializedLog = JsonConvert.SerializeObject(log);
                    DbConn.WriteData(TargetDatabase, TargetTable, null, serializedLog);
                    amount++;
                }

                LogMessage(MessageType.Info, string.Format("Amount of log entries processed: {0}", amount));
            }
            catch (Exception e)
            {
                var errorMsg = string.Format(
                    "Internal Error or Input JSON contains invalid character or format. Error message: {0}.\nQueue message dump: '{1}'",
                    e.Message,
                    message);

                LogMessage(MessageType.Error, errorMsg);
            }
        }

        /// <summary>
        /// Small method to abstract internal message logging.
        /// Here we just print on the console but it would also
        /// receive the error category and write it on Elasticsearch as
        /// an error or alert log.
        /// 
        /// A class with this method could be shared for all microservices like
        /// I did with the JSON class, letting them write log messages
        /// following a consistent format.
        /// </summary>
        public void LogMessage(MessageType typ, string message)
        {
            Console.WriteLine(string.Format("[{0}] : {1}", typ, message));
        }

        public enum MessageType
        {
            Alert,
            Error,
            Info
        }
    }
}
