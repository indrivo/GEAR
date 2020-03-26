using System;
using System.Net.Http;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace GR.TestMobileAuth
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync("http://localhost:9099")
                .GetAwaiter()
                .GetResult();

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var identityServerResponse = client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "password",
                ClientId = "xamarin password",
                ClientSecret = "secret",
                Scope = "openid profile offline_access core",
                UserName = "admin",
                Password = "Adm!n.2020"
            }).GetAwaiter().GetResult();

            if (identityServerResponse.IsError)
            {
                Console.WriteLine(identityServerResponse.Error);
                return;
            }

            client.SetBearerToken(identityServerResponse.AccessToken);

            var response = client.GetAsync("http://localhost:9099/api/country/GetCountriesInfo").GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(JArray.Parse(content));
            }
            Console.WriteLine("Hello World!");
        }
    }
}
