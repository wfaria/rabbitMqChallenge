namespace QueueDatabase.Model
{
    /// <summary>
    /// Base interface to receive messages from <see cref="QueueConnection"/>.
    /// The consumption logic should be implemented based on this interface,
    /// abstracting the database access logic into the connection classes.
    /// </summary>
    public interface IConsumerListener
    {
        /// <summary>
        /// This method should be called by the <see cref="QueueConnection"/>
        /// when it receives a message from some queue. Ideally inside of the
        /// <see cref="QueueConnection.ConsumerCallback(string, string)"/> method.
        /// </summary>
        void Consume(string queueName, string message);
    }
}
