namespace QueueDatabase
{
    using ConsumerToDb.Model.Queue.Rabbit;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var conn = new RabbitMqConnection("localhost", 5672, "applicationLogs");

            // To consume just call this. It will handle initialization for you.
            conn.Consume();

            // To publish you need to handle the initialization, since it isn't based on callbacks.
            conn.Initialize();
            conn.PrepareQueues();
            conn.Publish("Test 3");

            Console.WriteLine("Press ENTER to quit.");
            Console.ReadLine();
        }
    }
}
