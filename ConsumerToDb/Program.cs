namespace ConsumerToDb
{
    using ConsumerToDb.Model.Queue.Rabbit;

    class Program
    {
        static void Main(string[] args)
        {
            new RabbitMQConsumer("localhost", "applicationLogs").Consume();
        }
    }
}
