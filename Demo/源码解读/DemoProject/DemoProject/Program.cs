using DemoProject.Models;
using DemoProject.Utility;
using DemoProject.Utility.GraphEndpoint;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Drawing;
using System.Text.Json;
using WebCoreExtend.ConfigurationExtend;
using WebCoreExtend.ConventionExtend;
using WebCoreExtend.LogExtend;
using WebCoreExtend.MiddlewareExtend;
using WebCoreExtend.MiddlewareExtend.SimpleExtend;
using WebCoreExtend.MiddlewareExtend.StandardMiddlewareExtend;
using WebCoreExtend.RouteExtend;
using WebCoreExtend.RouteExtend.DynamicRouteExtend;
using WebCoreExtend.StartupExtend;
using WebCoreExtend.AuthenticationExtend;
using System.Security.Claims;
using WebCoreExtend.AuthorizationExtend.Requirement;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;
using WebCoreExtend.JWTExtend;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace DemoProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region 创建WebApplicationBuilder
            var builder = WebApplication.CreateBuilder(args);
            #endregion

            #region 配置WebApplicationBuilder

            #region Configuration
            //默认就有appsettings.json---commandline
            builder.Configuration.AddJsonFile("customjson.json", true, true);
            builder.Configuration.AddXmlFile("customxml.xml", true, true);
            builder.Configuration.AddIniFile("customini.ini", true, true);
            //builder.Configuration.AddCommandLine();
            //还支持各种数据源

            #region 自定义模式

            ////((IConfigurationBuilder)builder.Configuration).Add(new CustomConfigurationSource());

            ////builder.Configuration.Add(new CustomConfigurationSource());
            ////A类 实现了B接口，B接口是有Add方法   
            ////但是用A类实例直接去调用Add方法报错
            ////但如果把A强制转换成B接口类型，就不报错了
            ////因为接口的Add方法是显式实现，那么调用时就必须转换成B接口类型---IConfigurationBuilder.Add

            //builder.Configuration.AddCustomConfiguration();
            #endregion

            #region 内存Provider
            var memoryConfig = new Dictionary<string, string>
                {
                   {"TodayMemory", "Memory配置"},
                   {"MemoryOptions:HostName", "192.168.200.104"},
                   {"MemoryOptions:UserName", "Memory"},
                   {"MemoryOptions:Password", "Memory8888"}
                };
            builder.Configuration.AddInMemoryCollection(memoryConfig);
            #endregion

            #endregion

            #region 日志组件扩展
            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole()
            //    .AddDebug();
            //builder.Logging.AddLog4Net();//Microsoft.Extensions.Logging.Log4Net.AspNetCore

            //builder.Logging.AddFilter("System", LogLevel.Warning);//System开头，warning及以上级别
            //builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

            #region 自定义日志组件
            ////builder.Logging.AddProvider(new CustomConsoleLoggerProvider());//最初级的，不标准
            ////builder.Logging.AddProvider(new CustomConsoleLoggerProvider(new CustomConsoleLoggerOptions()
            ////{
            ////    MinLogLevel = LogLevel.Warning,
            ////    ConsoleColor = ConsoleColor.Green,
            ////}));

            ////builder.Logging.AddCustomLogger();
            ////builder.Logging.AddCustomLogger(new CustomConsoleLoggerOptions()
            ////{
            ////    MinLogLevel = LogLevel.Warning,
            ////    ConsoleColor = ConsoleColor.Green,
            ////});
            //builder.Logging.AddCustomLogger(options =>
            //{
            //    options.MinLogLevel = LogLevel.Debug;
            //    options.ConsoleColor = ConsoleColor.Yellow;

            //});

            #endregion

            #endregion


            #endregion

            #region IOC注册
            // Add services to the container.
            builder.Services.AddControllersWithViews(services =>
            {
                #region Conventions
                //services.Conventions.Add(new CustomControllerModelConvention());//全局式注册Conventions
                #endregion
            });

            #region Routing
            builder.Services.AddRouting(options => {
                options.ConstraintMap.Add("GenderConstraint", typeof(CustomGenderRouteConstraint));
            });
            builder.Services.AddDynamicRoute();
            #endregion

            #region IStartupFilter拓展
            //builder.Services.AddTransient<IStartupFilter, CustomStartupFilter>();
            #endregion

            #region MiddleWare
            //builder.Services.AddSingleton<SecondMiddleware>();
            //builder.Services.Replace(ServiceDescriptor.Singleton<IMiddlewareFactory, SecondNewMiddlewareFactory>());
            //builder.Services.AddBrowserFilter(options =>
            //{
            //    options.EnableEdge = false;
            //});
            #endregion


            #region Options
            //①默认名称
            builder.Services.Configure<EmailOptions>(op =>
            {
                op.Title = "Title--DefaultName";
                op.From = "From---DefaultName";
            });
            //②指定名称
            builder.Services.Configure<EmailOptions>("FromMemory", op =>
            {
                op.Title = "Title---FromMemory";
                op.From = "From---FromMemory";
            });
            //③从配置文件读取
            builder.Services.Configure<EmailOptions>("FromConfiguration", builder.Configuration.GetSection("Email"));
            //④等价于②
            builder.Services.AddOptions<EmailOptions>("AddOption").Configure(op =>
            {
                op.Title = "Title---AddOption";
                op.From = "From---AddOption";
            });
            ////⑤当配置名为null时，会更新所有EmailOptions配置
            //builder.Services.Configure<EmailOptions>(null, op =>
            //{
            //    op.Title = "Title---Configure--null";
            //    op.From = "From---Configure--null";
            //});
            ////⑥等价于⑤
            //builder.Services.ConfigureAll<EmailOptions>(op => op.From = "ConfigureAll");

            ////⑦等价于⑤，PostConfiger可在Configer基础上继续配置
            //builder.Services.PostConfigure<EmailOptions>(null, op =>
            //{
            //    op.Title = "Title---PostConfigure--null";
            //    op.From = "From---PostConfigure--null";
            //});
            //builder.Services.PostConfigureAll<EmailOptions>(op => op.Body = "services.PostConfigure<EmailOption>--Name null--Same With PostConfigureAll");
            #endregion

            #region 静态文件之文件夹浏览
            builder.Services.AddDirectoryBrowser();
            #endregion

            #region Session
            builder.Services.AddSession();

            builder.Services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "127.0.0.1:6379";
                options.InstanceName = "RedisDistributedCache123";
            });
            #endregion

            #region 鉴权授权

            #region 鉴权
            //builder.Services.AddAuthentication();//鉴权相关的IOC注册--还不够，因为凭证有很多方式--Cookie--JWT
            #region 自定义鉴权-UrlToken
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    //其实会保存成key-value     也就是name不能重复  value就是UrlTokenAuthenticationHandler
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //});
            #endregion

            #region Cookie
            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //     {
            //         //options.ExpireTimeSpan//过期时间
            //         options.LoginPath = "/Home/Index";//未登录，则跳转至/Home/Index页面
            //         options.AccessDeniedPath = "/Home/Privacy";//有登陆，但未授权，则跳转至/Home/Privacy页面
            //     });//使用Cookie的方式
            #endregion
            
            #region 多Scheme鉴权
            //默认scheme是UrlTokenScheme
            builder.Services.AddAuthentication(options =>
            {
                options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
                //其实会保存成key-value     也就是name不能重复  value就是UrlTokenAuthenticationHandler
                options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //options.ExpireTimeSpan//过期时间
                options.LoginPath = "/Home/Index";//未登录，则跳转至/Home/Index页面
                options.AccessDeniedPath = "/Home/Privacy";//有登陆，但未授权，则跳转至/Home/Privacy页面
            })//使用Cookie的方式
            //.AddJWT
            ;
            #endregion

            #region CustomAdd
            builder.Services.Replace(ServiceDescriptor.Scoped<IClaimsTransformation, CustomClaimsTransformation>());
            #endregion
            #endregion

            #region 授权
            builder.Services.AddSingleton<IAuthorizationHandler, DateOfBirthRequirementHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, CountryRequirementHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();//接口和父类分开，就得IOC注入
            builder.Services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            //builder.Services.AddAuthorization();//授权相关的IOC注册--在AddMVC已经有了
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin");
                });//等价于  Roles=Admin
                options.AddPolicy("MutiPolicy", policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin")//都属于框架封装好的
                    .RequireUserName("Eleven")//Role  UserName都是最常用的
                    .RequireClaim(ClaimTypes.Country)//只要求有Country属性
                    .RequireAssertion(context =>//可以灵活的扩展规则--cliam之外其他信息也可以IP等
                    {
                        return context.User.HasClaim(c => c.Type == ClaimTypes.Email)
                        && context.User.Claims.First(c => c.Type == ClaimTypes.Email).Value.Equals("57265177@qq.com");
                    })
                    .RequireAssertion(context =>//ClaimTypes和字符串的区别
                    {
                        return context.User.HasClaim(c => c.Type == "Email")
                        && context.User.Claims.First(c => c.Type == "Email").Value.Equals("12345678@163.com");
                    });
                    //.AddRequirements(new SingleEmailRequirement("@qq.com"));


                    policyBuilder.Requirements.Add(new SingleEmailRequirement("@qq.com"));
                    policyBuilder.Requirements.Add(new DateOfBirthRequirement());
                    policyBuilder.Requirements.Add(new DoubleEmailRequirement());

                    //policyBuilder.Requirements.Add(new SingleEmailRequirement("@qq.com"));
                    //policyBuilder.Combine(new AuthorizationPolicyBuilder().AddRequirements(new SingleEmailRequirement("@qq.com")).Build());
                });
            });



            #endregion

            #endregion

            #region JWT鉴权+授权  HS方式
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
                  .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间---默认还添加了300s后才过期
                    ClockSkew = TimeSpan.FromSeconds(0),//token过期后立马过期
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey

                    ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设置一致
                    ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//拿到SecurityKey
                };
            });
            #endregion

            #region JWT鉴权+授权  RS方式
            //  JWTTokenOptions tokenOptions = new JWTTokenOptions();
            //  builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
            //  //这里的SecurityKey其实没有意义了,换成下面的公钥
            //  #region 读取RSA的Key
            //  string path = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            //  string key = File.ReadAllText(path);
            //  Console.WriteLine($"KeyPath:{path}");
            //  var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            //  var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);
            //  #endregion
            //  builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,//是否验证Issuer
            //        ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致

            //        ValidateAudience = true,//是否验证Audience
            //        ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设置一致

            //        ValidateLifetime = true,//是否验证失效时间
            //        ClockSkew = TimeSpan.FromSeconds(0),//token过期后立马过期

            //        ValidateIssuerSigningKey = true,//是否验证SecurityKey
            //        IssuerSigningKey = new RsaSecurityKey(keyParams),

            //        IssuerSigningKeyValidator = (m, n, z) =>
            //        {
            //            Console.WriteLine("This is IssuerValidator");
            //            return true;
            //        },//自定义校验过程

            //        IssuerValidator = (m, n, z) =>
            //        {
            //            Console.WriteLine("This is IssuerValidator");
            //            return "http://localhost:5726";
            //        },//自定义校验过程
            //        AudienceValidator = (m, n, z) =>
            //        {
            //            Console.WriteLine("This is AudienceValidator");
            //            return true;
            //            //return m != null && m.FirstOrDefault().Equals(this.Configuration["Audience"]);
            //        },//自定义校验规则，可以新登录后将之前的无效
            //    };

            //    #region Events
            //    //即提供了委托扩展，也可以直接new新对象，override方法
            //    options.Events = new JwtBearerEvents()
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            Console.WriteLine($"This JWT Authentication OnAuthenticationFailed");
            //            if (context.Exception.GetType().Name.Equals("SecurityTokenExpiredException"))
            //            {
            //                context.Response.Headers.Add("JWTAuthenticationFailed", "1");//
            //            }
            //            return Task.CompletedTask;
            //        },
            //        OnChallenge = context =>
            //        {
            //            Console.WriteLine($"This JWT Authentication OnChallenge");
            //            context.Response.Headers.Add("JWTChallenge", "expired");//告诉客户端是过期了
            //            return Task.CompletedTask;
            //        },
            //        OnForbidden = context =>
            //        {
            //            Console.WriteLine($"This JWT Authentication OnForbidden");
            //            context.Response.Headers.Add("JWTForbidden", "1");//
            //            return Task.CompletedTask;
            //        },
            //        OnMessageReceived = context =>
            //        {
            //            Console.WriteLine($"This JWT Authentication OnMessageReceived");
            //            context.Response.Headers.Add("JWTMessageReceived", "1");//
            //            return Task.CompletedTask;
            //        },
            //        OnTokenValidated = context =>
            //        {
            //            Console.WriteLine($"This JWT Authentication OnTokenValidated");
            //            context.Response.Headers.Add("JWTTokenValidated", "1");//
            //            return Task.CompletedTask;
            //        }
            //    };
            //    #endregion
            //});

            #endregion

            #endregion

            #region Build
            var app = builder.Build();
            #endregion

            #region Use中间件

            #region 最原生Use中间件--来配置管道模型
            //#region 任意扩展
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 0.5");
            //    return new RequestDelegate(
            //       async context =>
            //       {
            //           context.Response.ContentType = "text/html";
            //           await context.Response.WriteAsync("This is Hello World 0.5 start </br>");
            //           await next.Invoke(context);
            //           await context.Response.WriteAsync("This is Hello World 0.5 end </br>");
            //       });
            //});

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 0.8");
            //    return new RequestDelegate(
            //       async context =>
            //       {
            //           await context.Response.WriteAsync("This is Hello World 0.8 start </br>");
            //           await next.Invoke(context);
            //       });
            //});
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 5");
            //    return new RequestDelegate(
            //       async context =>
            //       {
            //           await next.Invoke(context);
            //           await context.Response.WriteAsync("This is Hello World 5 end </br>");
            //       });
            //});
            //#endregion

            //app.Use(new Func<RequestDelegate, RequestDelegate>(
            //    next =>
            //    { 
            //        Console.WriteLine("This is middleware 1");
            //        return new RequestDelegate(
            //           async context =>
            //           {
            //                context.Response.ContentType = "text/html";
            //               //context.Response.ContentType = "text/html";
            //               await context.Response.WriteAsync("This is Hello World 1 start </br>");
            //               await next.Invoke(context);
            //               await context.Response.WriteAsync("This is Hello World 1 end </br>");
            //           });
            //    }));

            //#region 任意扩展
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 1.8");
            //    return new RequestDelegate(
            //       async context =>
            //       {
            //           await context.Response.WriteAsync("This is Hello World 1.8 start </br>");
            //           await next.Invoke(context);
            //           await context.Response.WriteAsync("This is Hello World 1.8 end </br>");
            //       });
            //});
            //#endregion

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 2");
            //    return new RequestDelegate(
            //       async context =>
            //       {
            //           await context.Response.WriteAsync("This is Hello World 2 start </br>");
            //           await next.Invoke(context);
            //           await context.Response.WriteAsync("This is Hello World 2 end </br>");
            //       });
            //});
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 3");
            //    return new RequestDelegate(
            //       async context =>
            //       {
            //           await context.Response.WriteAsync("This is Hello World 3 start </br>");
            //           //await next.Invoke(context);//没有下个中间件
            //           await context.Response.WriteAsync("This is The Chooen One! </br>");
            //           await context.Response.WriteAsync("This is Hello World 3 end </br>");
            //       });
            //});
            #endregion

            #region 各种内置扩展用法
            #region Use
            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("use1 start");
            //    await next.Invoke();
            //    Console.WriteLine("use1 end");
            //});
            #endregion

            #region Run
            ////Run相当于一个终结点，Run之后的中间件不会被执行，因为它不像Use一样可以调用next.Invoke();
            //app.Run(context =>
            //{
            //    Console.WriteLine("run");
            //    return context.Response.WriteAsync("Run,Hello World!");
            //});
            #endregion

            #region Map
            //只有访问特定的路径才会执行
            //app.Map("/map", app =>
            //{
            //    app.Run(context =>
            //    {
            //        Console.WriteLine("map");
            //        return context.Response.WriteAsync("Map,Hello World!");
            //    });
            //});
            #endregion

            #region MapWhen
            ////当条件成立时，中间件才会被执行，并且MapWhen创建了一个新的管道，当满足条件时，新的管道会代替主管道，这意味着主管道的中间件不会被执行
            //app.MapWhen(context =>
            //{
            //    return context.Request.Query.ContainsKey("Name");
            //    //拒绝非chorme浏览器的请求  		
            //    //多语言		
            //    //把ajax统一处理		
            //}, app =>
            //{
            //    app.Use(async (context, next) =>
            //    {
            //        Console.WriteLine("mapwhen1 start ");
            //        await next.Invoke();
            //        Console.WriteLine("mapwhen1 end");
            //        await context.Response.WriteAsync("Url is " + context.Request.QueryString.ToString());
            //    });
            //});

            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("use2 start ");
            //    await next.Invoke();
            //    Console.WriteLine("use2 end ");
            //});
            #endregion

            #region UseWhen
            ////UseWhen和MapWhen类似，也是当条件成立时，中间件才会被执行，区别是UseWhen不会代替主管道
            //app.UseWhen(context =>
            //{
            //    return context.Request.Query.ContainsKey("Name");
            //    //拒绝非chorme浏览器的请求  		
            //    //多语言		
            //    //把ajax统一处理		
            //}, app =>
            //{
            //    app.Use(async (context, next) =>
            //    {
            //        Console.WriteLine("usewhen1 start");
            //        await next.Invoke();
            //        Console.WriteLine("usewhen1 end");
            //        await context.Response.WriteAsync("Url is " + context.Request.QueryString.ToString());
            //    });
            //});


            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("use2 start ");
            //    await next.Invoke();
            //    Console.WriteLine("use2 end ");
            //});
            #endregion


            #endregion

            #region UseMiddleware式--用类
            //自定义中间件类-无参数
            //app.UseMiddleware<FirstMiddleware>();
            ////实现IMiddleWare接口
            //app.UseMiddleware<SecondMiddleware>();
            //实现IMiddleWare接口-有参数
            //app.UseMiddleware<ThirdMiddleware>("DemoProject");
            #endregion

            #region 标准自定义中间件
            //app.UseBrowserFilter();
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is standard middleware");
            //    return new RequestDelegate(
            //       async context =>
            //       {
            //           context.Response.ContentType = "text/html";
            //           await context.Response.WriteAsync("This is standard middleware start </br>");
            //           await next.Invoke(context);
            //           await context.Response.WriteAsync("This is standard middleware end </br>");
            //       });
            //});
            #endregion

            #region 框架默认中间件
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseCustomException();
                //app.UseExceptionHandler("/Home/CustomError");
                //app.UseExceptionHandler(build=> build.Use(ExceptionHandlerDemo));
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            //async Task ExceptionHandlerDemo(HttpContext httpContext, Func<Task> next)
            //{
            //    //该信息由ExceptionHandlerMiddleware中间件提供，里面包含了ExceptionHandlerMiddleware中间件捕获到的异常信息。
            //    var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            //    var ex = exceptionDetails?.Error;

            //    if (ex != null)
            //    {
            //        httpContext.Response.ContentType = "application/problem+json";

            //        var title = "An error occured: " + ex.Message;
            //        var details = ex.ToString();

            //        var problem = new ProblemDetails
            //        {
            //            Status = 500,
            //            Title = title,
            //            Detail = details
            //        };

            //        var stream = httpContext.Response.Body;
            //        await JsonSerializer.SerializeAsync(stream, problem);
            //    }
            //}

            app.UseHttpsRedirection();


            #region 防盗链
            app.UseHotlinkingPreventionMiddleware();
            #endregion

            #region 静态文件配置
            app.UseStaticFiles();

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),//指定路径---还有provider
            //    ServeUnknownFileTypes = false,//
            //    OnPrepareResponse = context =>
            //    {
            //        //响应请求之前，才可以修改header
            //        context.Context.Response.Headers[HeaderNames.CacheControl] = "no-store";//"no-cache";//
            //    }
            //});

            ////配置静态文件中间件
            //var provider = new FileExtensionContentTypeProvider();
            ////追加未知类型到MIME映射
            ////provider.Mappings.Add(".ini", "text/plain");
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticFiles")),
            //    RequestPath = new PathString("/StaticFiles"),
            //    ServeUnknownFileTypes = true,
            //    DefaultContentType = "image/jpeg", 
            //    //ContentTypeProvider = provider, //如果不配置该项则.ini文件将不能被访问，仅支持访问部分类型的文件，具体参考FileExtensionContentTypeProvider类
            //    OnPrepareResponse = (context) =>
            //    {
            //        //响应请求之前，才可以修改header
            //        //context.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");//配置可跨域访问
            //    }
            //});
            //app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
            //    RequestPath = "/StaticDirectory"
            //});//其实是个后门
            #endregion

            #region Session
            app.UseSession();
            #endregion


            #region graph
            //http://localhost:5726/graph
            //https://graphviz.christine.website/
            //使用该中间件，可以显示出RoutePattern矢量图
            app.Map("/graph", branch => branch.UseMiddleware<GraphEndpointMiddleware>());
            #endregion

            #region UseRouting
            app.UseRouting();
            #endregion

            #region 鉴权授权
            //app.UseAuthentication();//告诉框架，请求来了，需要做鉴权--检查下登陆信息
            app.UseAuthorization();//配置每次请求都要经过这个环节的处理,做授权检测的

            #endregion

            #region MapControllerRoute

            app.MapControllerRoute(
              name: "about-route",
              pattern: "about",
              defaults: new { controller = "Route", action = "About" }
              );

            //伪静态路由
            app.MapControllerRoute(
               name: "static",
               pattern: "static/{id:int}.html",
               defaults: new { controller = "Route", action = "StaticPage" });

            //特性路由：当年MVC的WebAPI，是需要单独启动的---现在不用了

            //约束
            //http://localhost:5726/hello/11e  
            //http://localhost:5726/hello/11
            //http://localhost:5726/hello/eleven
            app.MapGet("/hello/{name}", async context =>
            {
                var name = context.Request.RouteValues["name"];
                await context.Response.WriteAsync($"Hello {name} no constraint !");
            });  //处理动作   http://localhost:5726/hello/11e

            app.MapGet("/hello/{name:alpha}", async context =>
            {
                var name = context.Request.RouteValues["name"];
                await context.Response.WriteAsync($"Hello {name} alpha!");
            }); //处理动作   http://localhost:5726/hello/eleven  

            app.MapGet("/hello/{name:int}", async context =>
            {
                var name = context.Request.RouteValues["name"];
                await context.Response.WriteAsync($"Hello {name} int!");
            }); //处理动作   http://localhost:5726/hello/11


            //正则路由---优先匹配满足约束的  不够约束才走的默认路由
            //优先级顺序：range--正则---默认

            // http://localhost:5726/Route/Data/2019-11  三个都满足，但是找最精准的rang
            // http://localhost:5726/Route/Data/2019-13  满足2个，找更精准的
            // http://localhost:5726/Route/Data/2018-09  满足2个，找更精准的
            // http://localhost:5726/Route/Data/2018-9   只满足默认路由
            // http://localhost:5726/Route/Data?year=2019&month=11  默认路由
            app.MapControllerRoute(
               name: "range",
               pattern: "{controller=Home}/{action=Index}/{year:range(2019,2021)}-{month:range(1,12)}");

            app.MapControllerRoute(
                name: "regular",
                pattern: "{controller}/{action}/{year}-{month}",
                constraints: new { year = "^\\d{4}$", month = "^\\d{2}$" },
                defaults: new { controller = "Home", action = "Index", });

            //自定义约束

            //动态路由
            app.UseDynamicRouteDefault();

            //默认路由
            app.MapControllerRoute(
                name: "default",//路由的key---支持多个路由，key-value存储，所以不要重复key
                pattern: "{controller=Home}/{action=Index}/{id?}");//路由规则：
            //http://localhost:5726/home/index


            //MapGet指定处理方式---MinimalAPI
            //http://localhost:5726/ElevenTest
            app.MapGet("/ElevenTest", async context =>
            {
                await context.Response.WriteAsync($"This is ElevenTest");
            })
            //.RequireAuthorization();//要求授权
            //.WithMetadata(new AuditPolicyAttribute());//路由命中的话，可以多加个特性
            ;

            //app.MapPut

            #endregion

            #endregion

            #endregion

            #region Run
            app.Run();
            #endregion
        }
    }
}