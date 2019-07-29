using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ST.Core.Helpers;

namespace ST.TaskManager.Abstractions
{
    public interface ITaskManager<TUser> where TUser : IdentityUser
    {
        Task<ResultModel> CreateTaskAsync(TUser user, );
    }
}