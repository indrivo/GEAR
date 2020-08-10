using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GR.Core.Extensions;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.Models.PostmanModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GR.Core.Razor.Api
{
    [AllowAnonymous]
    [Route("/postman")]
    public class PostmanDocsApiController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject action descriptor service
        /// </summary>
        private readonly IActionDescriptorCollectionProvider _provider;

        #endregion

        public PostmanDocsApiController(IActionDescriptorCollectionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Postman collection
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(PostmanCollection))]
        public JsonResult Docs()
        {
            var postManCollection = new PostmanCollection
            {
                Info = new PostmanInfo
                {
                    Id = Guid.NewGuid(),
                    Name = $"{GearApplication.ApplicationName} API",
                    Description = "api"
                },
                Folders = new Collection<PostmanFolder>()
            };

            var apiRoutes = _provider.ActionDescriptors.Items
                .Where(x => x.AttributeRouteInfo?.Template?.StartsWith("api/") ?? false).ToList();

            var groups = apiRoutes.GroupBy(x => x.RouteValues["Controller"])
                  .ToList();
            var domain = new Uri(HttpContext.GetAppBaseUrl());

            foreach (var group in groups)
            {
                var controllerGroup = apiRoutes.FirstOrDefault(x => x.RouteValues["Controller"].Equals(group.Key)).Is<ControllerActionDescriptor>();
                var type = controllerGroup.ControllerTypeInfo;
                var typeSummary = type.GetSummary();
                var postManFolder = new PostmanFolder
                {
                    Name = group.Key,
                    Description = typeSummary,
                    FolderRequests = new List<PostmanFolderRequest>()
                };

                foreach (var route in group)
                {
                    var constraint = route.ActionConstraints[0]?.Is<HttpMethodActionConstraint>();
                    var methodSummary = type.GetMethod(route.RouteValues["Action"]).GetSummary();
                    var methodDescriptor = route.Is<ControllerActionDescriptor>();
                    var request = new PostmanRequest
                    {
                        Url = new PostmanRequestUrl
                        {
                            Host = domain.Authority,
                            Path = route.AttributeRouteInfo.Template,
                            Protocol = HttpContext.Request.Scheme,
                            Query = new List<object>()
                        },
                        Method = constraint?.HttpMethods.FirstOrDefault() ?? "GET",
                        Headers = new List<PostmanHeader>
                        {
                            new PostmanHeader
                            {
                                Key = "Content-Type",
                                Value = "application/json"
                            }
                        },
                        Responses = new Collection<object>(),
                        Description = methodSummary,
                    };

                    var inputDictionary = methodDescriptor.Parameters.ToList()
                        .ToDictionary(parameter => parameter.Name, parameter => parameter.ParameterType.GetDefault());

                    request.Body = new PostmanBodyRequest
                    {
                        Mode = "raw",
                        Raw = inputDictionary.SerializeAsJson()
                    };

                    postManFolder.FolderRequests.Add(new PostmanFolderRequest
                    {
                        Name = route.RouteValues["Action"],
                        Request = request
                    });
                }

                postManCollection.Folders.Add(postManFolder);
            }

            return Json(postManCollection);
        }
    }
}
