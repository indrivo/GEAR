﻿using System;
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
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Security.Abstractions.Enums;
using GR.Entities.Security.Abstractions.Helpers;
using GR.Entities.Abstractions;

namespace GR.Entities.Security.Abstractions.Attributes
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
        /// Inject entity repository
        /// </summary>
        private readonly IEntityRepository _entityRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityRoleAccessManager"></param>
        /// <param name="authorizationRequirement"></param>
        public AuthorizeEntityPermissionAttributeExecutor(IEntityRoleAccessManager entityRoleAccessManager,
            EntityPermissionAuthorizationRequirement authorizationRequirement, IEntityRepository entityRepository)
        {
            _entityRoleAccessManager = entityRoleAccessManager;
            _authorizationRequirement = authorizationRequirement;
            _entityRepository = entityRepository;
        }

       

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

                var isValid = await _entityRepository.FindTableByNameAsync(data.EntityName); //await IsValid(data.EntityName);
                if (!isValid.IsSuccess)
                {
                    await responseBody.WriteAsync(isValid.SerializeAsJson());
                }
                else if (!await _entityRoleAccessManager.HaveAccessAsync(data.EntityName,
                    _authorizationRequirement.RequiredPermissions))
                {
                    await responseBody.WriteAsync(new ResultModel
                    {
                        Errors = new List<IErrorModel>
                        {
                            new ErrorModel(nameof(GearSettings.ACCESS_DENIED_MESSAGE), GearSettings.ACCESS_DENIED_MESSAGE)
                        }
                    }.SerializeAsJson());
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
                }.SerializeAsJson());
            }
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