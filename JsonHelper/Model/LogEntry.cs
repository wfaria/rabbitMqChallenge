namespace JsonHelper.Model
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Represents an application log entry. Containing elements
    /// like time, machine info and others. All fields listed here must
    /// be defined or the deserialization method will fail.
    /// </summary>
    [JsonObject]
    public class LogEntry : JsonBase
    {
        [JsonProperty(Required = Required.Always)]
        public string ApplicationId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, int> AdditionalData { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string MachineName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Message { get; set; }

        /// <inheritdoc/>
        public override dynamic Deserialize(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<LogEntry>(json);
            }
            catch (Exception e)
            {
                // The field annotations will make the deserialization fail
                // if any field is missing as expected.
                Console.WriteLine(e);
                return null;
            }

            return JsonConvert.DeserializeObject(json);
        }
    }
}
