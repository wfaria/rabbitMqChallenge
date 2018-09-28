namespace ConsumerToDb.Model.Database
{
    using System;
    using System.Collections.Generic;

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

        /// <summary>
        /// Prepares the database to be accessed by this class.
        /// </summary>
        /// <param name="database">Database identifier.</param>
        /// <param name="failIfAlreadyCreated">Raises an <see cref="ArgumentException"/> if the element already exists on database.</param>
        public virtual void PrepareDatabase(
            string database,
            bool failIfAlreadyCreated = false)
        {
            if (failIfAlreadyCreated && PreparedDatabases.Contains(database))
            {
                throw GetExistentElementException(
                    new ArgumentException(string.Format("'{0}' already exists on Database", database)));
            }

            PreparedDatabases.Add(database);
        }

        /// <summary>
        /// Prepares a table to be accessed by this class.
        /// </summary>
        /// <param name="database">Database identifier.</param>
        /// <param name="table">Database table identifier.</param>
        /// <param name="failIfAlreadyCreated">Raises an <see cref="ArgumentException"/> if the element already exists on database.</param>
        public virtual void PrepareTable(
            string database,
            string table,
            bool failIfAlreadyCreated = false)
        {
            var key = string.Format("{0}/{1}", database, table);
            if (failIfAlreadyCreated && PreparedTables.Contains(key))
            {
                throw GetExistentElementException(
                    new ArgumentException(string.Format("'{0}' already exists on Database", key)));
            }

            PreparedTables.Add(key);
        }

        /// <summary>
        /// Write a data string into a table.
        /// </summary>
        /// <param name="database">Database identifier.</param>
        /// <param name="table">Database table identifier.</param>
        /// <param name="id">New or updated entry identifier.</param>
        /// <param name="data">Data represented on common string format.</param>
        public virtual void WriteData(
            string database,
            string table,
            string id,
            string data)
        {
            var key = string.Format("{0}/{1}/{2}", database, table, id);
            GeneratedData[key] = data;
        }

        /// <summary>
        /// Since different databases have different ways to detect
        /// duplicated insertion cases, we let each class detect it and
        /// use this method to signal the exception.
        /// </summary>
        /// <param name="innerException">An exception object with more details regarding the problem.</param>
        /// <returns>A new exception containing the inner exception.</returns>
        protected ArgumentException GetExistentElementException(Exception innerException)
        {
            return new ArgumentException("The element already exists on database.", innerException);
        }
    }
}
