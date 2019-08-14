using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ST.Files.Abstraction.Models;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Box.Models;


namespace ST.Files.Box
{
    public class FileBoxManager<TContext> : IFileBoxManager where TContext : DbContext, IFileContext
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

            var encryptedFile = EncryptFile(dto.File);
            var file = new FileBox
            {
                FileExtension = encryptedFile.FileExtension,
                Path = encryptedFile.Path,
                Name = encryptedFile.FileName,
                Size = encryptedFile.Size
            };
            _context.Set<FileBox>().Add(file);
            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }

        public virtual ResultModel<Guid> DeleteFile(Guid id)
        {
            var file = _context.Set<FileBox>().FirstOrDefault(x => x.Id == id);
            if (file != null)
            {
                file.IsDeleted = true;
                _context.Set<FileBox>().Update(file);
                _context.SaveChanges();
            }
            else
            {
                return ReturnErrorModel<Guid>("File not Found");
            }

            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<Guid> RestoreFile(Guid id)
        {

            var file = _context.Set<FileBox>().FirstOrDefault(x => x.Id == id);
            if (file != null)
            {
                file.IsDeleted = false;
                _context.Set<FileBox>().Update(file);
                _context.SaveChanges();
            }
            else
            {
                return ReturnErrorModel<Guid>("File not Found");
            }

            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<Guid> DeleteFilePermanent(Guid id)
        {
            var file = _context.Set<FileBox>().FirstOrDefault(x => x.Id == id);
            if (file != null)
                _context.Set<FileBox>().Remove(file);
            else
            {
                return ReturnErrorModel<Guid>("File not Found");
            }

            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true
            };
        }

        public virtual ResultModel<DownloadFileViewModel> GetFileById(Guid id)
        {
            var dbFileResult = _context.Set<FileBox>().FirstOrDefault(x => (x.Id == id) & (x.IsDeleted == false));
            var dto = new DownloadFileViewModel();
            if (dbFileResult == null)
                return new ResultModel<DownloadFileViewModel>
                {
                    IsSuccess = true,
                    Result = dto
                };

            dto.Path = dbFileResult.Path;
            dto.FileExtension = dbFileResult.FileExtension;
            dto.FileName = dbFileResult.Name;

            return new ResultModel<DownloadFileViewModel>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        private FileBoxDto EncryptFile(IFormFile file)
        {
            if (file.Length <= 0) return null;

            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, FileRootPath);
            var filePath = Path.Combine(uploads, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var response = new FileBoxDto
            {
                FileExtension = file.ContentType,
                Path = DateTime.Now.Month.ToString(),
                FileName = file.FileName,
                Size = file.Length
            };
            return response;
        }

        private static ResultModel<T> ReturnErrorModel<T>(object exceptionMessage)
        {
            return new ResultModel<T>
            {
                IsSuccess = false,
                Errors = new List<IErrorModel>
                {
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = "Failed to execute action!"
                    },
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = exceptionMessage.ToString()
                    }
                }
            };
        }

        private ResultModel<Guid> UpdateFile(UploadFileViewModel dto)
        {
            var encryptedFile = EncryptFile(dto.File);
            var file = new FileBox
            {
                FileExtension = encryptedFile.FileExtension,
                Path = encryptedFile.Path,
                Name = encryptedFile.FileName,
                Size = encryptedFile.Size
            };
            _context.Set<FileBox>().Add(file);
            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }
    }
}
