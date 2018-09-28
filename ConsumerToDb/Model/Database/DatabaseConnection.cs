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
        
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public DatabaseConnection()
        {
            GeneratedData = new Dictionary<string, string>();
            PreparedTables = new HashSet<string>();
        }

        public virtual bool PrepareTable(
            string database,
            string table,
            bool failIfAlreadyCreated = false)
        {
            var key = string.Format("{0}/{1}", database, table);
            if (failIfAlreadyCreated && GeneratedData.ContainsKey(key))
            {
                throw new ArgumentException(string.Format("'{0}' already exists on Database", key));
            }

            PreparedTables.Add(key);
            return true;
        }

        public virtual bool WriteData(
            string database,
            string table,
            string id,
            string data,
            bool failIfAlreadyCreated = false)
        {
            var key = string.Format("{0}/{1}/{2}", database, table, id);
            if (failIfAlreadyCreated && GeneratedData.ContainsKey(key))
            {
                throw new ArgumentException(string.Format("'{0}' already exists on Database", key));
            }

            GeneratedData[key] = data;
            return true;
        }
    }
}
