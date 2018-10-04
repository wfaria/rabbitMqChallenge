namespace ConsumerToDb
{
    using System;

    using ConsumerToDb.Controllers;
    using ConsumerToDb.Model.Database;
    using QueueDatabase.Model.Rabbit;

    class Program
    {
        static void Main(string[] args)
        {
            // We just connect our custom consumer to RabbitMQ
            // and let its callback method wait for new queue messages,
            // storing only the valid log entries into Elasticsearch.
            var consumerListener = new LogConsumer(new ElasticsearchConnection("localhost", 9200));

            var queueConn = new RabbitMqConnection(
                "localhost",
                5672,
                "applicationLogs",
                consumerListener);

            queueConn.RegisterConsumer();

            Console.WriteLine("Log consumer running. Press CTRL+C to quit.");
            Console.ReadLine();
        }
    }
}
