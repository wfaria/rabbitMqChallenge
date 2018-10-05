namespace JsonHelper.Model
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// A list of <see cref="LogEntry"/> objects. See its
    /// summary documentation for more details.
    /// </summary>
    public class LogEntryList : JsonBase
    {
        [JsonProperty(Required = Required.Default)]
        public LogEntry[] LogEntries { get; set; }

        public override dynamic Deserialize(string json)
        {
            try
            {
                // We reuse the other object structure but get them as a list.
                var obj = JsonConvert.DeserializeObject<LogEntry[]>(json);
                foreach (var log in obj)
                {
                    // See the project's README.md, this part can be improved in future.
                    var serialization = log.Serialize();
                    var validatedSerialization = log.Deserialize(serialization);
                    if (validatedSerialization == null)
                    {
                        // The log message has some validation issues, so we invalidate the whole list.
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                // In this case we prefer to abort the entire process rather than
                // return the list with missing elements if any element is incomplete.
                return null;
            }

            return JsonConvert.DeserializeObject(json);
        }

        public override string Serialize()
        {
            // We need this because the original JSON payload includes
            // a log array without name.
            return JsonConvert.SerializeObject(LogEntries);
        }
    }
}
