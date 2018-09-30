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
        public override dynamic Deserialize(string json)
        {
            try
            {
                // We reuse the other object structure but get them as a list.
                var obj = JsonConvert.DeserializeObject<LogEntry[]>(json);
            }
            catch (Exception e)
            {
                // In this case we prefer to abort the entire process rather than
                // return the list with missing elements if any element is incomplete.
                Console.WriteLine(e);
                return null;
            }

            return JsonConvert.DeserializeObject(json);
        }
    }
}
