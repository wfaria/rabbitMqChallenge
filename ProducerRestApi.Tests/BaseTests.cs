namespace ProducerRestApi.Tests
{
    using System;

    using JsonHelper.Model;
    using ProducerRestApi.Controllers;
    using QueueDatabase.Model;
    using Xunit;

    public class BaseTests
    {
        private LogsController Controller;

        public BaseTests()
        {
            // Creating Http controller and its mock connection for tests.
            Controller = new LogsController(new QueueConnection());
            Controller.QueueConn.Initialize();
            Controller.QueueConn.PrepareQueues();
        }

        [Fact]
        public void IsBasicPostMethodOk()
        {
            var logList = new LogEntryList();
            logList.LogEntries = new LogEntry[] { new LogEntry(), new LogEntry(), new LogEntry() };
            Assert.True(
                Controller.QueueConn.Queues.Count == 1,
                "We should have one queue to be accessed"
                );
            Assert.True(
                Controller.QueueConn.PublishedMessages.Count == 0,
                "We didn't publish any message yet."
                );

            var serialization = logList.Serialize();
            var response = Controller.Post(serialization);
            Assert.True(
                Controller.QueueConn.PublishedMessages.Count == 1,
                "The queue should contain one message."
                );
            Assert.True(
                response.StatusCode == System.Net.HttpStatusCode.OK,
                "The message should be processed without errors."
                );

            var invalidInputResponse = Controller.Post("[{'MachineName' : 'MachineName'}]");
            Assert.True(
                Controller.QueueConn.PublishedMessages.Count == 1,
                "The queue should still with only one message."
                );
            Assert.True(
                invalidInputResponse.StatusCode == System.Net.HttpStatusCode.BadRequest,
                "The response should report an error."
                );
        }
        [Fact]
        public void IsRandomPostMethodOk()
        {
            var numberOfRandomLogs = 50;
            Assert.True(
                Controller.QueueConn.Queues.Count == 1,
                "We should have one queue to be accessed"
                );
            Assert.True(
                Controller.QueueConn.PublishedMessages.Count == 0,
                "We didn't publish any message yet."
                );

            // The second parameter is necessary because it is a post method,
            // but it is ignored by the method logic.
            var response = Controller.PostRandom(numberOfRandomLogs, string.Empty);
            Assert.True(
                Controller.QueueConn.PublishedMessages.Count == 1,
                "The queue should contain one message with all 50 logs."
                );
            Assert.True(
                response.StatusCode == System.Net.HttpStatusCode.OK,
                "The message should be processed without errors."
                );

            var logList = new LogEntryList().Deserialize(Controller.QueueConn.PublishedMessages[0]);
            var count = 0;

            // Since we have a dynamic object, we need to do this to count the list size.
            foreach (var log in logList)
            {
                count++;
            }

            Assert.True(
                count == numberOfRandomLogs,
                "The amount of published logs is wrong."
                );
        }
    }
}
