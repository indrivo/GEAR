using GR.Identity.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace GR.UserPreferences.Abstractions.Models
{
    public class UserPreference
    {
        /// <summary>
        /// User id
        /// </summary>
        public virtual GearUser User { get; set; }
        [Required]
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Preference key
        /// </summary>
        [Required]
        public virtual string Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public virtual string Value { get; set; }
    }
}