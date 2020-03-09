using System.Collections.Generic;

namespace GR.Localization.Abstractions.Models
{
    public class Translation
    {
        /// <summary>
        /// Key
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Translations
        /// </summary>
        public virtual ICollection<TranslationItem> Translations { get; set; }
    }
}
