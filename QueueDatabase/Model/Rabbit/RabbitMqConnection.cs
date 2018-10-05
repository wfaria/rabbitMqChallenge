namespace QueueDatabase.Model.Rabbit
{
    using System;
    using System.Text;

    using QueueDatabase.Model;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Base class for RabbitMqConnections.
    /// It does all steps but it just prints the message after reading messages for consumers.
    /// Consumers should override the method 
    /// <see cref="ConsumerCallback(string, string)"/> or just
    /// pass a <see cref="IConsumerListener"/> implementation to this class constructor.
    /// to work properly.
    /// </summary>
    public class RabbitMqConnection : QueueConnection
    {
        private IModel channel;
        private IConnection connection;
        private readonly IConnectionFactory factory;
        private readonly string queueName;

        public RabbitMqConnection(
            string hostname,
            int port,
            string queueName,
            IConsumerListener consumerListener = null,
            IConnectionFactory factory = null)
        {
            this.ConsumerListener = consumerListener;
            if (factory == null)
            {
                this.factory = new ConnectionFactory()
                {
                    HostName = hostname,
                    Port = port,
                };
            }

            this.queueName = queueName;

            // Initialize connection variables and queue.
            Initialize();
            PrepareQueues();
        }

        /// <inheritdoc/>
        public override void ConsumerCallback(string queueName, string message)
        {
            if (ConsumerListener != null)
            {
                ConsumerListener.Consume(queueName, message);
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Empty message";
            }

            Console.WriteLine(string.Format("Read message from Queue: {0}", message));
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
        public override void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
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
        }
    }
}
