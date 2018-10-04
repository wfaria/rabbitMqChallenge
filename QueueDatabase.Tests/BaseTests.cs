namespace QueueDatabase.Tests
{
    using System;

    using QueueDatabase.Model;
    using Xunit;

    public class BaseTests
    {
        [Fact]
        public void IsMockQueueOk()
        {
            var queueConn = new QueueConnection();
            queueConn.Initialize();
            queueConn.PrepareQueues();
            Assert.True(
                queueConn.Queues.Count == 1,
                "We should have one queue to be accessed.");
            Assert.True(
                queueConn.PublishedMessages.Count == 0,
                "We didn't publish any message yet.");
            Assert.True(
                queueConn.MessagesFromCallbacks.Count == 0,
                "We didn't generate any callback yet.");

            queueConn.Publish("test");
            Assert.True(
                queueConn.PublishedMessages.Count == 1,
                "The queue should contain one message with all 50 logs.");

            queueConn.RegisterConsumer();
            Assert.True(
                queueConn.MessagesFromCallbacks.Count == 3,
                "The queue should contain 3 messages.");
        }
    }
}
