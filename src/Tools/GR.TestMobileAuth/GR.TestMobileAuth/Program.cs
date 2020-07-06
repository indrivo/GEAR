using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace GR.TestMobileAuth
{
    internal class Program
    {
        private static readonly bool IsDevelopment = true;

        private static void Main()
        {
            Task.Run(async () =>
            {
                var baseAddress = IsDevelopment ? "http://localhost:9099" : "http://localhost:5000";

                var client = new HttpClient
                {
                    BaseAddress = new Uri(baseAddress)
                };

                var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Address = baseAddress,
                    Policy =
                    {
                        RequireHttps = false
                    }
                });

                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }

                var identityServerResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "xamarin password",
                    ClientSecret = "secret",
                    Scope = "openid profile offline_access core email",
                    UserName = "admin",
                    Password = "Adm!n.2020"
                });

                if (identityServerResponse.IsError)
                {
                    Console.WriteLine(identityServerResponse.Error);
                    return;
                }

                //Set bearer code
                client.SetBearerToken(identityServerResponse.AccessToken);


                var userInfo = await client.GetUserInfoAsync(new UserInfoRequest
                {
                    Address = disco.UserInfoEndpoint,
                    Token = identityServerResponse.AccessToken
                });

                if (userInfo.IsError)
                {
                    Console.WriteLine(userInfo.Error);
                    return;
                }

                var response = await client.PostAsync("/api/Profile/EditProfile", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"FirstName", "test" },
                    {"LastName", "test" },
                    {"PhoneNumber", "+37369991207" },
                    {"Birthday", DateTime.Now.ToString(CultureInfo.InvariantCulture) },
                    {"Email", "nicolae.lupei.1996@gmail.com" }
                }));

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(JArray.Parse(content));
                }
            }).Wait();
        }
    }
}
