using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ST.Identity.Data.Permissions;
using ApiResource = IdentityServer4.Models.ApiResource;
using Client = IdentityServer4.Models.Client;
using identityModels = IdentityServer4.EntityFramework.Entities;
using IdentityResource = IdentityServer4.Models.IdentityResource;
using Secret = IdentityServer4.Models.Secret;

namespace ST.Configuration.Configuration
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource(BPMNServerConstants.StandarScopes.CORE, "Core web application"),
                new ApiResource(BPMNServerConstants.StandarScopes.ProcessEngine, "Process engine api"),
                new ApiResource(BPMNServerConstants.StandarScopes.ECommerce, "ECommerce application"),
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
                    ClientId = "eCommerce",
                    ClientName = "eCommerce",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("very_secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientUri = clientsUrl["eCommerce"],
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = new List<string>
                    {
                        $"{clientsUrl["eCommerce"]}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{clientsUrl["eCommerce"]}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        BPMNServerConstants.StandarScopes.ECommerce
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
        public static IEnumerable<identityModels.IdentityResource> GetSeedResources(this IEnumerable<IdentityResource> resources, ConfigurationDbContext context)
        {
            var resourceIndex = 1;
            foreach (var item in resources)
            {
                var obj = new identityModels.IdentityResource
                {
                    Id = resourceIndex++,
                    Required = item.Required,
                    Enabled = item.Enabled,
                    Created = DateTime.Now,
                    NonEditable = false,
                    Description = item.Description,
                    DisplayName = item.DisplayName,
                    Emphasize = item.Emphasize,
                    Name = item.Name,
                    ShowInDiscoveryDocument = item.ShowInDiscoveryDocument,
                    Updated = DateTime.Now
                };
                
                yield return obj;
            }
        }

        /// <summary>
        /// Get api resources
        /// </summary>
        /// <param name="apiResources"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<identityModels.ApiResource> GetSeedApiResources(this IEnumerable<ApiResource> apiResources, ConfigurationDbContext context)
        {
            var apiIndex = 1;
            var scopeIndex = context.ApiResources.Count() + 1;
            var dataResources = apiResources.Adapt<IEnumerable<identityModels.ApiResource>>().ToList();
            foreach (var apiResource in dataResources)
            {
                foreach (var scope in apiResource.Scopes)
                {
                    scope.Id = scopeIndex++;
                }
                apiResource.Id = apiIndex++;
                yield return apiResource;
            }
        }


        /// <summary>
        /// Generate index
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static int GenerateIndex(string format)
        {
            return Convert.ToInt32(format);
        }

        /// <summary>
        /// Get seed for clients
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<identityModels.Client> GetSeedClients(this IEnumerable<Client> clients, ConfigurationDbContext context)
        {
            //TODO: Replace with ToEntity() from AutoMapper extensions, the current version of AutoMapper and Identity.Server4 crash
            var index = 1;
            var enumerateClients = clients.ToList();
            foreach (var client in enumerateClients)
            {
                var retClient = new identityModels.Client
                {
                    Id = index++,
                    Enabled = client.Enabled,
                    Created = DateTime.Now,
                    AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                    AccessTokenLifetime = client.AccessTokenLifetime,
                    AccessTokenType = client.AccessTokenLifetime,
                    AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                    AllowOfflineAccess = client.AllowOfflineAccess,
                    AllowPlainTextPkce = client.AllowPlainTextPkce,
                    AllowRememberConsent = client.AllowRememberConsent,
                    AllowedCorsOrigins = client.AllowedCorsOrigins.Select(x => new identityModels.ClientCorsOrigin
                    {
                        Id = GenerateIndex($"{index}{client.AllowedCorsOrigins.IndexOf(x)}"),
                        ClientId = index,
                        Origin = x
                    }).ToList(),
                    ClientId = client.ClientId,
                    AllowedGrantTypes = client.AllowedGrantTypes.Select(x => new identityModels.ClientGrantType
                    {
                        Id = GenerateIndex($"{index}{client.AllowedGrantTypes.IndexOf(x)}"),
                        ClientId = index,
                        GrantType = x
                    }).ToList(),
                    AllowedScopes = client.AllowedScopes.Select(x => new identityModels.ClientScope
                    {
                        Id = GenerateIndex($"{index}{client.AllowedScopes.IndexOf(x)}"),
                        ClientId = index,
                        Scope = x
                    }).ToList(),
                    AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                    AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                    AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                    BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                    BackChannelLogoutUri = client.BackChannelLogoutUri,
                    Claims = client.Claims.Select(x => new identityModels.ClientClaim()
                    {
                        Id = GenerateIndex($"{index}{client.Claims.IndexOf(x)}"),
                        ClientId = index,
                        Value = x.Value,
                        Type = x.Type
                    }).ToList(),
                    ClientClaimsPrefix = client.ClientClaimsPrefix,
                    ClientName = client.ClientName,
                    ClientSecrets = client.ClientSecrets.Select(x => new identityModels.ClientSecret()
                    {
                        Id = GenerateIndex($"{index}{client.ClientSecrets.IndexOf(x)}"),
                        ClientId = index,
                        Value = x.Value,
                        Type = x.Type,
                        Created = DateTime.Now,
                        Description = x.Description,
                        Expiration = x.Expiration
                    }).ToList(),
                    Description = client.Description,
                    ClientUri = client.ClientUri,
                    ConsentLifetime = client.ConsentLifetime,
                    DeviceCodeLifetime = client.DeviceCodeLifetime,
                    EnableLocalLogin = client.EnableLocalLogin,
                    FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                    FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                    IdentityProviderRestrictions = client.IdentityProviderRestrictions.Select(x => new identityModels.ClientIdPRestriction
                    {
                        Id = GenerateIndex($"{index}{client.IdentityProviderRestrictions.IndexOf(x)}"),
                        ClientId = index,
                        Provider = x
                    }).ToList(),
                    IdentityTokenLifetime = client.IdentityTokenLifetime,
                    IncludeJwtId = client.IncludeJwtId,
                    LastAccessed = DateTime.Now,
                    LogoUri = client.LogoUri,
                    NonEditable = false,
                    PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                    PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(x => new identityModels.ClientPostLogoutRedirectUri
                    {
                        Id = GenerateIndex($"{index}{client.PostLogoutRedirectUris.IndexOf(x)}"),
                        ClientId = index,
                        PostLogoutRedirectUri = x
                    }).ToList(),
                    ProtocolType = client.ProtocolType,
                    RedirectUris = client.RedirectUris.Select(x => new identityModels.ClientRedirectUri
                    {
                        Id = GenerateIndex($"{index}{client.RedirectUris.IndexOf(x)}"),
                        ClientId = index,
                        RedirectUri = x
                    }).ToList(),
                    RefreshTokenUsage = (int)client.RefreshTokenUsage,
                    RefreshTokenExpiration = (int)client.RefreshTokenExpiration,
                    RequireClientSecret = client.RequireClientSecret,
                    RequireConsent = client.RequireConsent,
                    RequirePkce = client.RequirePkce,
                    SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                    UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                    Updated = DateTime.Now,
                    UserCodeType = client.UserCodeType,
                    UserSsoLifetime = client.UserSsoLifetime
                };

                yield return retClient;
            }

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