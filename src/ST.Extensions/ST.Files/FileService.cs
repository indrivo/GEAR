using Microsoft.AspNetCore.Http;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using ST.Files.Abstraction.ViewModels;
using ST.Files.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public virtual ResultModel<Guid> AddFile(FileViewModel dto)
        {
            try
            {
                if (dto.Id == Guid.Empty) return UpdateFile(dto);
                var encryptedFile = EncryptFile(dto.File);
                var file = new File
                {
                    Description = dto.Description,
                    FileExtension = encryptedFile.FileExtension,
                    Hash = encryptedFile.EncryptedFile,
                    Name = dto.Name,
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

        public virtual ResultModel<FileViewModel> GetFileById(Guid id)
        {
            try
            {
                var dbFileResult = _context.Files.FirstOrDefault(x => x.Id == id & x.IsDeleted == false);
                var decryptedFile = DecryptFile(dbFileResult?.Hash);
                var dto = new FileViewModel();
                if (dbFileResult != null)
                {
                    dto.Description = dbFileResult.Description;
                    dto.FileExtension = dbFileResult.FileExtension;
                    dto.File = decryptedFile;
                    dto.Id = dbFileResult.Id;
                    dto.Name = dbFileResult.Name;
                    dto.Size = dbFileResult.Size;
                }
                return new ResultModel<FileViewModel>
                {
                    IsSuccess = true,
                    Result = dto
                };
            }
            catch (Exception exceptionMessage)
            {
                return ReturnErrorModel<FileViewModel>(exceptionMessage.ToString());
            }
        }
        public virtual ResultModel<List<FileViewModel>> GetFilesByIds(List<Guid> idsList)
        {
            try
            {
                var dbFileResult = _context.Files
                    .Where(l => idsList.Contains(l.Id))
                    .ToList();
                var dto = new List<FileViewModel>();
                if (dbFileResult.Count > 0)
                {
                    foreach (var item in dbFileResult)
                    {
                        var decryptedFile = DecryptFile(item.Hash);
                        var dtoItem = new FileViewModel
                        {
                            Description = item.Description,
                            FileExtension = item.FileExtension,
                            File = decryptedFile,
                            Id = item.Id,
                            Name = item.Name,
                            Size = item.Size
                        };
                        dto.Add(dtoItem);
                    }
                }
                return new ResultModel<List<FileViewModel>>
                {
                    IsSuccess = true,
                    Result = dto
                };
            }
            catch (Exception exceptionMessage)
            {
                return ReturnErrorModel<List<FileViewModel>>(exceptionMessage.ToString());
            }
        }



        private static IFormFile DecryptFile(byte[] file)
        {
            return null;
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
                       EncryptedFile = data
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

        private ResultModel<Guid> UpdateFile(FileViewModel dto)
        {
            try
            {
                var encryptedFile = EncryptFile(dto.File);
                var file = new File
                {
                    Description = dto.Description,
                    FileExtension = encryptedFile.FileExtension,
                    Hash = encryptedFile.EncryptedFile,
                    Name = dto.Name,
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