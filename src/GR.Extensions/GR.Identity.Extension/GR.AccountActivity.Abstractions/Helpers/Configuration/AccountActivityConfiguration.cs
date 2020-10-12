using System;
using Microsoft.AspNetCore.Http;

namespace GR.AccountActivity.Abstractions.Helpers.Configuration
{
    public class AccountActivityConfiguration
    {
        public AccountActivityConfiguration()
        {
            DeviceCookieOptions = new DeviceCookieOptions
            {
                Enabled = true,
                CookieOptions = new CookieOptions
                {
                    MaxAge = TimeSpan.FromDays(30)
                }
            };
        }
        public DeviceCookieOptions DeviceCookieOptions { get; set; }
    }
}