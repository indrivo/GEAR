using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ST.BaseBusinessRepository;
using ST.Entities.Data;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Entities.ViewModels.Table;
using ST.Files.Abstraction;

namespace ST.Files.Providers
{
    /// <inheritdoc />
    public class DocumentProvider : IFileProvider
    {
        private readonly FileConfig _config;

        /// <summary>
        ///     Document provider
        /// </summary>
        /// <param name="config"></param>
        public DocumentProvider(FileConfig config)
        {
            _config = config;
            _config.Tables = new Dictionary<string, TableConfigViewModel>
            {
                {"Files", new TableConfigViewModel {Name = "Files", Schema = "systemcore"}},
                {"FileReferences", new TableConfigViewModel {Name = "FileReferences", Schema = "systemcore"}},
                {"EntityFiles", new TableConfigViewModel {Name = "EntityFiles", Schema = "systemcore"}}
            };
        }

        /// <summary>
        ///     Delete files
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> DeleteFiles(EntityViewModel model)
        {
            var commitTransaction = true;
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };

            var entFileModel = new EntityViewModel
            {
                TableSchema = _config.Tables["EntityFiles"].Schema,
                TableName = _config.Tables["EntityFiles"].Name,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "FileId"},
                    new EntityFieldsViewModel {ColumnName = "FileReferenceId"}
                }
            };

            var entFileResult = _config.DbContext.ListEntitiesByParams(entFileModel);

            if (!entFileResult.IsSuccess || entFileResult.Result.Values == null ||
                entFileResult.Result.Values.Count == 0) return returnModel;

            using (var dbContextTransaction = _config.DbContext.Database.BeginTransaction())
            {
                var fileModel = new EntityViewModel
                {
                    TableSchema = _config.Tables["Files"].Schema,
                    TableName = _config.Tables["Files"].Name,
                    Values = entFileResult.Result.Values.Select(s => new Dictionary<string, object>
                    {
                        {"Id", s["FileId"]}
                    }).ToList(),
                    Fields = model.Fields
                };
                try
                {
                    var fileModelResult = _config.DbContext.ListEntitiesByParams(fileModel);
                    if (fileModelResult.IsSuccess && fileModelResult.Result.Values != null)
                    {
                        foreach (var value in fileModelResult.Result.Values)
                        {
                            var fileUri = new Uri(NormalizePath(value["Uri"].ToString()));
                            if (File.Exists(fileUri.LocalPath)) File.Delete(fileUri.LocalPath);
                        }

                        _config.DbContext.DeleteById(new EntityViewModel
                        {
                            TableSchema = _config.Tables["EntityFiles"].Schema,
                            TableName = _config.Tables["EntityFiles"].Name,
                            Values = entFileResult.Result.Values.Select(s => new Dictionary<string, object>
                            {
                                {"Id", s["Id"]}
                            }).ToList(),
                            Fields = new List<EntityFieldsViewModel>
                            {
                                new EntityFieldsViewModel {ColumnName = "Id"}
                            }
                        }, true);

                        _config.DbContext.DeleteById(new EntityViewModel
                        {
                            TableSchema = _config.Tables["Files"].Schema,
                            TableName = _config.Tables["Files"].Name,
                            Values = fileModelResult.Result.Values.Select(s => new Dictionary<string, object>
                            {
                                {"Id", s["Id"]}
                            }).ToList(),
                            Fields = new List<EntityFieldsViewModel>
                            {
                                new EntityFieldsViewModel {ColumnName = "Id"}
                            }
                        }, true);
                    }
                }
                catch (Exception ex)
                {
                    commitTransaction = false;
                    returnModel.Errors.Add(new ErrorModel(ExceptionCodes.UnhandledException, ex.Message));
                }


                if (commitTransaction)
                {
                    returnModel.IsSuccess = true;
                    dbContextTransaction.Commit();
                }
                else
                {
                    returnModel.IsSuccess = false;
                    dbContextTransaction.Rollback();
                }
            }

            return returnModel;
        }

        /// <summary>
        ///     Add file
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> AddFile(EntityViewModel model, Guid fileRef)
        {
            var commitTransaction = true;
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };

            var fileRefModel = new EntityViewModel
            {
                TableSchema = _config.Tables["FileReferences"].Schema,
                TableName = _config.Tables["FileReferences"].Name,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"}
                }
            };

            var fileRefResult = _config.DbContext.GetEntityById(fileRefModel, fileRef);

            using (var dbContextTransaction = _config.DbContext.Database.BeginTransaction())
            {
                if (fileRefResult.IsSuccess)
                {
                    var fileColumnName = model.Fields.FirstOrDefault(k => k.Type == "File")?.ColumnName;

                    var fieldInfo = _config.DbContext.TableFields
                        .Where(s => s.Name == fileColumnName && s.Table.Name == model.TableName)
                        .Include(s => s.TableFieldConfigValues).ThenInclude(s => s.TableFieldConfig).FirstOrDefault();
                    if (fieldInfo != null)
                    {
                        var fieldConfigs = fieldInfo.TableFieldConfigValues.Select(s => s.TableFieldConfig).ToList();
                    }

                    if (fieldInfo != null)
                    {
                        var fileUri = fieldInfo.TableFieldConfigValues
                            .FirstOrDefault(s => s.TableFieldConfig.Name == "Uri")?.Value;


                        returnModel.Result = fileRefResult.Result;

                        foreach (var value in model.Values)
                            try
                            {
                                if (!value.ContainsKey("Id")) value.Add("Id", Guid.NewGuid());

                                if (!value.ContainsKey("Created")) value.Add("Created", DateTime.Now);

                                var fileContent = (byte[])value["Content"];

                                var fileHash = CalculateMD5(fileContent);

                                var fileExtension = Path.GetExtension(value["Name"].ToString());

                                var fileSize = fileContent.LongLength;

                                var exactPath = IsFullPath(fileUri)
                                    ? new Uri(NormalizePath(
                                        string.Format(@"{0}/{1}/{2}", fileUri, fileRef, value["Id"])))
                                    : new Uri(NormalizePath(
                                        $@"{_config.WebRootPath}/{fileUri}/{fileRef}/{value["Id"]}"));

                                var fileFullPath =
                                    NormalizePath(string.Format(@"{0}/{1}", exactPath.LocalPath, value["Name"]));

                                var absoluteUri = new Uri(fileFullPath).AbsoluteUri;

                                var fileModel = new EntityViewModel
                                {
                                    TableSchema = _config.Tables["Files"].Schema,
                                    TableName = _config.Tables["Files"].Name,
                                    Values = new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            {"Id", value["Id"]},
                                            {"Author", value["Author"]},
                                            {"Created", value["Created"]},
                                            {"Name", value["Name"]},
                                            {"Description", value["Description"]},
                                            {"Extension", fileExtension},
                                            {"Uri", absoluteUri},
                                            {"Size", fileSize},
                                            {"Hash", fileHash}
                                        }
                                    },
                                    Fields = new List<EntityFieldsViewModel>
                                    {
                                        new EntityFieldsViewModel {ColumnName = "Id"},
                                        new EntityFieldsViewModel {ColumnName = "Author"},
                                        new EntityFieldsViewModel {ColumnName = "Created"},
                                        new EntityFieldsViewModel {ColumnName = "Name"},
                                        new EntityFieldsViewModel {ColumnName = "Description"},
                                        new EntityFieldsViewModel {ColumnName = "Extension"},
                                        new EntityFieldsViewModel {ColumnName = "Uri"},
                                        new EntityFieldsViewModel {ColumnName = "Size"},
                                        new EntityFieldsViewModel {ColumnName = "Hash"}
                                    }
                                };

                                var fileResult = _config.DbContext.Insert(fileModel);
                                if (!fileResult.IsSuccess) continue;
                                var entFileModel = new EntityViewModel
                                {
                                    TableSchema = _config.Tables["EntityFiles"].Schema,
                                    TableName = _config.Tables["EntityFiles"].Name,
                                    Values = new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            {"Id", Guid.NewGuid()},
                                            {"Author", value["Author"]},
                                            {"Created", value["Created"]},
                                            {"FileId", fileResult.Result},
                                            {"FileReferenceId", fileRef}
                                        }
                                    },
                                    Fields = new List<EntityFieldsViewModel>
                                    {
                                        new EntityFieldsViewModel {ColumnName = "Id"},
                                        new EntityFieldsViewModel {ColumnName = "Author"},
                                        new EntityFieldsViewModel {ColumnName = "Created"},
                                        new EntityFieldsViewModel {ColumnName = "FileId"},
                                        new EntityFieldsViewModel {ColumnName = "FileReferenceId"}
                                    }
                                };

                                var entFileResult = _config.DbContext.Insert(entFileModel);

                                if (!entFileResult.IsSuccess)
                                {
                                    commitTransaction = false;
                                }
                                else
                                {
                                    if (!Directory.Exists(exactPath.LocalPath))
                                        Directory.CreateDirectory(exactPath.LocalPath);
                                    File.WriteAllBytes(fileFullPath, fileContent);
                                }
                            }
                            catch (Exception ex)
                            {
                                commitTransaction = false;
                                returnModel.Errors.Add(new ErrorModel(ExceptionCodes.UnhandledException, ex.Message));
                            }
                    }
                }
                else
                {
                    commitTransaction = false;
                }

                if (commitTransaction)
                {
                    returnModel.IsSuccess = true;
                    dbContextTransaction.Commit();
                }
                else
                {
                    returnModel.IsSuccess = false;
                    dbContextTransaction.Rollback();
                }
            }

            return returnModel;
        }

        /// <summary>
        ///     Get file content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> GetFileContent(EntityViewModel model)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };

            var entFileModel = new EntityViewModel
            {
                TableSchema = _config.Tables["EntityFiles"].Schema,
                TableName = _config.Tables["EntityFiles"].Name,
                Values = model.Values,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "Author"},
                    new EntityFieldsViewModel {ColumnName = "Created"},
                    new EntityFieldsViewModel {ColumnName = "FileId"},
                    new EntityFieldsViewModel {ColumnName = "FileReferenceId"}
                }
            };

            var entFileModelResult = _config.DbContext.ListEntitiesByParams(entFileModel);

            if (!entFileModelResult.IsSuccess) return returnModel;
            var fileModel = new EntityViewModel
            {
                TableSchema = _config.Tables["Files"].Schema,
                TableName = _config.Tables["Files"].Name,
                Values = entFileModelResult.Result.Values.Select(s => new Dictionary<string, object>
                {
                    {"Id", s["FileId"]}
                }).ToList(),
                Fields = model.Fields
            };
            var fileModelResult = _config.DbContext.ListEntitiesByParams(fileModel);

            foreach (var value in fileModelResult.Result.Values)
            {
                var exactPath = new Uri(value["Uri"].ToString());
                var array = File.ReadAllBytes(exactPath.LocalPath);
                value.Add("Content", array);
            }

            returnModel = fileModelResult;

            return returnModel;
        }

        /// <summary>
        ///     List files by Key
        /// </summary>
        /// <param name="model"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> ListFilesByKey(EntityViewModel model, Guid key)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };

            var entFileModel = new EntityViewModel
            {
                TableSchema = _config.Tables["EntityFiles"].Schema,
                TableName = _config.Tables["EntityFiles"].Name,
                Values = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> {{"FileReferenceId", key}}
                },
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "Author"},
                    new EntityFieldsViewModel {ColumnName = "Created"},
                    new EntityFieldsViewModel {ColumnName = "FileId"},
                    new EntityFieldsViewModel {ColumnName = "FileReferenceId"}
                }
            };

            var entFileModelResult = _config.DbContext.ListEntitiesByParams(entFileModel);

            if (!entFileModelResult.IsSuccess) return returnModel;
            var fileModel = new EntityViewModel
            {
                TableSchema = _config.Tables["Files"].Schema,
                TableName = _config.Tables["Files"].Name,
                Values = entFileModelResult.Result.Values.Select(s => new Dictionary<string, object>
                {
                    {"Id", s["FileId"]}
                }).ToList(),
                Fields = model.Fields
            };
            var fileModelResult = _config.DbContext.ListEntitiesByParams(fileModel);

            returnModel = fileModelResult;
            return returnModel;
        }

        /// <summary>
        ///     Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<Guid> Create(EntityViewModel model)
        {
            var returnModel = new ResultModel<Guid>
            {
                IsSuccess = false,
                Result = Guid.Empty
            };

            var commitTransaction = true;

            var fileColumnName = model.Fields.FirstOrDefault(k => k.Type == "File")?.ColumnName;

            var fieldInfo = _config.DbContext.TableFields
                .Where(s => s.Name == fileColumnName && s.Table.Name == model.TableName)
                .Include(s => s.TableFieldConfigValues).ThenInclude(s => s.TableFieldConfig).FirstOrDefault();
            if (fieldInfo != null)
            {
                var fieldConfigs = fieldInfo.TableFieldConfigValues.Select(s => s.TableFieldConfig).ToList();
            }

            if (fieldInfo == null) return returnModel;
            {
                var fileUri = fieldInfo.TableFieldConfigValues.FirstOrDefault(s => s.TableFieldConfig.Name == "Uri")
                    ?.Value;


                using (var dbContextTransaction = _config.DbContext.Database.BeginTransaction())
                {
                    var fileRefModel = new EntityViewModel
                    {
                        TableSchema = _config.Tables["FileReferences"].Schema,
                        TableName = _config.Tables["FileReferences"].Name,
                        Values = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object>
                            {
                                {"Id", Guid.NewGuid()}
                            }
                        },
                        Fields = new List<EntityFieldsViewModel>
                        {
                            new EntityFieldsViewModel {ColumnName = "Id"}
                        }
                    };

                    var fileRefResult = _config.DbContext.Insert(fileRefModel);

                    if (fileRefResult.IsSuccess)
                    {
                        returnModel.Result = fileRefResult.Result;

                        foreach (var value in model.Values)
                            try
                            {
                                if (!value.ContainsKey("Id")) value.Add("Id", Guid.NewGuid());

                                if (!value.ContainsKey("Created")) value.Add("Created", DateTime.Now);

                                var fileContent = (byte[])value["Content"];

                                var fileHash = CalculateMD5(fileContent);

                                var fileExtension = Path.GetExtension(value["Name"].ToString());

                                var fileSize = fileContent.LongLength;

                                var exactPath = IsFullPath(fileUri)
                                    ? new Uri(NormalizePath(string.Format(@"{0}/{1}/{2}", fileUri, fileRefResult.Result,
                                        value["Id"])))
                                    : new Uri(NormalizePath(
                                        $@"{_config.WebRootPath}/{fileUri}/{fileRefResult.Result}/{value["Id"]}"));

                                var fileFullPath =
                                    NormalizePath(string.Format(@"{0}/{1}", exactPath.LocalPath, value["Name"]));

                                var absoluteUri = new Uri(fileFullPath).AbsoluteUri;

                                var fileModel = new EntityViewModel
                                {
                                    TableSchema = _config.Tables["Files"].Schema,
                                    TableName = _config.Tables["Files"].Name,
                                    Values = new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            {"Id", value["Id"]},
                                            {"Author", value["Author"]},
                                            {"Created", value["Created"]},
                                            {"Name", value["Name"]},
                                            {"Description", value["Description"]},
                                            {"Extension", fileExtension},
                                            {"Uri", absoluteUri},
                                            {"Size", fileSize},
                                            {"Hash", fileHash}
                                        }
                                    },
                                    Fields = new List<EntityFieldsViewModel>
                                    {
                                        new EntityFieldsViewModel {ColumnName = "Id"},
                                        new EntityFieldsViewModel {ColumnName = "Author"},
                                        new EntityFieldsViewModel {ColumnName = "Created"},
                                        new EntityFieldsViewModel {ColumnName = "Name"},
                                        new EntityFieldsViewModel {ColumnName = "Description"},
                                        new EntityFieldsViewModel {ColumnName = "Extension"},
                                        new EntityFieldsViewModel {ColumnName = "Uri"},
                                        new EntityFieldsViewModel {ColumnName = "Size"},
                                        new EntityFieldsViewModel {ColumnName = "Hash"}
                                    }
                                };

                                var fileResult = _config.DbContext.Insert(fileModel);
                                if (!fileResult.IsSuccess) continue;
                                var entFileModel = new EntityViewModel
                                {
                                    TableSchema = _config.Tables["EntityFiles"].Schema,
                                    TableName = _config.Tables["EntityFiles"].Name,
                                    Values = new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            {"Id", Guid.NewGuid()},
                                            {"Author", value["Author"]},
                                            {"Created", value["Created"]},
                                            {"FileId", fileResult.Result},
                                            {"FileReferenceId", fileRefResult.Result}
                                        }
                                    },
                                    Fields = new List<EntityFieldsViewModel>
                                    {
                                        new EntityFieldsViewModel {ColumnName = "Id"},
                                        new EntityFieldsViewModel {ColumnName = "Author"},
                                        new EntityFieldsViewModel {ColumnName = "Created"},
                                        new EntityFieldsViewModel {ColumnName = "FileId"},
                                        new EntityFieldsViewModel {ColumnName = "FileReferenceId"}
                                    }
                                };

                                var entFileResult = _config.DbContext.Insert(entFileModel);

                                if (!entFileResult.IsSuccess)
                                {
                                    commitTransaction = false;
                                }
                                else
                                {
                                    if (!Directory.Exists(exactPath.LocalPath))
                                        Directory.CreateDirectory(exactPath.LocalPath);
                                    File.WriteAllBytes(fileFullPath, fileContent);
                                }
                            }
                            catch (Exception ex)
                            {
                                commitTransaction = false;
                                returnModel.Errors.Add(new ErrorModel(ExceptionCodes.UnhandledException, ex.Message));
                            }
                    }
                    else
                    {
                        commitTransaction = false;
                    }

                    if (commitTransaction)
                    {
                        returnModel.IsSuccess = true;
                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        returnModel.IsSuccess = false;
                        dbContextTransaction.Rollback();
                    }
                }
            }

            return returnModel;
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 ||
                !Path.IsPathRooted(path))
                return false;

            var pathRoot = Path.GetPathRoot(path);
            if (pathRoot.Length <= 2 && pathRoot != "/"
            ) // Accepts X:\ and \\UNC\PATH, rejects empty string, \ and X:, but accepts / to support Linux
                return false;

            return !(pathRoot == path && pathRoot.StartsWith("\\\\", StringComparison.Ordinal) &&
                     pathRoot.IndexOf('\\', 2) == -1
                ); // A UNC server name without a share name (e.g "\\NAME") is invalid
        }

        /// <summary>
        ///     Calculate MD5
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string CalculateMD5(byte[] content)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(content)).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        ///     Normalize Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}