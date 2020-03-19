using System.Collections.Generic;
using GR.Core.Helpers.Scopes;
using IdentityServer4;
using IdentityServer4.Models;
using ApiResource = IdentityServer4.Models.ApiResource;
using Client = IdentityServer4.Models.Client;
using IdentityResource = IdentityServer4.Models.IdentityResource;
using Secret = IdentityServer4.Models.Secret;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class DefaultClientsConfigurator
    {
        #region Default

        /// <summary>
        /// Default api resources
        /// </summary>
        protected IEnumerable<ApiResource> DefaultApiResources = new List<ApiResource>
        {
            new ApiResource(GearScopes.CORE, "Core web application")
        };

        /// <summary>
        /// Default identity resources
        /// </summary>
        protected IEnumerable<IdentityResource> DefaultIdentityResources = new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Address(),
            new IdentityResources.Phone()
        };

        #endregion

        /// <summary>
        /// Get api resources
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ApiResource> GetApiResources() => DefaultApiResources;

        /// <summary>
        /// Get client
        /// </summary>
        /// <param name="clientsUrl"></param>
        /// <returns></returns>
        public virtual IEnumerable<Client> GetClients(IDictionary<string, string> clientsUrl)
        {
            var defaultClients = new List<Client>
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
                        GearScopes.CORE
                    }
                },
                new Client
                {
                    ClientId = "core_swagger_ui",
                    ClientName = "Core Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        $"{clientsUrl["CORE"]}/swagger/o2c.html"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"{clientsUrl["CORE"]}/swagger/"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        GearScopes.CORE
                    }
                }
            };
            return defaultClients;
        }

        /// <summary>
        /// Get resources
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IdentityResource> GetResources() => DefaultIdentityResources;
    }
}