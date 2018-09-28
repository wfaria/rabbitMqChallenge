namespace ConsumerToDb.Model.Database
{
    using System;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Class to communicate with an Elasticsearch database
    /// using PUT requests.
    /// </summary>
    public class ElasticsearchConnection : DatabaseConnection
    {
        private readonly string ElsUrl;

        // Empty index declaration.
        private static readonly string IndexJsonPattern = "{ }";

        // Empty type declaration.
        private static readonly string TypeJsonPattern = "{ \"properties\": { } }";
            // "{{'settings' : {{'number_of_shards' : 1}}, 'mappings' : {{ '{0}' : {{'properties' : {{}}}}}}}}";

        public ElasticsearchConnection(string hostname, int port)
        {
            ElsUrl = string.Format("http://{0}:{1}", hostname, port);
        }

        /// <inheritdoc/>
        public override void PrepareDatabase(
            string database,
            bool failIfAlreadyCreated = false)
        {
            // Prepare index.
            var url = FormatDatabaseUrl(database);
            var data = IndexJsonPattern;
            Put(url, data, failIfAlreadyCreated);
        }

        /// <inheritdoc/>
        public override void PrepareTable(
            string database,
            string table,
            bool failIfAlreadyCreated = false)
        {
            // Prepare type inside index.
            var typeUrl = FormatTypeCreationUrl(database, table);
            var typeData = TypeJsonPattern;
            Put(typeUrl, typeData, failIfAlreadyCreated);
        }

        /// <inheritdoc/>
        public override void WriteData(
            string database,
            string table,
            string id,
            string data)
        {
            var url = FormatDatabaseUrl(database, table, id);

            // Using always false as last parameter since it is fine
            // to overwrite the element.
            Put(url, data, false);
        }

        /// <summary>
        /// Gets an URL to create a new type.
        /// </summary>
        /// <param name="index">Index name on Elasticsearch.</param>
        /// <param name="type">Child type name on Elasticsearch.</param>
        private string FormatTypeCreationUrl(string index, string type)
        {
            return string.Format("{0}/{1}/_mapping/{2}", ElsUrl, index, type);
        }

        /// <summary>
        /// Gets an URL to create a new index.
        /// </summary>
        /// <param name="index">Index name on Elasticsearch.</param>
        private string FormatDatabaseUrl(string index)
        {
            return string.Format("{0}/{1}", ElsUrl, index);
        }

        /// <summary>
        /// Gets an URL to create a new document part of a type inside of an index.
        /// </summary>
        /// <param name="index">Index name on Elasticsearch.</param>
        /// <param name="type">Child type name on Elasticsearch.</param>
        /// <param name="id">Document unique ID.</param>
        private string FormatDatabaseUrl(string index, string type, string id)
        {
            return string.Format(
                "{0}/{1}/{2}/{3}",
                ElsUrl,
                index,
                type,
                id);
        }

        private void Put(string url, string data, bool failIfAlreadyCreated)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Encoding = System.Text.Encoding.UTF8;

                byte[] b = Encoding.UTF8.GetBytes(data);
                client.UploadData(new Uri(url), "PUT", b);
            }
            catch(WebException e)
            {
                HttpWebResponse res = (HttpWebResponse)e.Response;
                Console.WriteLine(url + " " + res.StatusDescription);
                if (res.StatusCode != HttpStatusCode.BadRequest || failIfAlreadyCreated)
                {
                    throw e;
                }
            }
        }
    }
}
