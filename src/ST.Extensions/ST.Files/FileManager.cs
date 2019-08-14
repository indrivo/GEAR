using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ST.Files.Abstraction.Models;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Models;


namespace ST.Files
{
    public class FileManager<TContext> : IFileManager where TContext : DbContext, IFileContext
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
            var file = new FileStorage
            {
                FileExtension = encryptedFile.FileExtension,
                Hash = encryptedFile.EncryptedFile,
                Name = encryptedFile.FileName,
                Size = encryptedFile.Size
            };
            _context.Set<FileStorage>().Add(file);
            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }

        public virtual ResultModel<Guid> DeleteFile(Guid id)
        {
            var file = _context.Set<FileStorage>().FirstOrDefault(x => x.Id == id);
            if (file != null)
            {
                file.IsDeleted = true;
                _context.Set<FileStorage>().Update(file);
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

            var file = _context.Set<FileStorage>().FirstOrDefault(x => x.Id == id);
            if (file != null)
            {
                file.IsDeleted = false;
                _context.Set<FileStorage>().Update(file);
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
            var file = _context.Set<FileStorage>().FirstOrDefault(x => x.Id == id);
            if (file != null)
                _context.Set<FileStorage>().Remove(file);
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
            var dbFileResult = _context.Set<FileStorage>().FirstOrDefault(x => (x.Id == id) & (x.IsDeleted == false));
            var dto = new DownloadFileViewModel();
            if (dbFileResult == null)
                return new ResultModel<DownloadFileViewModel>
                {
                    IsSuccess = true,
                    Result = dto
                };

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
                file.CopyToAsync(memoryStream);
                var response = new FileStorageDto
                {
                    FileExtension = file.ContentType,
                    EncryptedFile = memoryStream.ToArray(),
                    FileName = file.FileName,
                    Size = file.Length
                };
                memoryStream.Close();
                return response;
            }
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
            var file = new FileStorage
            {
                FileExtension = encryptedFile.FileExtension,
                Hash = encryptedFile.EncryptedFile,
                Name = encryptedFile.FileName,
                Size = encryptedFile.Size
            };
            _context.Set<FileStorage>().Add(file);
            _context.SaveChanges();
            return new ResultModel<Guid>
            {
                IsSuccess = true,
                Result = file.Id
            };
        }
    }
}