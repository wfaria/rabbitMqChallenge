namespace ConsumerToDb.Model.Database
{
    using System;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Class to communicate with an Elasticsearch database.
    /// </summary>
    public class ElasticsearchConnection : DatabaseConnection
    {
        private readonly string ElsUrl;

        // Empty index declaration.
        private static readonly string IndexJsonPattern = "{ }";

        // Empty type declaration.
        private static readonly string TypeJsonPattern = "{ \"properties\": { } }";
        
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="hostname">The IP of the server machine.</param>
        /// <param name="port">The port used on the server machine.</param>
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
            HttpPublish(url, data, failIfAlreadyCreated);
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
            HttpPublish(typeUrl, typeData, failIfAlreadyCreated);
        }

        /// <inheritdoc/>
        public override void WriteData(
            string database,
            string table,
            string id,
            string data)
        {
            var url = FormatDatabaseUrl(database, table, id);
            HttpPublish(url, data, failIfAlreadyCreated: false, idOnUrl: !string.IsNullOrEmpty(id));
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
        /// <param name="id">Document unique ID, it can be null when it should be created by Elasticsearch.</param>
        private string FormatDatabaseUrl(string index, string type, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = string.Empty;
            }

            return string.Format(
                "{0}/{1}/{2}/{3}",
                ElsUrl,
                index,
                type,
                id);
        }

        /// <summary>
        /// Sends an Update command to the Elasticserver machine
        /// using a PUT method (when the ID is known) or the POST method instead.
        /// </summary>
        /// <param name="url">Any URL indexing an Elasticsearch element.</param>
        /// <param name="data">A string describing the update command.</param>
        /// <param name="failIfAlreadyCreated">Raises an <see cref="ArgumentException"/> if the element already exists on database.</param>
        /// <param name="idOnUrl">If true it will use the PUT verb, otherwise will use the POST and let the server create the ID.</param>
        private void HttpPublish(string url, string data, bool failIfAlreadyCreated, bool idOnUrl = true)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Encoding = System.Text.Encoding.UTF8;
                byte[] b = Encoding.UTF8.GetBytes(data);
                client.UploadData(new Uri(url), (idOnUrl ? "PUT" : "POST"), b);
            }
            catch (WebException e)
            {
                HttpWebResponse res = (HttpWebResponse)e.Response;
                if (res.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ArgumentException("The URL path doesn't exist on database. Check if there is missing an index or type there.");
                }
                else if (res.StatusCode != HttpStatusCode.BadRequest || failIfAlreadyCreated)
                {
                    throw GetExistentElementException(e);
                }
            }
        }
    }
}
