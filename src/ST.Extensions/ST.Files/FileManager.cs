using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using System;
using System.IO;
using System.Linq;
using ST.Files.Abstraction.Helpers;
using ST.Files.Abstraction.Models;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Data;
using ST.Files.Models;


namespace ST.Files
{
    public class FileManager<TContext> : IFileManager where TContext : FileDbContext, IFileContext
    {
        private readonly TContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public FileManager(TContext context)
        {
            _context = context;
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
            var dbFileResult = _context.Files.FirstOrDefault(x => (x.Id == id) & (x.IsDeleted == false));
            var dto = new DownloadFileViewModel();
            if (dbFileResult == null) return ExceptionHandler.ReturnErrorModel<DownloadFileViewModel>(ExceptionMessagesEnum.FileNotFound);

            dto.EncryptedFile = dbFileResult.Hash;
            dto.FileExtension = dbFileResult.FileExtension;
            dto.FileName = dbFileResult.Name;

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
    }
}