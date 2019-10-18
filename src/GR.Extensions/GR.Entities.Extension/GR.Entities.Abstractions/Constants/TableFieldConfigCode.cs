namespace GR.Entities.Abstractions.Constants
{
    public struct TableFieldConfigCode
    {
        public struct Decimal
        {
            public const string Precision = "0000";
            public const string Scale = "0001";
            public const string DefaultValue = "0002";
        }

        public struct Integer
        {
            public const string MaxValue = "0100";
            public const string DefaultValue = "0101";
        }
        public struct Text
        {
            public const string ContentLen = "1000";
            public const string DefaultValue = "1001";
        }

        public struct Character
        {
            public const string DefaultValue = "1100";
        }

        public struct Date
        {
            public const string DefaultValue = "2000";
        }

        public struct Time
        {
            public const string DefaultValue = "2100";
        }

        public struct DateTime
        {
            public const string DefaultValue = "2200";
        }

        public struct Reference
        {
            public const string ForeingTable = "3000";
            public const string ForeingSchemaTable = "9999";
            public const string DisplayFormat = "3001";
        }
    }
}
