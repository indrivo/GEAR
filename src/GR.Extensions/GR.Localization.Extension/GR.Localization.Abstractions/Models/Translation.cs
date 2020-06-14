using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Localization.Abstractions.Models
{
    public class Translation : BaseModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Required]
        public virtual string Key { get; set; }

        /// <summary>
        /// Translations
        /// </summary>
        public virtual ICollection<TranslationItem> Translations { get; set; }
    }
}
