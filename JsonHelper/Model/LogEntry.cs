namespace JsonHelper.Model
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

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

        public override dynamic Deserialize(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<LogEntry>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return JsonConvert.DeserializeObject(json);
        }
    }
}
