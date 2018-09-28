namespace ConsumerToDb.Model.Queue.Rabbit
{
    using System;
    using System.Text;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Base class for RabbitMQConsumers.
    /// It does all steps but it just prints the message after reading.
    /// Other classes should just override thhe ConsumerCallback to work with
    /// queue messages.
    /// </summary>
    public class RabbitMQConsumer : QueueConsumer 
    {
        private IModel channel;
        private IConnection connection;
        private readonly IConnectionFactory factory;
        private readonly string queueName;

        public RabbitMQConsumer(
            string hostname,
            int port,
            string queueName,
            IConnectionFactory factory = null)
        {
            if (factory == null)
            {
                this.factory = new ConnectionFactory()
                {
                    HostName = hostname,
                    Port = port,
                };
            }
            
            this.queueName = queueName;
        }

        /// <inheritdoc/>
        public override void ConsumerCallback(string queueName, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Empty message";
            }

            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        /// <inheritdoc/>
        public override void PrepareQueues()
        {
            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        /// <inheritdoc/>
        public override void RegisterConsumer()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                ConsumerCallback(queueName, message);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            var msg = string.Format(
                "Consumer connected on queue {0}. Press [CTRL+C] to exit.",
                queueName);
            Console.WriteLine(msg);
        }
    }
}
