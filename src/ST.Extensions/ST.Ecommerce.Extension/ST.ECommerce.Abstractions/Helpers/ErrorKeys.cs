namespace ST.ECommerce.Abstractions.Helpers
{
    public struct ErrorKey
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public struct CommerceErrorKeys
    {
        /// <summary>
        /// Invalid model
        /// </summary>
        public static ErrorKey InvalidModel = new ErrorKey
        {
            Key = "Commerce_0001",
            Value = "Invalid model provided in post"
        };
    }
}