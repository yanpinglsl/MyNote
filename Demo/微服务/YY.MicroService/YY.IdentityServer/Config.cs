using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
namespace YY.IdentityServer
{
    public static class Config
    {
        private static readonly IEnumerable<string> UserClaims = new[]
        {
            "UserId",
            "UserRole"
        };

        /// <summary>
        /// API资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
           {
                new ApiResource("YY")
                {
                        Scopes = new List<string>
                        {
                            "YY"
                        }
                },
                new ApiResource("Ocelot")
                {
                    Scopes = new List<string>
                    {
                        "Ocelot"
                    }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
                {
                    new ApiScope("YY",UserClaims),
                    new ApiScope("Ocelot")
                    //new ApiScope("group1")
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
                new Client
                {
                    ClientId = "Postman_Client",
                    ClientSecrets =
                    {
                        new Secret("123456".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = {"YY"}
                },
                new Client
                {
                    ClientId = "Ocelot_Client",
                    ClientSecrets =
                    {
                        new Secret("123456".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"Ocelot"}
                }
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
                        new Claim("UserId", "A"),
                        new Claim("UserRole", "Admin")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("UserId", "B"),
                        new Claim("UserRole", "User")
                    }
                }
            };
        }
    }
}

