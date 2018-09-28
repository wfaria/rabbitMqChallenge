namespace ConsumerToDb.Model.Database
{
    using System.Net;

    /// <summary>
    /// Class to communicate with an Elasticsearch database
    /// using POST requests.
    /// </summary>
    public class ElasticsearchConnection : DatabaseConnection
    {
        private readonly string ElsUrl;
        private static readonly string IndexJsonPattern =
            "{\"settings\" : {\"number_of_shards\" : 1}, \"mappings\" : { \"{0}\" : {\"properties\" : {}}}}";

        public ElasticsearchConnection(string hostname, string port)
        {
            ElsUrl = string.Format("http://{0}:{1}", hostname, port);
        }

        public override bool PrepareTable(
            string database,
            string table,
            bool failIfAlreadyCreated = false)
        {
            var url = FormatDatabaseUrl(database);
            var data = string.Format(IndexJsonPattern, table);
            SendPostRequest(url, data);

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
                var response = wb.UploadString(url, data);
            }
        }
    }
}
