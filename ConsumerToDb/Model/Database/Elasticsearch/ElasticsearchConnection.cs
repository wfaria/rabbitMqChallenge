namespace ConsumerToDb.Model.Database
{
    using System;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Class to communicate with an Elasticsearch database
    /// using POST requests.
    /// </summary>
    public class ElasticsearchConnection : DatabaseConnection
    {
        private readonly string ElsUrl;

        // Empty index declaration.
        private static readonly string IndexJsonPattern = "{ }";

        // Empty type declaration.
        private static readonly string TypeJsonPattern = "{'properties': {}}";
            // "{{'settings' : {{'number_of_shards' : 1}}, 'mappings' : {{ '{0}' : {{'properties' : {{}}}}}}}}";

        public ElasticsearchConnection(string hostname, string port)
        {
            ElsUrl = string.Format("http://{0}:{1}", hostname, port);
        }

        public override bool PrepareTable(
            string database,
            string table,
            bool failIfAlreadyCreated = false)
        {
            // Prepare index
            var url = FormatDatabaseUrl(database);
            var data = IndexJsonPattern;
            Console.WriteLine(url);
            Console.WriteLine(data);
            Post(url, data);

            // Prepare type inside index.
            //var typeUrl = FormatTypeCreationUrl(database, table);
            //var typeData = TypeJsonPattern;
            //Console.WriteLine(typeUrl);
            //Console.WriteLine(typeData);
            //Post(typeUrl, typeData);

            return true;
        }

        public override bool WriteData(
            string database,
            string table,
            string id,
            string data,
            bool failIfAlreadyCreated = false)
        {
            var url = FormatDatabaseUrl(database, table, id);
            SendPostRequest(url, data);

            return true;
        }

        private string FormatTypeCreationUrl(string index, string type)
        {
            return string.Format("{0}/{1}/_mapping/{2}", ElsUrl, index, type);
        }

        private string FormatDatabaseUrl(string index)
        {
            return string.Format("{0}/{1}", ElsUrl, index);
        }

        private string FormatDatabaseUrl(string index, string type, string id)
        {
            return string.Format(
                "{0}/{1}/{2}/{3}",
                ElsUrl,
                index,
                type,
                id);
        }

        private void SendPostRequest(string url, string data)
        {
            using (var wb = new WebClient())
            {
                // wb.Headers["Content-Type"] = "application/json";
                //wb.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wb.Headers.Add("Content-Type", "application/json");
                
                var response = wb.UploadString(url, data);

                //byte[] d = Encoding.UTF8.GetBytes(data);
                //wb.UploadData(url, d);
            }
        }

        public void Post(string url, string data)
        {
            WebClient client = new WebClient();
            client.UploadDataCompleted += (s, e) =>
            {
                if (e.Error == null && e.Result != null)
                {
                    try
                    {
                        string response = Encoding.UTF8.GetString(e.Result);
                        Console.WriteLine("Success");
                        Console.WriteLine(response);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Response parsing error");
                        Console.WriteLine(ex);
                    }
                }

                else
                    Console.WriteLine("No Response error");
                    Console.WriteLine(e.Error);
            };

            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Encoding = System.Text.Encoding.UTF8;

            byte[] b = Encoding.UTF8.GetBytes(data);
            client.UploadDataAsync(new Uri(url), "PUT", b);
        }
    }
}
