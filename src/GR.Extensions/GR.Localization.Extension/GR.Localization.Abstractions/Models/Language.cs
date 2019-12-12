using System.Globalization;

namespace GR.Localization.Abstractions.Models
{
	/// <summary>
	/// Represents a language that is used in the system
	/// for localization.
	/// </summary>
	public class Language
    {
        /// <summary>
        /// The identifier of the language that is
        /// represented by <see cref="CultureInfo.TwoLetterISOLanguageName"/>
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Friendly name of the language used.
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Identifier ?? "Null identifier" } - {Name ?? "Null name"}";
        }
    }
}
