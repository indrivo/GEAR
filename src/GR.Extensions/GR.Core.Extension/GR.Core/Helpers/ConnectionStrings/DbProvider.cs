namespace GR.Core.Helpers.ConnectionStrings
{
    public enum DbProviderType
    {
        MsSqlServer, PostgreSql
    }

    public static class DbProvider
    {
        public const string SqlServer = "Microsoft.EntityFrameworkCore.SqlServer";
        public const string Sqlite = "Microsoft.EntityFrameworkCore.Sqlite";
        public const string InMemory = "Microsoft.EntityFrameworkCore.InMemory";
        public const string Cosmos = "Microsoft.EntityFrameworkCore.Cosmos";
        public const string PostgreSQL = "Npgsql.EntityFrameworkCore.PostgreSQL";
        public const string MySql = "Pomelo.EntityFrameworkCore.MySql";
        public const string MyCat = "Pomelo.EntityFrameworkCore.MyCat";
        public const string SqlServerCompact40 = "EntityFrameworkCore.SqlServerCompact40";
        public const string SqlServerCompact35 = "EntityFrameworkCore.SqlServerCompact35";
        public const string Jet = "EntityFrameworkCore.Jet";
        public const string MySqlData = "MySql.Data.EntityFrameworkCore";
        public const string Firebird = "FirebirdSql.EntityFrameworkCore.Firebird";
        public const string FirebirdSql = "EntityFrameworkCore.FirebirdSql";
        public const string IBM = "IBM.EntityFrameworkCore";
        public const string OpenEdge = "EntityFrameworkCore.OpenEdge";
    }
}
