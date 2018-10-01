namespace ConsumerToDb
{
    using System;

    using ConsumerToDb.Controllers;
    using ConsumerToDb.Model.Database;

    class Program
    {
        static void Main(string[] args)
        {
            // We just connect our custom consumer to RabbitMQ
            // and let its callback method wait for new queue messages,
            // storing only the valid log entries into Elasticsearch.
            var consumer = new LogConsumer(
                "localhost",
                5672,
                "applicationLogs",
                new ElasticsearchConnection("localhost", 9200));

            consumer.RegisterConsumer();

            Console.WriteLine("Press CTRL+C to quit.");
            Console.ReadLine();
        }
    }
}
