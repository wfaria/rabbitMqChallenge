namespace ConsumerToDb
{
    using ConsumerToDb.Model.Database;
    using ConsumerToDb.Model.Queue.Rabbit;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            // new RabbitMQConsumer("localhost", "applicationLogs").Consume();

            var conn = new DatabaseConnection();
            conn.PrepareTable("test", "table");
            conn.WriteData("test", "table", "1", "data");
            conn.WriteData("test", "table", "2", "data");

            var elsConn = new ElasticsearchConnection("localhost", "9200");
            elsConn.PrepareTable("test45", "type1");
            // elsConn.WriteData("test3", "type1", "1", "{{'field1': 'Hello VS World!', 'field2': 'Test'}}");

            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
        }
    }
}
