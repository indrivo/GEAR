namespace GR.Documents.Abstractions.Helpers
{
    public struct ErrorKey
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public struct ErrorKeys
    {
        /// <summary>
        /// Invalid model
        /// </summary>
        public static ErrorKey InvalidModel = new ErrorKey
        {
            Key = "document_0001",
            Value = "Invalid model provided in post"
        };
    }
}
