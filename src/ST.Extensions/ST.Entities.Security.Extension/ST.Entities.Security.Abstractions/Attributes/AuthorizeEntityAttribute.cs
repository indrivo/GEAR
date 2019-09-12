using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Security.Abstractions.Enums;
using ST.Entities.Security.Abstractions.Helpers;

namespace ST.Entities.Security.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeEntityAttribute : TypeFilterAttribute
    {
        public AuthorizeEntityAttribute(params EntityAccessType[] accessType) : base(
            typeof(AuthorizeEntityPermissionAttributeExecutor))
        {
            Arguments = new object[] { new EntityPermissionAuthorizationRequirement(accessType) };
        }
    }

    [Serializable]
    public class AuthorizeEntityPermissionAttributeExecutor : IAsyncResourceFilter //OnMethodBoundaryAspect
    {
        /// <summary>
        /// Permissions
        /// </summary>
        private readonly EntityPermissionAuthorizationRequirement _authorizationRequirement;

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly IEntityRoleAccessManager _entityRoleAccessManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityRoleAccessManager"></param>
        /// <param name="authorizationRequirement"></param>
        public AuthorizeEntityPermissionAttributeExecutor(IEntityRoleAccessManager entityRoleAccessManager,
            EntityPermissionAuthorizationRequirement authorizationRequirement)
        {
            _entityRoleAccessManager = entityRoleAccessManager;
            _authorizationRequirement = authorizationRequirement;
        }

        //public override bool CompileTimeValidate(System.Reflection.MethodBase method)
        //{
        //    var type = method.DeclaringType;
        //    if (type != typeof(Task<JsonResult>) && type != typeof(JsonResult))
        //    {
        //        throw new Exception(
        //            $"{nameof(AuthorizeEntityAttribute)} can only be used with {nameof(JsonResult)} methods");
        //    }

        //    return true;
        //}

        /// <summary>
        /// On executing context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var req = context.HttpContext.Request;
            req.EnableRewind();
            var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();
            req.Body.Position = 0;
            var responseBody = context.HttpContext.Response;
            try
            {
                var data = JsonConvert.DeserializeObject<RequestData>(body);

                var isValid = await IsValid(data.EntityName);
                if (!isValid.IsSuccess)
                {
                    await responseBody.WriteAsync(isValid.Serialize());
                }
                else if (!await _entityRoleAccessManager.HaveAccessAsync(data.EntityName,
                    _authorizationRequirement.RequiredPermissions))
                {
                    await responseBody.WriteAsync(new ResultModel
                    {
                        Errors = new List<IErrorModel>
                        {
                            new ErrorModel(nameof(Settings.ACCESS_DENIED_MESSAGE), Settings.ACCESS_DENIED_MESSAGE)
                        }
                    }.Serialize());
                }
                else
                {
                    await next();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await responseBody.WriteAsync(new ResultModel
                {
                    Errors = new List<IErrorModel>
                    {
                        new ErrorModel(nameof(BadRequestResult), nameof(BadRequestResult))
                    }
                }.Serialize());
            }
        }

        /// <summary>
        /// Is valid entity
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<ResultModel<TableModel>> IsValid(string tableName)
        {
            var badParams = new ErrorModel(string.Empty, "Entity not identified!");
            var entityNotFound = new ErrorModel(string.Empty, "Entity not found!");
            var result = new ResultModel<TableModel>();
            if (string.IsNullOrEmpty(tableName))
            {
                result.Errors.Add(badParams);
                return result;
            }

            var entity = await _entityRoleAccessManager.Tables.FirstOrDefaultAsync(x => x.Name == tableName);

            if (entity == null)
            {
                result.Errors.Add(entityNotFound);
                return result;
            }

            result.IsSuccess = true;
            result.Result = entity;
            return result;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Requirements class
    /// </summary>
    public class EntityPermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// List of Required Permissions
        /// </summary>
        public IEnumerable<EntityAccessType> RequiredPermissions { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requiredPermissions">List of required Permissions</param>
        public EntityPermissionAuthorizationRequirement(IEnumerable<EntityAccessType> requiredPermissions)
        {
            RequiredPermissions = requiredPermissions;
        }
    }
}