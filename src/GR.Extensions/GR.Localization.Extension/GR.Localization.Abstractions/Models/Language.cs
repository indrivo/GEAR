using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;

namespace GR.Localization.Abstractions.Models
{
    /// <summary>
    /// Represents a language that is used in the system
    /// for localization.
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [DebuggerDisplay("{" + nameof(Identifier) + "}", Name = "{Name}")]
    public class Language : BaseModel
    {
        /// <summary>
        /// The identifier of the language that is
        /// represented by <see cref="CultureInfo.TwoLetterISOLanguageName"/>
        /// </summary>
        [Required]
        [StringLength(2)]
        public virtual string Identifier { get; set; }

        /// <summary>
        /// Friendly name of the language used.
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Translations
        /// </summary>
        public virtual ICollection<TranslationItem> TranslationItems { get; set; } = new List<TranslationItem>();

        /// <summary>
        /// Show language
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Identifier ?? "Null identifier" } - {Name ?? "Null name"}";
        }
    }
}
