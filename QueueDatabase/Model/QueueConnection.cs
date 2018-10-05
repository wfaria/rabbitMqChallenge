namespace QueueDatabase.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Mock class of an object that access a message broker software 
    /// based on Queues to store messages.
    /// </summary>
    public class QueueConnection
    {
        public IConsumerListener ConsumerListener;
        public HashSet<string> Queues;
        public HashSet<string> MessagesFromCallbacks;
        public List<string> PublishedMessages;

        /// <summary>
        /// The method that is called by each callback registered by
        /// <see cref="RegisterConsumer"/>.
        /// </summary>
        public virtual void ConsumerCallback(string queueName, string message)
        {
            if (ConsumerListener != null)
            {
                ConsumerListener.Consume(queueName, message);
            }

            MessagesFromCallbacks.Add(message);
        }

        /// <summary>
        /// Initialize object before starting consumption and the database state
        /// if necessary.
        /// </summary>
        public virtual void Initialize()
        {
            MessagesFromCallbacks = new HashSet<string>();
            PublishedMessages = new List<string>();
            Queues = new HashSet<string>();
        }

        /// <summary>
        /// Prepare the target queues to be accessed on the target database.
        /// </summary>
        public virtual void PrepareQueues()
        {
            Queues.Add("queueName");
        }

        /// <summary>
        /// Publishes a message to the target queue.
        /// </summary>
        /// <param name="message">Any string which is valid for the target queue context.</param>
        public virtual void Publish(string message)
        {
            PublishedMessages.Add(message);
        }

        /// <summary>
        /// Register callback for each message received from the database.
        /// </summary>
        public virtual void RegisterConsumer()
        {
            // Calling methods directly here, on real application it should be 
            // called inside of a proper callback method.
            ConsumerCallback("queueName", "Test one");
            ConsumerCallback("queueName", "Test two");
            ConsumerCallback("queueName", "Test three");
        }
    }
}
