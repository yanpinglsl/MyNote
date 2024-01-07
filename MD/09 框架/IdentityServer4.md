# IdentityServer4 中文文档与实战

> 原文链接：https://www.cnblogs.com/stulzq/p/8119928.html

# 1. 介绍

## 1.1 特性一览

entityServer4是ASP.NET Core 2的OpenID Connect和OAuth 2.0框架。它可以在您的应用程序中提供以下功能：

它使你的应用程序具有如下特点：

- 认证即服务

  适用于所有应用程序（web, native, mobile, services）的集中登录逻辑和工作流程。IdentityServer是OpenID Connect的官方认证实现。

- 单点登录/注销

  多个类型的应用程序在一个点进行登录和注销操作。

- API 访问控制

  为各种类型的客户端颁发API的访问令牌，例如 服务器到服务器、Web应用程序，SPA、本地应用和移动应用程序。

-  联合网关

  支持Azure Active Directory，Google，Facebook等外部身份提供商。这可以保护您的应用程序免受如何连接到这些外部提供商的详细信息的影响

- 专注于定制

  最重要的部分 - IdentityServer的许多方面都可以根据您的需求进行定制。由于IdentityServer是一个框架而不是现成的产品或SaaS，因此您可以编写代码以使系统适应您的方案。

-  成熟的开源

  IdentityServer使用的Apache 2开源协议，允许在其上构建商业产品。它也是.NET Foundation的一部分，它提供治理和法律支持

-  免费和商业支持

  如果您需要帮助构建或运行您的身份平台，请告知IdentityServer官方。 他们可以通过多种方式为您提供帮助。

## 1.2 整体介绍

- 整体情况

  现代应用程序看起来更像这个：![image-20240104092203631](image/image-20240104092203631.png)

  最常见的相互作用：

  - 浏览器与Web应用程序的通信 Browser -> Web App
  - Web应用程序与Web API通信
  - 基于浏览器的应用程序与Web API
  - 本机应用程序与Web API进行沟通
  - 基于服务器的应用程序与Web API
  - Web API与Web API通信

  通常，每个层（前端、中间层和后端）必须保护资源并实现身份验证或授权——通常针对同一个用户存储区。 

  将这些基本安全功能外包给安全令牌服务可以防止在这些应用程序和端点上复制该功能。 

  应用支持安全令牌服务将引起下面的体系结构和协议：![image-20240104092236665](image/image-20240104092236665.png)

- 认证

  认证可以让一个应用程序知道当前用户的身份。 通常，这些应用程序代表该用户管理数据，并且需要确保该用户只能访问允许他访问的数据。最常见的示例是Web应用程序，但基于本地和基于js的应用程序也需要进行身份验证。 

  最常用的认证协议saml2p、WS-Federation和OpenID，saml2p协议是最流行和实际应用最多的。

  OpenID Connect对于现在应用来说是被认为是未来最有潜力的，这是专为移动应用场景设计的，一开始就被设计成对移动应用场景友好。

- API访问

  应用程序有两种基本方式与API进行通信，一种是使用应用程序标识，另一种是委托用户的身份。有时这两种方法都需要结合。

  OAuth2协议，它允许应用程序从一个安全令牌服务要求访问令牌，使用这个访问令牌来访问API。这个机制降低了客户机应用程序和API的复杂性，因为身份验证和授权可以是集中式的。

- OpenID Connect和OAuth2.0结合

  OpenID Connect 和 OAuth 2.0非常相似，事实上OpenID Connect 是在OAuth 2.0之上的一个扩展。两个基本的安全问题，认证和API访问，被组合成单个协议，通常只需一次往返安全令牌服务。

   我们认为OpenID Connect和OAuth 2.0的组合是可预见在未来是保护现代应用程序的最佳方法。IdentityServer4是这两种协议的实现，并且被高度优化以解决当今移动应用、本地应用和web应用的典型安全问题

- IdentityServer4可以帮助你做什么

  IdentityServer是将规范兼容的OpenID Connect和OAuth 2.0端点添加到任意ASP.NET Core应用程序的中间件。通常，您构建（或重新使用）包含登录和注销页面的应用程序，IdentityServer中间件会向其添加必要的协议头，以便客户端应用程序可以与其对话 使用这些标准协议。

## 1.3 术语的解释

- 身份认证服务器（IdentityServer）

  IdentityServer是一个OpenID Connect提供程序，它实现了OpenID Connect 和 OAuth 2.0 协议。

  同样的角色，不同的文档使用不同的术语。在有些文档中，它（IdentityServer）可能会被叫做安全令牌服务器（security token service）、身份提供者（identity provider）、授权服务器（authorization server）、 标识提供方（(IP-STS，什么是[IP-STS](https://msdn.microsoft.com/zh-cn/library/ee748489.aspx)）等等。

  但是它们都是一样的，都是向客户端发送安全令牌（security token），

  IdentityServer有许多功能：

  - 保护你的资源

  - 使用本地帐户或通过外部身份提供程序对用户进行身份验证

  - 提供会话管理和单点登录

  - 管理和验证客户机

  - 向客户发出标识和访问令牌

  - 验证令牌

- 用户（User）

  用户是使用注册的客户端访问资源的人。

- 客户端（Client）

  客户端是从IdentityServer请求令牌的软件，用于验证用户（请求身份令牌）或访问资源（请求访问令牌）。 必须首先向IdentityServer注册客户端才能请求令牌。

  客户端可以是Web应用程序，本地移动或桌面应用程序，SPA，服务器进程等。

- 资源（Resources）

  资源是您想要使用IdentityServer保护的资源 ， 您的用户的身份数据或API。

  每个资源都有一个唯一的名称 ，客户端使用这个名称来指定他们想要访问的资源。

  用户身份数据标识信息，比如姓名或邮件地址等。

  API资源，表示客户端想要调用的功能 ，通常被建模为Web API，但不一定。

- 身份令牌（Identity Token）

  身份令牌表示身份验证过程的结果。 它最低限度地标识了某个用户，还包含了用户的认证时间和认证方式。 它可以包含额外身份数据。

- 访问令牌（Access Token）

  访问令牌允许访问API资源。 客户端请求访问令牌并将其转发到API。 访问令牌包含有关客户端和用户的信息（如果存在）。 API使用该信息来授权访问其数据。

## 1.4 支持的规范

IdentityServer实现以下规范：

- OpenID Connect

  OpenID Connect Core 1.0 (spec)
  OpenID Connect Discovery 1.0 (spec)
  OpenID Connect Session Management 1.0 - draft 22 (spec)
  OpenID Connect HTTP-based Logout 1.0 - draft 03 (spec)

- OAuth 2.0
  OAuth 2.0 (RC-6749)
  OAuth 2.0 Bearer Token Usage (RFC 6750)
  OAuth 2.0 Multiple Response Types (spec)
  OAuth 2.0 Form Post Response Mode (spec)
  OAuth 2.0 Token Revocation (RFC 7009)
  OAuth 2.0 Token Introspection (RFC 7662)
  Proof Key for Code Exchange (RFC 7636)
  JSON Web Tokens for Client Authentication (RFC 7523)

## 1.5 包和构建说明

IdentityServer有许多Nuget包组件

- IdentityServer4
  nuget | github

  包含IdentityServer核心对象模型、服务和中间件。默认只包含了基于内存（In-Memory）的配置和用户信息的存储，主要用于快速学习、测试IdentityServer4，你可通过实现 IdentityServer4 提供的接口，来接入自定义持久化存储。

- Quickstart UI
  github

  包含一个简单的入门UI，包括登录，注销和授权询问页面。

- Access token validation middleware
  nuget | github

  用于验证API中令牌的ASP.NET Core身份验证处理程序。处理程序允许在同一API中支持JWT和reference Token。

- ASP.NET Core Identity
  nuget | github

  IdentityServer的ASP.NET Core Identity集成包。此包提供了一个简单的配置API，可以让IdentityServer用户使用ASP.NET Identity。

- EntityFramework Core
  nuget | github

  IdentityServer的EntityFramework Core存储实现。这个包提供了IdentityServer的配置和操作存储的EntityFramework Core实现。

- Dev builds
  此外，开发/临时构建将发布到MyGet。如果要尝试，请将以下包源添加到Visual Studio：https://www.myget.org/F/identity/

# 2. 快速入门

## 授权许可类型

- 客户端凭据许可：客户端模式只对客户端进行授权，不涉及到用户信息。如果你的api需要提供到第三方应用，第三方应用自己做用户授权，不需要用到你的用户资源，就可以用客户端模式，只对客户端进行授权访问api资源。
- 资源拥有者凭据许可：需要客户端提供用户名和密码，密码模式相较于客户端凭证模式。通过User的用户名和密码向Identity Server申请访问令牌。适用于当前的APP是专门为服务端设计的情况(比如为主项目设计的子项目）
- 隐式许可：没有后端的前端应用，即纯粹的js应用
- 授权码许可类型（参考OAuth2.0内容）

## 2.1 客户端授权模式

### 使用客户端认证保护API方式

此示例介绍了使用IdentityServer保护API的最基本场景。

在这种情况下，我们将定义一个**API**和要访问它的**客户端**。 客户端将在**IdentityServer**上请求访问令牌，并使用它来访问API。

### 2.1.1 准备

创建一个名为`QuickstartIdentityServer`的ASP.NET Core Web 空项目（.NET 8.0），端口5000
创建一个名为`Api`的ASP.NET Core Web Api 项目（.NET 8.0），端口6000
创建一个名为`Client`的控制台项目（.NET 8.0）

![image-20240104150153530](images/IdentityServer4/image-20240104150153530.png)



### 2.1.2 定义Identity资源

QuickstartIdentityServer授权中心

```C#
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
                    new ApiResource("api1", "My API"){ Scopes={ "api1" } }
                };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
                {
                    new ApiScope("api1")
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
                    #region 客户端模式授权
                    new Client
                    {
                        //客户端ID
                        ClientId = "client",

                        //AccessToken 过期时间，默认3600秒，注意这里直接设置5秒过期是不管用的，解决方案继续看下面 API资源添加JWT
                        //AccessTokenLifetime=5,

                        //配置授权类型，可以配置多个授权类型
                        //没有交互性用户，使用 clientid/secret 实现认证。
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        // 客户端加密方式
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },

                        //配置授权范围，这里指定哪些API 受此方式保护
                        AllowedScopes = { "api1" }
                    },
                    #endregion
                };
        }
    }
```

### 2.1.3 配置 IdentityServer

```C#
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    #region 客户端模式授权
    //注册IdentityServer，它还会注册一个基于内存存储的运行时状态
    builder.Services.AddIdentityServer(options =>
    {
        //options.Events.RaiseErrorEvents = true;
        //options.Events.RaiseInformationEvents = true;
        //options.Events.RaiseFailureEvents = true;
        //options.Events.RaiseSuccessEvents = true;
    })
        //开发模式下的签名证书，开发环境启用即可
        .AddDeveloperSigningCredential()
        //OpenID Connect相关认证信息配置
        //.AddInMemoryIdentityResources(Config.GetIdentityResources())
        //相关资源配置
        .AddInMemoryApiResources(Config.GetApis())//把受保护的Api资源添加到内存中                                                         
        .AddInMemoryApiScopes(Config.GetApiScopes()) //定义范围
        .AddInMemoryClients(Config.GetClients());//客户端配置添加到内存
    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    //app.UseHttpsRedirection();
    app.UseStaticFiles();

    //添加IdentityServer中间件
    app.UseIdentityServer();

    app.UseRouting();
    //app.UseAuthentication();
    //app.UseAuthorization();

    //app.MapRazorPages();

    app.Run();
}
```

运行此项目，打开浏览器访问`http://localhost:5000/.well-known/openid-configuration`你将会看到IdentityServer的各种元数据信息。![image-20240104150808859](images/IdentityServer4/image-20240104150808859.png)

首次启动时，IdentityServer将为您创建一个开发人员签名密钥，它是一个名为tempkey.rsa的文件。 您不必将该文件检入源代码管理中，如果该文件不存在，将重新创建该文件。

### 2.1.4 添加API

在项目`Api`中添加一个Controller：`IdentityController`

```C#
[Route("identity")]
[Authorize]
public class IdentityController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}
```

最后一步是将身份验证服务添加到DI和身份验证中间件到管道。 这些将：

- 验证传入令牌以确保它来自受信任的颁发者
- 验证令牌是否有效用于此API（也称为 audience）

将Program更新为如下所示：

```C#
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //引入 Microsoft.AspNetCore.Mvc.NewtonsoftJson
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        // 将身份认证服务添加到DI，并将“Bearer”配置为默认方案
        // AddJwtBearer 将 JWT 认证处理程序添加到DI中以供身份认证服务使用
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://localhost:5000";//配置IdentityServer的授权地址
                options.RequireHttpsMetadata = false;//不需要https
                options.Audience = "api1";//api的name,需要与config的名称相同
            });
        //需引入Swashbuckle.AspNetCore包
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
        });


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IEMS v1"));
        }
        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication(); // 将身份认证中间件添加到管道中，因此将在每次调用API时自动执行身份验证
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
```

`AddAuthentication` 将身份认证服务添加到DI，并将“Bearer”配置为默认方案。 `AddJwtBearer` 将 JWT 认证处理程序添加到DI中以供身份认证服务使用。 `UseAuthentication` 将身份认证中间件添加到管道中，因此将在每次调用API时自动执行身份验证。

如果在浏览器访问（http:// localhost:6000/identity），你会得到HTTP 401的结果。 这意味着您的API需要凭据。

就是这样，API现在受 IdentityServer 保护。

### 2.1.5 创建客户端

为 "Client" 项目添加 Nuget 包：`IdentityModel`

IdentityModel 包括用于发现 IdentityServer 各个终结点（EndPoint）的客户端库。这样您只需要知道 IdentityServer 的地址 - 可以从元数据中读取实际的各个终结点地址：

```C#
rivate static async Task Main()
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            // call api
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:6000/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JsonArray.Parse(content));
            }
            Console.ReadLine();
        }
```

> 默认情况下，Token将包含有关 Scope，生命周期（nbf和exp），客户端ID（client_id）和颁发者名称（iss）的身份信息单元（Claim）。

### 2.1.6 使用Postman调试

![image-20240104153057179](images/IdentityServer4/image-20240104153057179.png)



## 2.2 资源所有者密码授权模式

### 前言

OAuth 2.0 资源所有者密码模式允许客户端向令牌服务发送用户名和密码，并获取**代表该用户**的访问令牌。

除了无法通过浏览器进行交互的应用程序之外，通常建议不要使用资源所有者密码模式。 一般来说，当您要对用户进行身份验证并请求访问令牌时，使用其中一个交互式 OpenID Connect 流程通常要好得多。

在这里使用这种模式是为了学习如何快速在 IdentityServer 中使用它，

### 添加用户

就像API资源（也称为 Scope）、客户端一样，用户也有一个基于内存存储（In-Memory）的实现。

> 有关如何正确存储（持久化存储）和管理用户帐户的详细信息，请查看基于 ASP.NET Identity的快速入门。

`TestUser` 类代表测试用户及其身份信息单元（Claim）。 让我们通过在 `config` 类中添加以下代码来创建几个用户：

首先添加以下语句 到`Config.cs`文件中：

```C#
#region 资源所有者密码授权模式
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
#endregion
```

然后将测试用户注册到 IdentityServer：

```C#
#region 资源所有者密码授权模式
builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddInMemoryApiResources(Config.GetApis())
    .AddInMemoryClients(Config.GetClients())
    .AddInMemoryApiScopes(Config.GetApiScopes())
    .AddTestUsers(Config.GetUsers());//将测试用户注册到 IdentityServer：
#endregion
```

添加客户端定义

你可以通过修改 ·AllowedGrantTypes· 属性简单地添加对已有客户端授权类型的支持。

通常你会想要为资源所有者用例创建独立的客户端，添加以下代码到你配置中的客户端定义中：

```C#
#region 资源所有者密码授权模式
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
            ClientId = "ro.client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "group1" }
        }
    };
}
#endregion
```

### 创建客户端

```C#
#region 资源所有者密码授权模式
// request token
var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "ro.client",
    ClientSecret = "secret",
    //此处必须与TestUser对象保持一致
    UserName = "alice",
    Password = "password",
    Scope = "group1"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}
#endregion
```

当您将令牌发送到身份API终结点时，您会注意到与客户端模式相比有一个小但重要的区别。 访问令牌现在将包含唯一标识用户的`sub` claim。 通过在调用API之后检查内容变量可以看到这个“sub”，并且控制器应用程序也会在屏幕上显示该claim。

sub claim的存在（或不存在）允许API区分代表客户端的调用和代表用户的调用

下面这张图，是理解的客户端请求流程![image-20240104194006343](images/IdentityServer4/image-20240104194006343.png)

> 关于上图的补充说明，这里讲一下。api资源收到第一个请求之后，会去id4服务器公钥，然后用公钥验证token是否合法，如果合法进行后面后面的有效性验证。有且只有第一个请求才会去id4服务器请求公钥，后面的请求都会用第一次请求的公钥来验证，这也是jwt去中心化验证的思想。

### 使用Postman调试

![image-20240104194350848](images/IdentityServer4/image-20240104194350848.png)

## 2.3 简化流程授权模式

## 2.4 第三方登录

## 2.5 混合流程授权模式

## 2.6 授权码模式

## 2.7 Entity Framework存储配置和操作数据库

## 2.8 ASP.NET Core Identity
