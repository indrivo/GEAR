using System;
using System.IO;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Hooks.Gitlab.Enums;
using GR.Hooks.Gitlab.Models;
using GR.WebHooks.Abstractions;
using GR.WebHooks.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace GR.Hooks.Gitlab
{
    public class GitlabHookProvider : IHookReceiver
    {
        public async Task<ResultModel> ReceiveEventAsync(Guid? hookId, HttpContext httpContext)
        {
            var result = new ResultModel();
            httpContext.Request.Headers.TryGetValue("X-Gitlab-Event", out var eventType);
            var evt = eventType.ToString().ToEnumMemberValue<GitlabEventType>();
            string eventBody;
            using (var reader = new StreamReader(httpContext.Request.Body))
            {
                eventBody = await reader.ReadToEndAsync();
            }

            switch (evt)
            {
                case GitlabEventType.Unknown:
                    break;
                case GitlabEventType.Push:
                    var pushHook = eventBody.Deserialize<PushHook>();
                    break;
                case GitlabEventType.TagPush:
                    break;
                case GitlabEventType.Issue:
                    break;
                case GitlabEventType.Note:
                    break;
                case GitlabEventType.MergeRequest:
                    break;
                case GitlabEventType.WikiPage:
                    break;
                case GitlabEventType.Pipeline:
                    break;
                case GitlabEventType.Job:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        /// <summary>
        /// Validate receiver
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public ResultModel ValidateReceiver(WebHook hook, HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("X-Gitlab-Token", out var token);
            return new ResultModel
            {
                IsSuccess = hook.AllowAnonymous || hook.Token.Equals(token)
            };
        }
    }
}