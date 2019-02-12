using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using ST.Identity.Data.Permissions;
using identityModels = IdentityServer4.EntityFramework.Entities;

namespace ST.Identity.Configuration
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource(BPMNServerConstants.StandarScopes.CORE, "Core web application"),
                new ApiResource(BPMNServerConstants.StandarScopes.ProcessEngine, "Process engine api"),
            };
        }
        /// <summary>
        /// Get client
        /// </summary>
        /// <param name="clientsUrl"></param>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients(IDictionary<string, string> clientsUrl)
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
        /// Get seed resources
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<identityModels.IdentityResource> GetSeedResources(this IEnumerable<identityModels.IdentityResource> resources, ConfigurationDbContext context)
        {
            if (!context.Database.IsNpgsql())
            {
                return resources;
            }
            var resourceIndex = 1;
            var userClaimIndex = 1;

            var seed = new List<identityModels.IdentityResource>();
            foreach (var item in resources)
            {
                item.Id = resourceIndex++;
                foreach (var userClaim in item.UserClaims)
                {
                    userClaim.Id = userClaimIndex++;
                }
                seed.Add(item);
            }
            return seed;
        }

        /// <summary>
        /// Get api resources
        /// </summary>
        /// <param name="apiResources"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<identityModels.ApiResource> GetSeedApiResources(this IEnumerable<identityModels.ApiResource> apiResources, ConfigurationDbContext context)
        {
            if (!context.Database.IsNpgsql())
            {
                return apiResources;
            }
            var apiIndex = 1;
            var data = new List<identityModels.ApiResource>();
            var scopeIndex = context.ApiResources.Count() + 1;
            foreach (var apiResource in apiResources)
            {
                foreach (var scope in apiResource.Scopes)
                {
                    scope.Id = scopeIndex++;
                }
                apiResource.Id = apiIndex++;
                data.Add(apiResource);
            }
            return data;
        }

        /// <summary>
        /// Get seed for clients
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<identityModels.Client> GetSeedClients(this IEnumerable<identityModels.Client> clients, ConfigurationDbContext context)
        {
            if (!context.Database.IsNpgsql())
            {
                return clients;
            }
            var response = new List<identityModels.Client>();
            var index = 1;
            var redirectIndex = 1;
            var propIndex = 1;
            var grantTypeIndex = 1;
            var clientSecretIndex = 1;
            var redirectUriIndex = 1;
            var scopeIndex = 1;
            var resIndex = 1;
            var allowedCorsOriginIndex = 1;
            //var scopes = new Dictionary<string, int>();
            try
            {
                foreach (var client in clients)
                {
                    client.Id = index++;

                    foreach (var restriction in client.IdentityProviderRestrictions)
                    {
                        restriction.Id = resIndex++;
                    }

                    foreach (var redirectUri in client.PostLogoutRedirectUris)
                    {
                        redirectUri.Id = redirectIndex++;
                    }

                    foreach (var allowedCorsOrigin in client.AllowedCorsOrigins)
                    {
                        allowedCorsOrigin.Id = allowedCorsOriginIndex++;
                    }

                    foreach (var property in client.Properties)
                    {
                        property.Id = propIndex++;
                    }

                    foreach (var grantType in client.AllowedGrantTypes)
                    {
                        grantType.Id = grantTypeIndex++;
                    }

                    foreach (var scope in client.AllowedScopes)
                    {
                        scope.Id = scopeIndex++;
                        //if (scopes.ContainsKey(scope.Scope))
                        //{
                        //    scope.Id = scopes[scope.Scope];
                        //}
                        //else
                        //{
                        //    scope.Id = scopeIndex++;
                        //    scopes.Add(scope.Scope, scope.Id);
                        //}
                    }

                    foreach (var clientSecret in client.ClientSecrets)
                    {
                        clientSecret.Id = clientSecretIndex++;
                    }

                    foreach (var redirect in client.RedirectUris)
                    {
                        redirect.Id = redirectUriIndex++;
                    }
                    response.Add(client);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return response;
        }

        /// <summary>
        /// Get resources
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetResources()
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