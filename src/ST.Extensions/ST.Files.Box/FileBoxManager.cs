using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using ST.Files.Abstraction.Helpers;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Box.Abstraction;
using ST.Files.Box.Abstraction.Models;
using ST.Files.Box.Data;
using ST.Files.Box.Models;


namespace ST.Files.Box
{
    public class FileBoxManager<TContext> : IFileBoxManager where TContext : FileBoxDbContext, IFileBoxContext
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly TContext _context;
        private const string FileRootPath = "FileBox";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hostingEnvironment"></param>
        public FileBoxManager(TContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public virtual ResultModel<Guid> AddFile(UploadFileViewModel dto)
        {
            if (dto.Id != Guid.Empty) return UpdateFile(dto);

            var encryptedFile = SaveFilePhysical(dto.File);
            if (encryptedFile == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.NullIFormFile);

            var file = new FileBox
            {
                FileExtension = encryptedFile.FileExtension,
                Path = encryptedFile.Path,
                Name = encryptedFile.FileName,
                Size = encryptedFile.Size
            };
            _context.FilesBox.Add(file);
            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }

        public virtual ResultModel<Guid> DeleteFile(Guid id)
        {
            var file = _context.FilesBox.FirstOrDefault(x => x.Id == id);
            if (file == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            file.IsDeleted = true;
            _context.FilesBox.Update(file);
            _context.SaveChanges();

            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<Guid> RestoreFile(Guid id)
        {
            var file = _context.FilesBox.FirstOrDefault(x => x.Id == id);
            if (file == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            file.IsDeleted = false;
            _context.FilesBox.Update(file);
            _context.SaveChanges();


            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<Guid> DeleteFilePermanent(Guid id)
        {
            var file = _context.FilesBox.FirstOrDefault(x => x.Id == id);
            if (file != null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, FileRootPath, file.Path, file.Name);
            File.Delete(filePath);
            _context.FilesBox.Remove(file);

            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<DownloadFileViewModel> GetFileById(Guid id)
        {
            var dbFileResult = _context.FilesBox.FirstOrDefault(x => (x.Id == id) & (x.IsDeleted == false));
            var dto = new DownloadFileViewModel();
            if (dbFileResult == null)
                return new ResultModel<DownloadFileViewModel>
                {
                    IsSuccess = false,
                    Result = dto
                };

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, FileRootPath, dbFileResult.Path);
            dto.Path = filePath;
            dto.FileExtension = dbFileResult.FileExtension;
            dto.FileName = dbFileResult.Name;

            return new ResultModel<DownloadFileViewModel>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        private FileBoxDto SaveFilePhysical(IFormFile file)
        {
            if (file.Length <= 0) return null;

            var directory = Path.Combine(_hostingEnvironment.WebRootPath, FileRootPath, DateTime.Now.ToLongDateString());
            var exists = Directory.Exists(directory);
            if (!exists) Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var response = new FileBoxDto
            {
                FileExtension = file.ContentType,
                Path = DateTime.Now.ToLongDateString(),
                FileName = file.FileName,
                Size = file.Length
            };
            return response;
        }

        private ResultModel<Guid> UpdateFile(UploadFileViewModel dto)
        {
            var file = _context.FilesBox.FirstOrDefault(x => x.Id == dto.Id);
            if (file == null)
                return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.FileNotFound);

            DeleteFilePhysical(file.Path, file.Name);
            var encryptedFile = SaveFilePhysical(dto.File);
            if (encryptedFile == null) return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.NullIFormFile);

            file.FileExtension = encryptedFile.FileExtension;
            file.Path = encryptedFile.Path;
            file.Name = encryptedFile.FileName;
            file.Size = encryptedFile.Size;
            _context.FilesBox.Update(file);
            _context.SaveChanges();

            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }

        private void DeleteFilePhysical(string path, string fileName)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, FileRootPath, path, fileName);
            File.Delete(filePath);
        }
    }
}
