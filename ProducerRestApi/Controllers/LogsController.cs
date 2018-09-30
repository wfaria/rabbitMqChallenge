namespace ProducerRestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    using ConsumerToDb.Model.Queue;
    using ConsumerToDb.Model.Queue.Rabbit;
    using JsonHelper.Model;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private QueueConnection QueueConn;

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "valueTest1", "valueTest2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public HttpResponseMessage Post([FromBody] string value)
        {
            var response = new HttpResponseMessage();
            try
            {
                var dynamicObj = new LogEntryList().Deserialize(value);
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
                PublishLog(value);
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

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private void PublishLog(string message)
        {
            if (QueueConn == null)
            {
                QueueConn = new RabbitMqConnection("localhost", 5672, "applicationLogs");
            }

            QueueConn.Publish(message);
        }
    }
}
