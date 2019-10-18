using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace GR.Entities.Abstractions.Helpers
{
    public static class DbConnectionFactory
    {
        /// <summary>
        /// Connections
        /// </summary>
        private static List<DbConnection> Connections { get; set; } = new List<DbConnection>();

        /// <summary>
        /// Get free connection
        /// </summary>
        /// <returns></returns>
        private static DbConnection GetConnection()
        {
            var unusedConnections =
                Connections.Where(x => x.State == ConnectionState.Broken || x.State == ConnectionState.Closed).ToList();

            if (unusedConnections.Count > Connections.Count + 2)
            {
                foreach (var unusedConnection in unusedConnections)
                {
                    unusedConnection.Close();
                    Connections.Remove(unusedConnection);
                }
            }

            var connection = Connections.FirstOrDefault(x =>
                x.State != ConnectionState.Executing || x.State != ConnectionState.Fetching);

            if (connection != null) return connection;
            var first = Connections.FirstOrDefault();
            first?.Close();
            Connections.Add(first);
            connection = first;
            return connection;
        }

        /// <summary>
        /// Close all connections
        /// </summary>
        public static void CloseAll()
        {
            Connections.ToList().ForEach(x => x.Close());
        }

        public static class Connection
        {
            /// <summary>
            /// Get connection
            /// </summary>
            /// <returns></returns>
            public static DbConnection Get()
            {
                if (!Connections.Any()) throw new Exception("Connection not initiated");
                var connection = GetConnection();
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                return connection;
            }

            /// <summary>
            /// Set connection
            /// </summary>
            /// <typeparam name="TProvider"></typeparam>
            /// <param name="connection"></param>
            public static void SetConnection<TProvider>(TProvider connection)
                where TProvider : DbConnection
            {
                Connections.Add(connection);
            }
        }
    }
}
