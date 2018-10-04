namespace QueueDatabase
{
    using QueueDatabase.Model.Rabbit;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var conn = new RabbitMqConnection("localhost", 5672, "applicationLogs");

            conn.RegisterConsumer();

            conn.Publish("Test 4");

            Console.WriteLine("Press CTRL+C to quit.");
            Console.ReadLine();
        }
    }
}
