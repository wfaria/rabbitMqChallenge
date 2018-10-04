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
        [JsonProperty(Required = Required.Default)]
        public Dictionary<string, int> AdditionalData { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ApplicationId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string MachineName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Message { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string NativeProcessId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string NativeThreadId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string OSFullName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ProcessName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ProcessPath { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ProductCompany { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ProductName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ProductVersion { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Severity { get; set; }

        [JsonProperty(Required = Required.Default)]
        public string[] Tags { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Timestamp { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string TypeName { get; set; }

        public LogEntry()
        {
            AdditionalData = new Dictionary<string, int>();
            ApplicationId = "f24dbb31-a065-4fa7-b6f8-19cddac7d7c5";
            MachineName = "MachineName";
            Message = "New message";
            NativeProcessId = "1";
            NativeThreadId = "1";
            OSFullName = "Ubuntu";
            ProcessName = "Process.exe";
            ProcessPath = "C\\Application\\Process.exe";
            ProductCompany = "MyCompany";
            ProductName = "Product";
            ProductVersion = "1.0.0";
            Severity = "Info";
            Tags = new string[0]; // empty array.
            Timestamp = DateTime.UtcNow.ToString("o");
            TypeName = "LogEntry";
        }

        /// <inheritdoc/>
        public override dynamic Deserialize(string json)
        {
            try
            {
                // You can also do other logic or business verifications here if you need.
                var obj = JsonConvert.DeserializeObject<LogEntry>(json);
                var auxNum = 0;
                if (!int.TryParse(obj.NativeProcessId, out auxNum) || 
                    !int.TryParse(obj.NativeThreadId, out auxNum))
                {
                    // Example of schema validation. Invalid number string.
                    return null;
                }
            }
            catch (Exception)
            {
                // The field annotations will make the deserialization fail
                // if any field is missing as expected.
                return null;
            }

            return JsonConvert.DeserializeObject(json);
        }
    }
}
