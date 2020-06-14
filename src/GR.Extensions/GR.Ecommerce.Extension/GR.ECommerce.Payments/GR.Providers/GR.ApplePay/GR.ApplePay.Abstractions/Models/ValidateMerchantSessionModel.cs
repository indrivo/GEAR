using System.ComponentModel.DataAnnotations;

namespace GR.ApplePay.Abstractions.Models
{
    public class ValidateMerchantSessionModel
    {
        [DataType(DataType.Url)]
        [Required]
        public string ValidationUrl { get; set; }
    }
}
