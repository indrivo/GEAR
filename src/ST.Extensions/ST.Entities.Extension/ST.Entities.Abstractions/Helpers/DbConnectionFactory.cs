using System;
using System.Data;
using System.Data.Common;

namespace ST.Entities.Abstractions.Helpers
{
    public static class DbConnectionFactory
    {
        public static class Connection
        {
            /// <summary>
            /// Close connection
            /// </summary>
            public static void Close()
            {
                DbConnection?.Close();
            }

            /// <summary>
            /// Store connection
            /// </summary>
            private static DbConnection DbConnection { get; set; }

            /// <summary>
            /// Get connection
            /// </summary>
            /// <returns></returns>
            public static DbConnection Get()
            {
                if (DbConnection == null) throw new Exception("Connection not configured");
                if (DbConnection.State != ConnectionState.Open)
                    DbConnection.Open();
                return DbConnection;
            }

            /// <summary>
            /// Set connection
            /// </summary>
            /// <typeparam name="TProvider"></typeparam>
            /// <param name="connection"></param>
            public static void SetConnection<TProvider>(TProvider connection)
                where TProvider : DbConnection
            {
                if (DbConnection == null)
                    DbConnection = connection;
            }
        }
    }
}
