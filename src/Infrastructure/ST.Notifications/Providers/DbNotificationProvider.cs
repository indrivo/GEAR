using System;
using System.Collections.Generic;
using System.Linq;
using ST.BaseBusinessRepository;
using ST.Entities.Controls.Builders;
using ST.Entities.Data;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Entities.ViewModels.Table;
using ST.Notifications.Abstraction;

namespace ST.Notifications.Providers
{
    /// <inheritdoc />
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DbNotificationProvider : INotificationProvider
    {
        private static readonly object LockObj = new object();
        private readonly DbNotificationConfig _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public DbNotificationProvider(DbNotificationConfig config)
        {
            _config = config;
            _config.Tables = new Dictionary<string, TableConfigViewModel>
            {
                {Tables.Email, new TableConfigViewModel {Name = Tables.Email, Schema = "indrivo"}},
                {
                    Tables.UserEmailFolders,
                    new TableConfigViewModel {Name =  Tables.UserEmailFolders, Schema = "indrivo"}
                },
                {Tables.EmailUsers, new TableConfigViewModel {Name = Tables.EmailUsers, Schema = "indrivo"}}
            };
        }

        /// <summary>
        ///     Restore from trash
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        public ResultModel<bool> RestoreFromTrash(IEnumerable<Guid> notifications)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };

            var resultSuccess = true;

            var users = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                HasConfig = true,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "UserEmailFolderId"},
                    new EntityFieldsViewModel {ColumnName = "Author"},
                    new EntityFieldsViewModel {ColumnName = "Type"}
                },
                Values = notifications.Select(s => new Dictionary<string, object> { { "Id", s } }).ToList()
            };

            var usersResult = _config.DbContext.ListEntitiesByParams(users);

            if (usersResult.IsSuccess && usersResult.Result.Values != null && usersResult.Result.Values.Count > 0)
            {
                var tableFolders = new EntityViewModel
                {
                    TableSchema = _config.Tables["UserEmailFolders"].Schema,
                    TableName = _config.Tables["UserEmailFolders"].Name,
                    HasConfig = false,
                    Fields = new List<EntityFieldsViewModel>
                    {
                        new EntityFieldsViewModel {ColumnName = "Id"},
                        new EntityFieldsViewModel {ColumnName = "Name"}
                    },
                    Values = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object> {{"Author", usersResult.Result.Values[0]["Author"]}}
                    }
                };

                var folders = _config.DbContext.ListEntitiesByParams(tableFolders);

                if (folders.IsSuccess && folders.Result.Values != null && folders.Result.Values.Count > 0)
                {
                    var sentFolderId = folders.Result.Values.FirstOrDefault(s => s["Name"].ToString() == "Sent")?["Id"];
                    var inboxFolderId =
                        folders.Result.Values.FirstOrDefault(s => s["Name"].ToString() == "Inbox")?["Id"];

                    try
                    {
                        foreach (var userNotification in usersResult.Result.Values)
                            if ((int)userNotification["Type"] == 0)
                            {
                                var resultResore = ChangeFolder(new EntityViewModel
                                {
                                    Values = new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            {"Id", userNotification["Id"]},
                                            {"UserEmailFolderId", sentFolderId}
                                        }
                                    }
                                });
                                if (!resultResore.IsSuccess) resultSuccess = false;
                            }
                            else if ((int)userNotification["Type"] == 1)
                            {
                                var resultResore = ChangeFolder(new EntityViewModel
                                {
                                    Values = new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object>
                                        {
                                            {"Id", userNotification["Id"]},
                                            {"UserEmailFolderId", inboxFolderId}
                                        }
                                    }
                                });
                                if (!resultResore.IsSuccess) resultSuccess = false;
                            }
                    }
                    catch (Exception ex)
                    {
                        returnModel.Errors.Add(new ErrorModel(ExceptionCodes.UnhandledException, ex.Message));
                        resultSuccess = false;
                    }
                }
            }

            returnModel.Result = resultSuccess;
            returnModel.IsSuccess = resultSuccess;

            return returnModel;
        }

        /// <summary>
        ///     Get unreaded notification
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultModel<int> GetUnreadNotifications(string userId)
        {
            var returnModel = new ResultModel<int>
            {
                IsSuccess = false
            };

            var tableUsers = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                HasConfig = true,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "UserEmailFolderId"}
                },
                Values = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> {{"Author", userId}, {"Type", 1}}
                }
            };

            var count = _config.DbContext.GetCountByParameter(tableUsers);

            if (!count.IsSuccess) return returnModel;
            returnModel.IsSuccess = true;
            returnModel.Result = 0;

            if (count.Result.Values != null && count.Result.Values.Count > 0)
                returnModel.Result = (int)count.Result.Values[0][""];
            return returnModel;
        }

        /// <summary>
        ///     Get user folders
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="withCount"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> GetUserFolders(string userId, bool withCount = false)
        {
            var refreshList = false;

            ResultModel<EntityViewModel> returnModel;

            var tableFolders = new EntityViewModel
            {
                TableSchema = _config.Tables["UserEmailFolders"].Schema,
                TableName = _config.Tables["UserEmailFolders"].Name,
                HasConfig = false,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "Author"},
                    new EntityFieldsViewModel {ColumnName = "IsSystem"},
                    new EntityFieldsViewModel {ColumnName = "Name"},
                    new EntityFieldsViewModel {ColumnName = "Description"},
                    new EntityFieldsViewModel {ColumnName = "Order"}
                },
                Values = new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Author", userId } } }
            };

            var folders = _config.DbContext.ListEntitiesByParams(tableFolders);

            if (folders.IsSuccess && folders.Result.Values != null && folders.Result.Values.Count > 0)
            {
                if (folders.Result.Values.All(s => s["Name"].ToString().ToLower() != "inbox"))
                {
                    tableFolders.Values = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {"Author", userId},
                            {"IsSystem", true},
                            {"Name", "Inbox"},
                            {"Description", "Inbox"},
                            {"Order", 1}
                        }
                    };
                    _config.DbContext.Insert(tableFolders);
                    refreshList = true;
                }

                if (folders.Result.Values.All(s => s["Name"].ToString().ToLower() != "sent"))
                {
                    tableFolders.Values = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {"Author", userId},
                            {"IsSystem", true},
                            {"Name", "Sent"},
                            {"Description", "Sent"},
                            {"Order", 2}
                        }
                    };
                    _config.DbContext.Insert(tableFolders);
                    refreshList = true;
                }

                if (folders.Result.Values.All(s => s["Name"].ToString().ToLower() != "trash"))
                {
                    tableFolders.Values = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            {"Author", userId},
                            {"IsSystem", true},
                            {"Name", "Trash"},
                            {"Description", "Trash"},
                            {"Order", 3}
                        }
                    };
                    _config.DbContext.Insert(tableFolders);
                    refreshList = true;
                }
            }
            else
            {
                tableFolders.Values = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Author", userId},
                        {"IsSystem", true},
                        {"Name", "Inbox"},
                        {"Description", "Inbox"},
                        {"Order", 1}
                    }
                };
                _config.DbContext.Insert(tableFolders);

                tableFolders.Values = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Author", userId},
                        {"IsSystem", true},
                        {"Name", "Sent"},
                        {"Description", "Sent"},
                        {"Order", 2}
                    }
                };
                _config.DbContext.Insert(tableFolders);

                tableFolders.Values = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Author", userId},
                        {"IsSystem", true},
                        {"Name", "Trash"},
                        {"Description", "Trash"},
                        {"Order", 3}
                    }
                };
                _config.DbContext.Insert(tableFolders);
                refreshList = true;
            }

            if (refreshList)
            {
                tableFolders.Values =
                    new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Author", userId } } };
                returnModel = _config.DbContext.ListEntitiesByParams(tableFolders);
            }
            else
            {
                returnModel = folders;
            }

            if (returnModel.IsSuccess && returnModel.Result.Values != null)
                returnModel.Result.Values = returnModel.Result.Values.OrderBy(s => s["Order"]).ToList();

            if (!withCount) return returnModel;
            if (returnModel.Result.Values == null) return returnModel;
            foreach (var value in returnModel.Result.Values)
            {
                try
                {
                    var tableUsers = new EntityViewModel
                    {
                        TableSchema = _config.Tables["EmailUsers"].Schema,
                        TableName = _config.Tables["EmailUsers"].Name,
                        HasConfig = true,
                        Fields = new List<EntityFieldsViewModel>
                    {
                        new EntityFieldsViewModel {ColumnName = "UserEmailFolderId"}
                    },
                        Values = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object> {{"UserEmailFolderId", value["Id"]}}
                    }
                    };
                    var count = _config.DbContext.GetCountByParameter(tableUsers);
                    value.Add("Count", count.Result.Values[0].FirstOrDefault());
                }
                catch
                {
                    //Ignore
                }
            }

            return returnModel;
        }

        /// <summary>
        ///     Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<List<Guid>> Create(EntityViewModel model)
        {
            var returnModel = new ResultModel<List<Guid>>
            {
                IsSuccess = false,
                Result = new List<Guid>()
            };

            var commitTransaction = true;

            if (model?.Values == null || model.Values.Count <= 0) return returnModel;
            using (var dbContextTransaction = _config.DbContext.Database.BeginTransaction())
            {
                foreach (var value in model.Values)
                    try
                    {
                        model.TableSchema = _config.Tables["Email"].Schema;
                        model.TableName = _config.Tables["Email"].Name;

                        var recipientKey = value.Keys.FirstOrDefault(s => s.ToLower() == "recipients");

                        List<string> recipients = null;
                        if (recipientKey != null)
                        {
                            recipients = value[recipientKey] as List<string>;
                            value.Remove(recipientKey);
                        }

                        var recipientField = model.Fields.FirstOrDefault(s => s.ColumnName.ToLower() == "recipients");

                        if (recipientField != null) model.Fields.Remove(recipientField);

                        var notificationModel = _config.DbContext.Insert(model);

                        if (notificationModel != null && notificationModel.IsSuccess)
                        {
                            var senderFolders = GetUserFolders(value["Author"].ToString());

                            var senderModel = new EntityViewModel
                            {
                                TableSchema = _config.Tables["EmailUsers"].Schema,
                                TableName = _config.Tables["EmailUsers"].Name,
                                Values = new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        {"Id", Guid.NewGuid()},
                                        {"EmailId", notificationModel.Result},
                                        {
                                            "UserEmailFolderId",
                                            senderFolders.Result.Values.FirstOrDefault(s =>
                                                s["Name"].ToString() == "Sent")["Id"]
                                        },
                                        {"Author", value["Author"]},
                                        {"Created", DateTime.Now},
                                        {"Type", 0}
                                    }
                                },
                                Fields = new List<EntityFieldsViewModel>
                                {
                                    new EntityFieldsViewModel {ColumnName = "Id"},
                                    new EntityFieldsViewModel {ColumnName = "EmailId"},
                                    new EntityFieldsViewModel {ColumnName = "UserEmailFolderId"},
                                    new EntityFieldsViewModel {ColumnName = "Author"},
                                    new EntityFieldsViewModel {ColumnName = "Created"},
                                    new EntityFieldsViewModel {ColumnName = "Type"}
                                }
                            };

                            var entSenderResult = _config.DbContext.Insert(senderModel);

                            if (!entSenderResult.IsSuccess)
                                commitTransaction = false;
                            else
                                returnModel.Result.Add(entSenderResult.Result);


                            if (recipients == null) continue;
                            {
                                foreach (var recipient in recipients)
                                {
                                    var recipientFolders = GetUserFolders(recipient);

                                    var recipientModel = new EntityViewModel
                                    {
                                        TableSchema = _config.Tables["EmailUsers"].Schema,
                                        TableName = _config.Tables["EmailUsers"].Name,
                                        Values = new List<Dictionary<string, object>>
                                        {
                                            new Dictionary<string, object>
                                            {
                                                {"Id", Guid.NewGuid()},
                                                {"EmailId", notificationModel.Result},
                                                {
                                                    "UserEmailFolderId",
                                                    recipientFolders.Result.Values.FirstOrDefault(s =>
                                                        s["Name"].ToString() == "Inbox")["Id"]
                                                },
                                                {"Author", recipient},
                                                {"Created", DateTime.Now},
                                                {"Type", 1}
                                            }
                                        },
                                        Fields = new List<EntityFieldsViewModel>
                                        {
                                            new EntityFieldsViewModel {ColumnName = "Id"},
                                            new EntityFieldsViewModel {ColumnName = "EmailId"},
                                            new EntityFieldsViewModel {ColumnName = "UserEmailFolderId"},
                                            new EntityFieldsViewModel {ColumnName = "Author"},
                                            new EntityFieldsViewModel {ColumnName = "Created"},
                                            new EntityFieldsViewModel {ColumnName = "Type"}
                                        }
                                    };

                                    var entRecipientResult = _config.DbContext.Insert(recipientModel);

                                    if (!entRecipientResult.IsSuccess) commitTransaction = false;
                                }
                            }
                        }
                        else
                        {
                            commitTransaction = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        commitTransaction = false;
                        returnModel.Errors.Add(new ErrorModel(ExceptionCodes.UnhandledException, ex.Message));
                    }

                lock (LockObj)
                {
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
        ///     Send
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="recipients"></param>
        /// <returns></returns>
        public ResultModel<bool> Send(Guid notificationId, List<Guid> recipients)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        /// <summary>
        ///     Get list of notification by folder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> ListNotificationsByFolder(EntityViewModel model)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false,
                Result = new EntityViewModel
                {
                    TableSchema = _config.Tables["Email"].Schema,
                    TableName = _config.Tables["Email"].Name,
                    Fields = new List<EntityFieldsViewModel>(),
                    Values = new List<Dictionary<string, object>>()
                }
            };

            var tableUsers = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                HasConfig = true,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "EmailId"},
                    new EntityFieldsViewModel {ColumnName = "Author"},
                    new EntityFieldsViewModel {ColumnName = "Type"},
                    new EntityFieldsViewModel {ColumnName = "IsRead"}
                },
                Values = model.Values
            };

            var userNotifications = _config.DbContext.ListEntitiesByParams(tableUsers);

            returnModel.IsSuccess = userNotifications.IsSuccess;

            if (!userNotifications.IsSuccess || userNotifications.Result.Values == null ||
                userNotifications.Result.Values.Count <= 0) return returnModel;
            var tableNotifications = new EntityViewModel
            {
                TableSchema = _config.Tables["Email"].Schema,
                TableName = _config.Tables["Email"].Name,
                HasConfig = false,
                Fields = model.Fields,
                Values = userNotifications.Result.Values.Select(s => new Dictionary<string, object>
                {
                    {"Id", s["EmailId"]}
                }).ToList()
            };

            var tableRecipients = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "Author"},
                    new EntityFieldsViewModel {ColumnName = "Type"}
                }
            };

            var userModel = new EntityViewModel
            {
                TableSchema = "Identity",
                TableName = "Users",
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "Email"},
                    new EntityFieldsViewModel {ColumnName = "UserName"}
                }
            };

            tableNotifications = ViewModelBuilder.Create(_config.DbContext, tableNotifications);

            var notifications = _config.DbContext.ListEntitiesByParams(tableNotifications);

            if (!notifications.IsSuccess || notifications.Result.Values.Count <= 0) return returnModel;
            {
                foreach (var notification in notifications.Result.Values)
                {
                    var notificationAuthor =
                        userNotifications.Result.Values.FirstOrDefault(s =>
                            (Guid)s["EmailId"] == (Guid)notification["Id"]);

                    if (notificationAuthor != null && (int)notificationAuthor["Type"] == 1)
                    {
                        notification.Add("IsRead", notificationAuthor["IsRead"]);
                        var notificationParam = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object> {{"EmailId", (Guid) notification["Id"]}, {"Type", 0}}
                        };
                        tableRecipients.Values = notificationParam;
                        var notificationRecipientsObjResult = _config.DbContext.ListEntitiesByParams(tableRecipients);
                        if (notificationRecipientsObjResult.IsSuccess &&
                            notificationRecipientsObjResult.Result.Values != null &&
                            notificationRecipientsObjResult.Result.Values.Count > 0)
                        {
                            userModel.Values = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    {"Id", notificationRecipientsObjResult.Result.Values[0]["Author"]}
                                }
                            };
                            var userAuthor = _config.DbContext.GetEntityByParams(userModel);
                            if (userAuthor.IsSuccess && userAuthor.Result.Values != null &&
                                userAuthor.Result.Values.Count > 0)
                                notification["Author"] = userAuthor.Result.Values[0];
                        }
                    }
                    else
                    {
                        var notificationParam = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object> {{"EmailId", (Guid) notification["Id"]}, {"Type", 1}}
                        };
                        tableRecipients.Values = notificationParam;

                        var notificationRecipientsObjResult = _config.DbContext.ListEntitiesByParams(tableRecipients);

                        if (notificationRecipientsObjResult.IsSuccess &&
                            notificationRecipientsObjResult.Result.Values != null)
                        {
                            userModel.Values = notificationRecipientsObjResult.Result.Values
                                .Select(s => new Dictionary<string, object> { { "Id", s["Author"] } }).ToList();
                            var userRecipients = _config.DbContext.GetEntityByParams(userModel);
                            if (userRecipients.IsSuccess && userRecipients.Result.Values != null &&
                                userRecipients.Result.Values.Count > 0)
                                notification.Add("Recipients", userRecipients.Result.Values);
                        }

                        if (notificationAuthor != null)
                            userModel.Values = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object> {{"Id", notificationAuthor["Author"]}}
                            };
                        var userAuthor = _config.DbContext.GetEntityByParams(userModel);
                        if (userAuthor.IsSuccess && userAuthor.Result.Values != null &&
                            userAuthor.Result.Values.Count > 0) notification["Author"] = userAuthor.Result.Values[0];
                    }

                    if (notificationAuthor == null) continue;
                    notification["Id"] = notificationAuthor["Id"];
                    notification.Add("Type", notificationAuthor["Type"]);
                }

                returnModel = notifications;
            }
            return returnModel;
        }

        /// <summary>
        ///     Send Notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> ListSentNotifications(EntityViewModel model)
        {
            model.TableSchema = _config.Tables["Email"].Schema;
            model.TableName = _config.Tables["Email"].Name;

            model = ViewModelBuilder.Create(_config.DbContext, model);

            var result = _config.DbContext.ListEntitiesByParams(model);

            var tableRecipients = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "Author"}
                }
            };

            foreach (var notification in result.Result.Values)
            {
                var notificationParam = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> {{"EmailId", (Guid) notification["Id"]}, {"Type", 1}}
                };
                tableRecipients.Values = notificationParam;

                var notificationRecipientsObjResult = _config.DbContext.ListEntitiesByParams(tableRecipients);

                if (notificationRecipientsObjResult.IsSuccess && notificationRecipientsObjResult.Result.Values != null)
                    notification.Add("Recipients", notificationRecipientsObjResult.Result.Values);
            }

            return result;
        }

        public ResultModel<EntityViewModel> ListReceivedNotifications(EntityViewModel model)
        {
            var finalResultList = new List<Dictionary<string, object>>();

            model.Values[0].Add("Type", 1);

            var tableRecipients = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                HasConfig = true,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "EmailId"},
                    new EntityFieldsViewModel {ColumnName = "Type"},
                    new EntityFieldsViewModel {ColumnName = "IsRead"}
                },
                Values = model.Values
            };

            var result = _config.DbContext.ListEntitiesByParams(tableRecipients);


            var tableNotifications = new EntityViewModel
            {
                TableSchema = _config.Tables["Email"].Schema,
                TableName = _config.Tables["Email"].Name,
                HasConfig = true,
                Fields = model.Fields
            };

            tableNotifications = ViewModelBuilder.Create(_config.DbContext, tableNotifications);


            if (result.IsSuccess && result.Result.Values.Count > 0)
                foreach (var notificationRecipient in result.Result.Values)
                {
                    var notificationObjResult = _config.DbContext.GetEntityById(tableNotifications,
                        (Guid)notificationRecipient["EmailId"]);
                    if (!notificationObjResult.IsSuccess || notificationObjResult.Result.Values == null) continue;
                    var notificationObj = notificationObjResult.Result.Values.FirstOrDefault();
                    if (notificationObj == null) continue;
                    notificationObj.Add("IsRead", notificationRecipient["IsRead"]);
                    notificationObj["Id"] = notificationRecipient["Id"];
                    finalResultList.Add(notificationObj);
                }

            tableNotifications.Values = finalResultList;

            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = true,
                Result = tableNotifications
            };

            return returnModel;
        }

        /// <summary>
        ///     Mark as read
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        public ResultModel<bool> MarkAsRead(List<Guid> notifications)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };

            var commitTransaction = true;

            if (notifications == null || notifications.Count <= 0) return returnModel;
            var tableRecipients = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                HasConfig = true,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "IsRead"}
                },
                Values = notifications.Select(s => new Dictionary<string, object>
                {
                    {"Id", s},
                    {"IsRead", true}
                }).ToList()
            };

            using (var dbContextTransaction = _config.DbContext.Database.BeginTransaction())
            {
                try
                {
                    returnModel = _config.DbContext.Refresh(tableRecipients);

                    if (!returnModel.IsSuccess || !returnModel.Result) commitTransaction = false;
                }
                catch (Exception ex)
                {
                    commitTransaction = false;
                    returnModel.Errors.Add(new ErrorModel(ExceptionCodes.UnhandledException, ex.Message));
                    return returnModel;
                }

                lock (LockObj)
                {
                    if (commitTransaction)
                        dbContextTransaction.Commit();
                    else
                        dbContextTransaction.Rollback();
                }
            }

            return returnModel;
        }

        public ResultModel<bool> ChangeFolder(EntityViewModel model)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };

            var tableRecipients = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                HasConfig = true,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "UserEmailFolderId"}
                },
                Values = model.Values
            };

            using (var dbContextTransaction = _config.DbContext.Database.BeginTransaction())
            {
                try
                {
                    returnModel = _config.DbContext.Refresh(tableRecipients);

                    lock (LockObj)
                    {
                        if (returnModel.IsSuccess && returnModel.Result)
                            dbContextTransaction.Commit();
                        else
                            dbContextTransaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    returnModel.Errors.Add(new ErrorModel(ExceptionCodes.UnhandledException, ex.Message));
                    return returnModel;
                }
            }

            return returnModel;
        }

        public ResultModel<EntityViewModel> GetNotificationById(Guid userNotificationId)
        {
            var response = new ResultModel<EntityViewModel>();
            var tableNotification = new EntityViewModel
            {
                TableSchema = _config.Tables["EmailUsers"].Schema,
                TableName = _config.Tables["EmailUsers"].Name,
                Fields = new List<EntityFieldsViewModel>
                {
                    new EntityFieldsViewModel {ColumnName = "Id"},
                    new EntityFieldsViewModel {ColumnName = "Author"},
                },
                HasConfig = true,
                Values = new List<Dictionary<string, object>>
                {
                   new Dictionary<string, object>
                   {
                       {
                           "Id",
                           userNotificationId
                       }
                   }
                }
            };
            tableNotification = ViewModelBuilder.Create(_config.DbContext, tableNotification);
            var notificationUser = _config.DbContext.ListEntitiesByParams(tableNotification);

            if (!notificationUser.IsSuccess) return response;
            var table = _config.Tables["Email"];
            tableNotification.TableName = table.Name;
            tableNotification.TableSchema = table.Schema;
            var notId = notificationUser.Result.Values[0].First(x => x.Key == "EmailId").Value;
            tableNotification.Values = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {
                        "Id",
                        notId
                    }
                }
            };
            tableNotification = ViewModelBuilder.Create(_config.DbContext, tableNotification);
            tableNotification.Fields.AddRange(new List<EntityFieldsViewModel>
            {
                new EntityFieldsViewModel{ColumnName = "Author"},
                new EntityFieldsViewModel{ColumnName = "Created"},
            });
            return _config.DbContext.ListEntitiesByParams(tableNotification);
        }
    }
}