﻿using System;
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
        public static ResultModel<Guid> ValidateFile<TFileSettingsViewModel>(IFormFile file, TFileSettingsViewModel settings) where TFileSettingsViewModel : FileSettingsViewModel
        {
            var result = new ResultModel<Guid> {IsSuccess = true};

            if (file.Length <= 0)
                return ExceptionMessagesEnum.FileNotFound.ToErrorModel<Guid>();

            if (settings?.Extensions.Contains(Path.GetExtension(file.FileName)) == false)
                return ExceptionMessagesEnum.InvalidExtension.ToErrorModel<Guid>();

            return settings != null && file.Length < settings.MaxSize * 1024 ? ExceptionMessagesEnum.InvalidExtension.ToErrorModel<Guid>() : result;
        }
    }
}
