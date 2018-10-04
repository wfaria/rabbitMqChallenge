namespace ProducerRestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    using JsonHelper.Model;
    using Microsoft.AspNetCore.Mvc;
    using QueueDatabase.Model;
    using QueueDatabase.Model.Rabbit;

    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        /// <summary>
        /// Connection object used to access queues to post log messages.
        /// </summary>
        public QueueConnection QueueConn;

        /// <summary>
        /// Base constructor used for mock tests.
        /// </summary>
        /// <param name="queueConn">A mock queue connection.</param>
        public LogsController(QueueConnection queueConn)
        {
            QueueConn = queueConn;
        }

        // POST api/logs
        [HttpPost]
        public HttpResponseMessage Post([FromBody] string value)
        {
            return GenericLogsPost(value);
        }

        // POST api/logs/5
        [HttpPost("{randomLogsNum}")]
        public HttpResponseMessage PostRandom(int randomLogsNum, [FromBody] string value)
        {
            if (randomLogsNum < 1)
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ReasonPhrase = "The number of logs should be greater than 0.";
                return response;
            }

            var logs = new LogEntryList();
            logs.LogEntries = new LogEntry[randomLogsNum];
            Random rnd = new Random();
            for (int i = 0; i < randomLogsNum; i++)
            {
                // Forcing smallest ID process = fastest process.
                var processNameId = rnd.Next(1, 21);
                var responseTime = rnd.Next(100,  100 + rnd.Next(10, 10 * processNameId));
                var log = new LogEntry();
                log.ProductName = "Product " + processNameId;
                log.AdditionalData["ResponseTime"] = responseTime;
                logs.LogEntries[i] = log;
            }

            try
            {
                return GenericLogsPost(logs.Serialize());
            }
            catch (Exception e)
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ReasonPhrase = string.Format(
                    "Internal Error while serializing random logs. Error message: {0}",
                    e.Message);
                return response;
            }

        }

        private void PublishLog(string message)
        {
            if (QueueConn == null)
            {
                QueueConn = new RabbitMqConnection("localhost", 5672, "applicationLogs");
            }

            QueueConn.Publish(message);
        }

        private HttpResponseMessage GenericLogsPost(string serializedLogs)
        {
            var response = new HttpResponseMessage();
            try
            {
                var dynamicObj = new LogEntryList().Deserialize(serializedLogs);
                if (dynamicObj == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ReasonPhrase = "Input JSON string doesn't contain all required fields in all objects.";
                    return response;
                }

                response.StatusCode = HttpStatusCode.OK;
                response.ReasonPhrase = "Log list received with success.";
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ReasonPhrase = string.Format(
                    "Internal Error or Input JSON contains invalid character or format. Error message: {0}",
                    e.Message);
                return response;
            }

            try
            {
                // We already have the object deserialized, so we can just publish it.
                PublishLog(serializedLogs);
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ReasonPhrase = string.Format(
                    "Internal error while publishing logs. Error message: {0}",
                    e.Message);
            }

            return response;
        }
    }
}
