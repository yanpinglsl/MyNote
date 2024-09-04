using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.DataProtection;
using System.Collections.Generic;
using System.Security.Claims;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer
{
    public static class Config
    {
        /// <summary>
        /// 用户认证信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                    new IdentityResources.OpenId(),//添加对标准 openid（subject id）的支持
                    new IdentityResources.Profile(),//添加对标准profile （名字，姓氏等）Scope的支持
                                                    //new IdentityResources.Address(),
                                                    //new IdentityResources.Email(),
                                                    //new IdentityResources.Phone()
            };
        }

        /// <summary>
        /// API资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
                {
                    //new ApiResource("group1", "My API"){ Scopes={ "group1" } }
                    new ApiResource("group1", "My API")
                };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
                {
                    new ApiScope("group1")
                };
        }

        /// <summary>
        /// 客户端应用
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                #region 客户端模式授权
                //new Client
                //{
                //    //客户端ID
                //    ClientId = "client",

                //    //AccessToken 过期时间，默认3600秒，注意这里直接设置5秒过期是不管用的，解决方案继续看下面 API资源添加JWT
                //    //AccessTokenLifetime=5,

                //    //配置授权类型，可以配置多个授权类型
                //    //没有交互性用户，使用 clientid/secret 实现认证。
                //    AllowedGrantTypes = GrantTypes.ClientCredentials,

                //    // 客户端加密方式
                //    ClientSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    },

                //    //配置授权范围，这里指定哪些API 受此方式保护
                //    AllowedScopes = { "group1" }
                //},
                #endregion
                
                #region 资源所有者密码授权模式
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "group1" }
                },
                #endregion
                
                #region OpenID Connect
                //new Client
                //{
                //    ClientId = "mvc",
                //    ClientName = "MVC Client",
                //    AllowedGrantTypes = GrantTypes.Implicit,

                //    // 登录后跳转的页面
                //    RedirectUris = { "https://localhost:4001/signin-oidc" },
                    
                //    // 登出后跳转的页面
                //    PostLogoutRedirectUris = { "https://localhost:4001/signout-callback-oidc" },

                //    AllowedScopes = new List<string>
                //        {
                //            IdentityServerConstants.StandardScopes.OpenId,
                //            IdentityServerConstants.StandardScopes.Profile
                //        },
                //    AllowAccessTokensViaBrowser = true,
                //    // 是否需要同意授权 （默认是false）
                //    RequireConsent = true
                //},
                #endregion

                #region 混合模式
                //new Client
                //{
                //    ClientId = "hybrid",
                //    ClientName = "MVC Client",
                //    AllowedGrantTypes = GrantTypes.Hybrid,
                //    ClientSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    },

                //    RedirectUris           = { "https://localhost:4001/signin-oidc" },
                //    PostLogoutRedirectUris = { "https://localhost:4001/signout-callback-oidc" },

                //    AllowedScopes =
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "group1"
                //    },
                //    //AllowOfflineAccess = true 
                //    //允许将token通过浏览器传递
                //    AllowAccessTokensViaBrowser=true,
                //    // 是否需要同意授权 （默认是false）
                //    RequireConsent = true
                //}
            #endregion
        };
        }
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "1",
                        Username = "alice",
                        Password = "password",

                        Claims = new []
                        {
                            new Claim("name", "Alice"),
                            new Claim("website", "https://alice.com")
                        }
                    },
                    new TestUser
                    {
                        SubjectId = "2",
                        Username = "bob",
                        Password = "password",

                        Claims = new []
                        {
                            new Claim("name", "Bob"),
                            new Claim("website", "https://bob.com")
                        }
                    }
                };
        }
    }

}
