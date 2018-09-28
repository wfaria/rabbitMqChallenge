namespace ConsumerToDb
{
    using ConsumerToDb.Model.Database;
    using ConsumerToDb.Model.Queue.Rabbit;

    class Program
    {
        static void Main(string[] args)
        {
            // new RabbitMQConsumer("localhost", "applicationLogs").Consume();
            var conn = new DatabaseConnection();
            conn.PrepareTable("test", "table");
            conn.WriteData("test", "table", "1", "data");
            conn.WriteData("test", "table", "2", "data");
        }
    }
}
