using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Localization.Abstractions.Models
{
    public class TranslationItem : BaseModel
    {
        public virtual Language Language { get; set; }
        [Required]
        public virtual string Identifier { get; set; }

        public virtual string Value { get; set; }

        public virtual Translation Translation { get; set; }
        [Required]
        public virtual Guid TranslationId { get; set; }
    }
}