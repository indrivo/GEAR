using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using ST.Core.Abstractions;
using ST.Files.Abstraction.Helpers;
using ST.Files.Abstraction.Models;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Data;
using ST.Files.Models;


namespace ST.Files
{
    public class FileManager<TContext> : IFileManager where TContext : FileDbContext, IFileContext
    {
        private readonly IWritableOptions<List<FileSettingsViewModel>> _options;
        private readonly TContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="options"></param>
        public FileManager(TContext context, IWritableOptions<List<FileSettingsViewModel>> options)
        {
            _context = context;
            _options = options;
        }

        public virtual ResultModel<Guid> AddFile(UploadFileViewModel dto)
        {
            if (dto.Id != Guid.Empty) return UpdateFile(dto);

            var encryptedFile = EncryptFile(dto.File);
            if (encryptedFile == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.NullIFormFile);

            var file = new FileStorage
            {
                FileExtension = encryptedFile.FileExtension,
                Hash = encryptedFile.EncryptedFile,
                Name = encryptedFile.FileName,
                Size = encryptedFile.Size
            };
            _context.Files.Add(file);
            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }

        public virtual ResultModel<Guid> DeleteFile(Guid id)
        {
            if (id == Guid.Empty) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.NullParameter);

            var file = _context.Files.FirstOrDefault(x => x.Id == id);
            if (file == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            file.IsDeleted = true;
            _context.Files.Update(file);
            _context.SaveChanges();


            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }


        public virtual ResultModel<Guid> RestoreFile(Guid id)
        {
            if (id == Guid.Empty) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.NullParameter);

            var file = _context.Files.FirstOrDefault(x => x.Id == id);
            if (file == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            file.IsDeleted = false;
            _context.Files.Update(file);
            _context.SaveChanges();


            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<Guid> DeleteFilePermanent(Guid id)
        {
            if (id == Guid.Empty) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.NullParameter);

            var file = _context.Files.FirstOrDefault(x => x.Id == id);
            if (file == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            _context.Files.Remove(file);

            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<DownloadFileViewModel> GetFileById(Guid id)
        {
            if (id == Guid.Empty) return ExceptionHandler.ReturnErrorModel<DownloadFileViewModel>(ExceptionMessagesEnum.NullParameter);

            var dbFileResult = _context.Files.FirstOrDefault(x => (x.Id == id) & (x.IsDeleted == false));
            if (dbFileResult == null) return ExceptionHandler.ReturnErrorModel<DownloadFileViewModel>(ExceptionMessagesEnum.FileNotFound);

            var dto = new DownloadFileViewModel
            {
                EncryptedFile = dbFileResult.Hash,
                FileExtension = dbFileResult.FileExtension,
                FileName = dbFileResult.Name
            };

            return new ResultModel<DownloadFileViewModel>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        private static FileStorageDto EncryptFile(IFormFile file)
        {
            if (file.Length <= 0) return null;

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] array = new byte[memoryStream.Length];
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.Read(array, 0, array.Length);

                var response = new FileStorageDto
                {
                    FileExtension = file.ContentType,
                    EncryptedFile = array,
                    FileName = file.FileName,
                    Size = file.Length
                };
                memoryStream.Close();
                return response;
            }
        }

        private ResultModel<Guid> UpdateFile(UploadFileViewModel dto)
        {
            var file = _context.Files.FirstOrDefault(x => x.Id == dto.Id);
            if (file == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            var encryptedFile = EncryptFile(dto.File);
            if (encryptedFile == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.NullIFormFile);

            file.FileExtension = encryptedFile.FileExtension;
            file.Hash = encryptedFile.EncryptedFile;
            file.Name = encryptedFile.FileName;
            file.Size = encryptedFile.Size;
            _context.Files.Add(file);
            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }

        public virtual ResultModel ChangeSettings(FileSettingsViewModel newSettings)
        {
            var result = new ResultModel();
            var fileSettingsList = _options.Value ?? new List<FileSettingsViewModel>();
            var fileSettings = _options?.Value?.Find(x => x.TenantId == newSettings.TenantId);
            if (fileSettings == null)
            {
                fileSettingsList.Add(newSettings);
            }
            else
            {
                var index = fileSettingsList.FindIndex(m => m.TenantId == newSettings.TenantId);
                if (index >= 0)
                    fileSettingsList[index] = newSettings;
            }

            _options.Update(x => x = fileSettingsList);
            result.IsSuccess = true;
            return result;
        }
    }
}