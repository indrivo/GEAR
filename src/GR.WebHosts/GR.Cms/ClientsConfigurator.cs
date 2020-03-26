using System.Collections.Generic;
using System.Linq;
using GR.Core.Helpers.Scopes;
using GR.Identity.Clients.Abstractions.Helpers;
using IdentityServer4;
using IdentityServer4.Models;

namespace GR.Cms
{
	public class ClientsConfigurator : DefaultClientsConfigurator
	{
		/// <summary>
		/// Get clients
		/// </summary>
		/// <param name="clientsUrl"></param>
		/// <returns></returns>
		public override IEnumerable<Client> GetClients(IDictionary<string, string> clientsUrl)
		{
			var def = base.GetClients(clientsUrl).ToList();
			def.AddRange(new List<Client>
			{
				 // JavaScript Client
                new Client
				{
					ClientId = "js",
					ClientName = "Gear SPA OpenId Client",
					AllowedGrantTypes = GrantTypes.Implicit,
					AllowAccessTokensViaBrowser = true,
					RedirectUris =           { $"{clientsUrl["Spa"]}/" },
					RequireConsent = false,
					PostLogoutRedirectUris = { $"{clientsUrl["Spa"]}/" },
					AllowedCorsOrigins =     { $"{clientsUrl["Spa"]}" },
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile
					},
				},
				new Client
				{
					ClientId = "xamarin interactive",
					ClientName = "Gear Xamarin OpenId Client",
					AllowedGrantTypes = GrantTypes.Hybrid,                    
                    //Used to retrieve the access token on the back channel.
                    ClientSecrets =
					{
						new Secret("secret".Sha256())
					},
					RedirectUris = { clientsUrl["Xamarin"] },
					RequireConsent = false,
					RequirePkce = true,
					PostLogoutRedirectUris = { $"{clientsUrl["Xamarin"]}/Account/Redirecting" },
					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						IdentityServerConstants.StandardScopes.OfflineAccess
					},
                    //Allow requesting refresh tokens for long lived API access
                    AllowOfflineAccess = true,
					AllowAccessTokensViaBrowser = true
				},
				new Client
				{
					ClientId = "xamarin password",
					ClientName = "Gear Password OpenId Client",
					AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						IdentityServerConstants.StandardScopes.OfflineAccess,
						GearScopes.CORE
					},
					AllowOfflineAccess = true,
					AllowAccessTokensViaBrowser = true
				}
			});
			return def;
		}
	}
}
