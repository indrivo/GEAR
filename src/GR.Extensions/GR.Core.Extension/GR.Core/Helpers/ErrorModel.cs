using System;
using Newtonsoft.Json;

namespace GR.Core.Helpers
{
    public class ErrorModel : IErrorModel
    {
        private const string ToStringFormat = "{0}: {1}";

        /// <summary>
        /// The instance of <see cref="ErrorModel"/>
        /// </summary>
        public ErrorModel() { }

        /// <summary>
        /// The instance of <see cref="ErrorModel"/>
        /// </summary>
        public ErrorModel(string key, string message = null)
        {
            Key = key;

            if (!string.IsNullOrEmpty(message))
                Message = message.Trim();
        }

        /// <summary>
        /// Localization key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Detailed message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString() =>
            string.Format(ToStringFormat, Key, Message);

        /// <summary>
        /// Implicit operator
        /// </summary>
        public static implicit operator ErrorModel(string formatted)
        {
            try
            {
                string[] parts = formatted.Split(':');

                if (parts.Length < 2)
                    return new ErrorModel(ExceptionCodes.UnhandledException, formatted);

                if (parts.Length == 2)
                    return new ErrorModel(parts[0], parts[1]);

                return new ErrorModel(parts[0], formatted.Substring(parts[0].Length).TrimStart(':').Trim());
            }
            catch (Exception)
            {
                return new ErrorModel(ExceptionCodes.UnhandledException, formatted);
            }
        }
    }
}
