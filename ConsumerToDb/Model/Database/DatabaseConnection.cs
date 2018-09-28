namespace ConsumerToDb.Model.Database
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Mock class for classic database connections.
    /// Recommended when you need to create a connection instance
    /// and use it sporadically to read or write data.
    /// It should make it easier to change database systems
    /// without affecting multiple files in the code.
    /// </summary>
    public class DatabaseConnection
    {
        private Dictionary<string, string> GeneratedData;
        private HashSet<string> PreparedTables;
        private HashSet<string> PreparedDatabases;

        /// <summary>
        /// Basic constructor.
        /// </summary>
        public DatabaseConnection()
        {
            GeneratedData = new Dictionary<string, string>();
            PreparedDatabases = new HashSet<string>();
            PreparedTables = new HashSet<string>();
        }

        public virtual void PrepareDatabase(
            string database,
            bool failIfAlreadyCreated = false)
        {
            if (failIfAlreadyCreated && PreparedDatabases.Contains(database))
            {
                throw new ArgumentException(string.Format("'{0}' already exists on Database", database));
            }

            PreparedDatabases.Add(database);
        }

        public virtual void PrepareTable(
            string database,
            string table,
            bool failIfAlreadyCreated = false)
        {
            var key = string.Format("{0}/{1}", database, table);
            if (failIfAlreadyCreated && PreparedTables.Contains(key))
            {
                throw new ArgumentException(string.Format("'{0}' already exists on Database", key));
            }

            PreparedTables.Add(key);
        }

        public virtual void WriteData(
            string database,
            string table,
            string id,
            string data)
        {
            var key = string.Format("{0}/{1}/{2}", database, table, id);
            GeneratedData[key] = data;
        }
    }
}
