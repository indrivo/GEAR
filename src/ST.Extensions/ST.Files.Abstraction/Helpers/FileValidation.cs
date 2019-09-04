using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using ST.Files.Abstraction.Models.ViewModels;

namespace ST.Files.Abstraction.Helpers
{
    public static class FileValidation
    {
        public static ResultModel<Guid> ValidateFile(IFormFile file, FileSettingsViewModel settings)
        {
            var result = new ResultModel<Guid> {IsSuccess = true};

            if (file.Length <= 0)
                return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            if (settings?.Extensions.Contains(Path.GetExtension(file.FileName)) == false)
                return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.InvalidExtension);

            return settings != null && file.Length < settings.MaxSize * 1024 ? ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.InvalidExtension) : result;
        }
    }
}
