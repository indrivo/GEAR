using System;
using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using Microsoft.AspNetCore.Http;

namespace GR.Core.Attributes.Validation
{
    [Author(Authors.LUPEI_NICOLAE)]
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile file)) return ValidationResult.Success;
            return file.Length > _maxFileSize ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is { _maxFileSize} bytes.";
        }
    }
}