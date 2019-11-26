using GR.ECommerce.Abstractions.Enums;

namespace GR.ECommerce.Abstractions.Models.Settings
{
    public class CommerceSetting
    {
        /// <summary>
        /// object key
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Serialized value
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Value object type
        /// </summary>
        public virtual CommerceSettingType Type { get; set; } = CommerceSettingType.Text;
    }
}