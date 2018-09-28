namespace ConsumerToDb
{
    using ConsumerToDb.Model.Database;
    using ConsumerToDb.Model.Queue.Rabbit;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            // new RabbitMQConsumer("localhost", 5672, "applicationLogs").Consume();

            var conn = new DatabaseConnection();
            conn.PrepareDatabase("test");
            conn.PrepareTable("test", "table");
            conn.WriteData("test", "table", "1", "data");
            conn.WriteData("test", "table", "2", "data");

            var elsConn = new ElasticsearchConnection("localhost", 9200);
            elsConn.PrepareDatabase("test45");
            elsConn.PrepareTable("test45", "type1");
            elsConn.WriteData("test45", "type1", "2", "{\"field1\": \"Hello VS World!\", \"field2\": \"Test Two\"}");

            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
        }
    }
}
