using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.IO;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace GR.Core.Attributes.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile file)) return ValidationResult.Success;
            var extension = Path.GetExtension(file.FileName);
            if (extension != null && !((IList) _extensions).Contains(extension.ToLower()))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"This file extension is not allowed!, allowed are: {_extensions.Join(", ")}";
        }
    }
}
