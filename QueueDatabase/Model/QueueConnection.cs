namespace ConsumerToDb.Model.Queue
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Mock class of an object that access a message broker software 
    /// based on Queues to store messages.
    /// </summary>
    public class QueueConnection
    {
        private HashSet<string> queues;
        private HashSet<string> messages;
        private List<string> publishedMessages;

        /// <summary>
        /// Starts main loop to consume messages from any queue abstraction.
        /// </summary>
        public void Consume()
        {
            try
            {
                Initialize();
                PrepareQueues();
                RegisterConsumer();
            }
            catch (Exception e)
            {
                CatchException(e);
            }
        }

        /// <summary>
        /// Method called on errors to end any pendent task before finishing the program.
        /// This can also be used to log errors on other databases.
        /// </summary>
        public virtual void CatchException(Exception e)
        {
            Console.WriteLine(e);
        }

        /// <summary>
        /// The method that is called by each callback registered by
        /// <see cref="RegisterConsumer"/>.
        /// </summary>
        public virtual void ConsumerCallback(string queueName, string message)
        {
            messages.Add(message);
        }

        /// <summary>
        /// Initialize object before starting consumption.
        /// </summary>
        public virtual void Initialize()
        {
            messages = new HashSet<string>();
            publishedMessages = new List<string>();
            queues = new HashSet<string>();
        }

        /// <summary>
        /// Prepare the target queues to be accessed on the target database.
        /// </summary>
        public virtual void PrepareQueues()
        {
            queues.Add("queueName");
        }

        /// <summary>
        /// Publishes a message to the target queue.
        /// </summary>
        /// <param name="message">Any string which is valid for the target queue context.</param>
        public virtual void Publish(string message)
        {
            publishedMessages.Add(message);
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
