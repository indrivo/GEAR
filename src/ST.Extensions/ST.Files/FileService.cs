using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using ST.Files.Abstraction.ViewModels;
using ST.Files.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http.Internal;
using File = ST.Files.Abstraction.Models.File;

namespace ST.Files
{
    public class FileService<TContext> : IFileService where TContext : FileDbContext, IFileContext
    {
        private readonly TContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public FileService(TContext context)
        {
            _context = context;
        }

        public virtual ResultModel<Guid> AddFile(UploadFileViewModel dto)
        {
            try
            {
                if (dto.Id != Guid.Empty) return UpdateFile(dto);
                var encryptedFile = EncryptFile(dto.File);
                var file = new File
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
            catch (Exception exceptionMessage)
            {
                return ReturnErrorModel<Guid>(exceptionMessage.ToString());
            }
        }

        public virtual ResultModel<Guid> DeleteFile(Guid id)
        {
            try
            {
                var file = _context.Files.FirstOrDefault(x => x.Id == id);
                if (file != null)
                {
                    file.IsDeleted = true;
                    _context.Files.Update(file);
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
            catch (Exception exceptionMessage)
            {
                return ReturnErrorModel<Guid>(exceptionMessage.ToString());
            }
        }

        public virtual ResultModel<Guid> DeleteFilePermanent(Guid id)
        {
            try
            {
                var file = _context.Files.FirstOrDefault(x => x.Id == id);
                if (file != null)
                    _context.Files.Remove(file);
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
            catch (Exception exceptionMessage)
            {
                return ReturnErrorModel<Guid>(exceptionMessage.ToString());
            }
        }

        public virtual ResultModel<DownloadFileViewModel> GetFileById(Guid id)
        {
            try
            {
                var dbFileResult = _context.Files.FirstOrDefault(x => x.Id == id & x.IsDeleted == false);
                var dto = new DownloadFileViewModel();
                if (dbFileResult != null)
                {
                    dto.EncryptedFile = dbFileResult.Hash;
                    dto.FileExtension = dbFileResult.FileExtension;
                    dto.FileName = dbFileResult.Name;
                }
                return new ResultModel<DownloadFileViewModel>
                {
                    IsSuccess = true,
                    Result = dto
                };
            }
            catch (Exception exceptionMessage)
            {
                return ReturnErrorModel<DownloadFileViewModel>(exceptionMessage.ToString());
            }
        }

        private static FileDto EncryptFile(IFormFile file)
        {
            if (file.Length > 0)
            {
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                   file.CopyToAsync(stream);
                   var length = Convert.ToInt32(stream.Length);
                   var data = new byte[length];
                   stream.Read(data, 0, length);
                   stream.Close();
                   return new FileDto
                   {
                       FileExtension = file.ContentType,
                       EncryptedFile = data,
                       FileName = file.FileName,
                       Size = file.Length
                   };
                }
            }
            else
            {
                return null;
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
            try
            {
                var encryptedFile = EncryptFile(dto.File);
                var file = new File
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
            catch (Exception exceptionMessage)
            {
                return ReturnErrorModel<Guid>(exceptionMessage.ToString());
            }
        }
    }
}