using GR.Identity.Data.Permissions;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using ApiResource = IdentityServer4.Models.ApiResource;
using Client = IdentityServer4.Models.Client;
using IdentityResource = IdentityServer4.Models.IdentityResource;
using Secret = IdentityServer4.Models.Secret;

namespace GR.Identity.IdentityServer4
{
    public class IdentityServer4Configurator
    {
        public virtual IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource(BPMNServerConstants.StandarScopes.CORE, "Core web application")
            };
        }

        /// <summary>
        /// Get client
        /// </summary>
        /// <param name="clientsUrl"></param>
        /// <returns></returns>
        public virtual IEnumerable<Client> GetClients(IDictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "core",
                    ClientName = "CORE",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("very_secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientUri = clientsUrl["CORE"],
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = new List<string>
                    {
                        $"{clientsUrl["CORE"]}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{clientsUrl["CORE"]}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        BPMNServerConstants.StandarScopes.CORE
                    }
                },
                new Client
                {
                    ClientId = "bpmn_swagger_ui",
                    ClientName = "BPMN Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        $"{clientsUrl["BPMApi"]}/swagger/o2c.html"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"{clientsUrl["BPMApi"]}/swagger/"
                    },
                    AllowedScopes =
                    {
                        BPMNServerConstants.StandarScopes.BPMApi,
                        BPMNServerConstants.StandarScopes.CORE
                    }
                }
            };
        }

        /// <summary>
        /// Get resources
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResources.Phone()
            };
        }
    }
}