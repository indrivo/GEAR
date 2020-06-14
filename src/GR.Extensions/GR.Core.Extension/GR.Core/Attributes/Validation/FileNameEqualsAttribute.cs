using System;
using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using Microsoft.AspNetCore.Http;

namespace GR.Core.Attributes.Validation
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class FileNameEqualsAttribute : ValidationAttribute
    {
        private readonly string _fileName;
        public FileNameEqualsAttribute(string fileName)
        {
            if (fileName.IsNullOrEmpty()) throw new Exception("Name is invalid");
            _fileName = fileName;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile file)) return ValidationResult.Success;
            return file.FileName != _fileName ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"The file must have the name: {_fileName}";
        }
    }
}
