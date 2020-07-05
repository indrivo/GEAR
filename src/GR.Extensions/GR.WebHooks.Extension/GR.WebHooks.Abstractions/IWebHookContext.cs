using GR.Core.Abstractions;
using GR.WebHooks.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.WebHooks.Abstractions
{
    public interface IWebHookContext : IDbContext
    {
        DbSet<WebHook> WebHooks { get; set; }
        DbSet<HookEvent> Events { get; set; }
        DbSet<HookProvider> HookProviders { get; set; }
    }
}