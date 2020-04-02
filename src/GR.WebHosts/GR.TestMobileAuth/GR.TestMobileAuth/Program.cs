using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace GR.TestMobileAuth
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("http://localhost:9099");

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

                client.SetBearerToken(identityServerResponse.AccessToken);

                var response = await client.GetAsync("http://localhost:9099/api/country/GetCountriesInfo");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(JArray.Parse(content));
                }

                Console.WriteLine("Hello World!");
            }).Wait();
        }
    }
}
