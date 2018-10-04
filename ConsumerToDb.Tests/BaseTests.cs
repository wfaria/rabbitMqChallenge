namespace ConsumerToDb.Tests
{
    using System;

    using ConsumerToDb.Controllers;
    using ConsumerToDb.Model.Database;
    using JsonHelper.Model;
    using QueueDatabase.Model;
    using Xunit;

    public class BaseTests
    {
        [Fact]
        public void IsConsumerListenerOk()
        {
            // We create the listener like we do on the actual consumer service,
            // but we use mock database connections.
            var dbConn = new DatabaseConnection();
            var consumerListener = new LogConsumer(dbConn);
            Assert.True(
                dbConn.PreparedDatabases.Count == 1,
                "We should have one database to be accessed.");
            Assert.True(
                dbConn.PreparedTables.Count == 1,
                "We should have one table to be accessed.");
            Assert.True(
                dbConn.GeneratedData.Count == 0,
                "We didn't publish any data yet.");

            var queueConn = new QueueConnection();
            queueConn.Initialize();
            queueConn.PrepareQueues();
            queueConn.ConsumerListener = consumerListener;
            Assert.True(
                queueConn.Queues.Count == 1,
                "We should have one queue to be accessed.");
            Assert.True(
                queueConn.PublishedMessages.Count == 0,
                "We didn't publish any message yet.");
            Assert.True(
                queueConn.MessagesFromCallbacks.Count == 0,
                "We didn't generate any callback yet.");

            // Simulating list reception from queue.
            var logList = new[] { new LogEntry(), new LogEntry(), new LogEntry() };
            var logs = new LogEntryList
            {
                LogEntries = logList
            };

            var sourceQueue = "queueName";
            var serialization = logs.Serialize();
            queueConn.ConsumerCallback(sourceQueue, serialization);
            Assert.True(
                queueConn.MessagesFromCallbacks.Count == 1,
                "We generated 1 callback.");
            Assert.True(
                dbConn.GeneratedData.Count == logList.Length,
                "Since the input had a valid JSON list with 3 elmeents, they should have been posted on the database.");

            // Simulating bad data reception from queue.
            queueConn.ConsumerCallback(sourceQueue, "[{'MachineName': 'MachineName'}]");
            Assert.True(
                queueConn.MessagesFromCallbacks.Count == 2,
                "We generated 2 callbacks now.");
            Assert.True(
                dbConn.GeneratedData.Count == logList.Length,
                "Since the input had an invalid JSON list, the database amount of data shouldn't change.");

        }
    }
}
