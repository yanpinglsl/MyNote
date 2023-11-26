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
using WebCoreExtend.FilterExtend.RegisterWayShow;
using WebCoreExtend.FilterExtend.DIShow;

namespace DemoProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region ����WebApplicationBuilder
            var builder = WebApplication.CreateBuilder(args);
            #endregion

            #region ����WebApplicationBuilder

            #region Configuration
            //Ĭ�Ͼ���appsettings.json---commandline
            builder.Configuration.AddJsonFile("customjson.json", true, true);
            builder.Configuration.AddXmlFile("customxml.xml", true, true);
            builder.Configuration.AddIniFile("customini.ini", true, true);
            //builder.Configuration.AddCommandLine();
            //��֧�ָ�������Դ

            #region �Զ���ģʽ

            ////((IConfigurationBuilder)builder.Configuration).Add(new CustomConfigurationSource());

            ////builder.Configuration.Add(new CustomConfigurationSource());
            ////A�� ʵ����B�ӿڣ�B�ӿ�����Add����   
            ////������A��ʵ��ֱ��ȥ����Add��������
            ////�������Aǿ��ת����B�ӿ����ͣ��Ͳ�������
            ////��Ϊ�ӿڵ�Add��������ʽʵ�֣���ô����ʱ�ͱ���ת����B�ӿ�����---IConfigurationBuilder.Add

            //builder.Configuration.AddCustomConfiguration();
            #endregion

            #region �ڴ�Provider
            var memoryConfig = new Dictionary<string, string>
                {
                   {"TodayMemory", "Memory����"},
                   {"MemoryOptions:HostName", "192.168.200.104"},
                   {"MemoryOptions:UserName", "Memory"},
                   {"MemoryOptions:Password", "Memory8888"}
                };
            builder.Configuration.AddInMemoryCollection(memoryConfig);
            #endregion

            #endregion

            #region ��־�����չ
            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole()
            //    .AddDebug();
            //builder.Logging.AddLog4Net();//Microsoft.Extensions.Logging.Log4Net.AspNetCore

            //builder.Logging.AddFilter("System", LogLevel.Warning);//System��ͷ��warning�����ϼ���
            //builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

            #region �Զ�����־���
            ////builder.Logging.AddProvider(new CustomConsoleLoggerProvider());//������ģ�����׼
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

            #region IOCע��
            // Add services to the container.
            builder.Services.AddControllersWithViews(services =>
            {
                #region ȫ��ע��Filter
                //services.Filters.Add(new CustomGlobalRegisterActionFilterAttribute());//ȫ��ע���Filter
                #endregion

                #region Filterע��
                //services.Filters.Add<CustomDIActionFilterAttribute>();//ȫ��ʽ
                #endregion

                #region Conventions
                //services.Conventions.Add(new CustomControllerModelConvention());//ȫ��ʽע��Conventions
                #endregion
            });

            #region ServiceFilterע����Ҫ---CustomIOCFilterFactoryע����Ҫ
            builder.Services.AddTransient<CustomDIActionFilterAttribute>();
            //builder.Services.AddTransient<CustomDisposeActionFilterAttribute>();
            #endregion

            #region Routing
            builder.Services.AddRouting(options => {
                options.ConstraintMap.Add("GenderConstraint", typeof(CustomGenderRouteConstraint));
            });
            builder.Services.AddDynamicRoute();
            #endregion

            #region IStartupFilter��չ
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
            //��Ĭ������
            builder.Services.Configure<EmailOptions>(op =>
            {
                op.Title = "Title--DefaultName";
                op.From = "From---DefaultName";
            });
            //��ָ������
            builder.Services.Configure<EmailOptions>("FromMemory", op =>
            {
                op.Title = "Title---FromMemory";
                op.From = "From---FromMemory";
            });
            //�۴������ļ���ȡ
            builder.Services.Configure<EmailOptions>("FromConfiguration", builder.Configuration.GetSection("Email"));
            //�ܵȼ��ڢ�
            builder.Services.AddOptions<EmailOptions>("AddOption").Configure(op =>
            {
                op.Title = "Title---AddOption";
                op.From = "From---AddOption";
            });
            ////�ݵ�������Ϊnullʱ�����������EmailOptions����
            //builder.Services.Configure<EmailOptions>(null, op =>
            //{
            //    op.Title = "Title---Configure--null";
            //    op.From = "From---Configure--null";
            //});
            ////�޵ȼ��ڢ�
            //builder.Services.ConfigureAll<EmailOptions>(op => op.From = "ConfigureAll");

            ////�ߵȼ��ڢݣ�PostConfiger����Configer�����ϼ�������
            //builder.Services.PostConfigure<EmailOptions>(null, op =>
            //{
            //    op.Title = "Title---PostConfigure--null";
            //    op.From = "From---PostConfigure--null";
            //});
            //builder.Services.PostConfigureAll<EmailOptions>(op => op.Body = "services.PostConfigure<EmailOption>--Name null--Same With PostConfigureAll");
            #endregion

            #region ��̬�ļ�֮�ļ������
            builder.Services.AddDirectoryBrowser();
            #endregion

            #region Session
            builder.Services.AddSession();

            //builder.Services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration = "127.0.0.1:6379";
            //    options.InstanceName = "RedisDistributedCache123";
            //});
            #endregion

            #region ��Ȩ��Ȩ

            #region ��Ȩ
            //builder.Services.AddAuthentication();//��Ȩ��ص�IOCע��--����������Ϊƾ֤�кܶ෽ʽ--Cookie--JWT
            #region �Զ����Ȩ-UrlToken
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    //��ʵ�ᱣ���key-value     Ҳ����name�����ظ�  value����UrlTokenAuthenticationHandler
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
            //         //options.ExpireTimeSpan//����ʱ��
            //         options.LoginPath = "/Home/Index";//δ��¼������ת��/Home/Indexҳ��
            //         options.AccessDeniedPath = "/Home/Privacy";//�е�½����δ��Ȩ������ת��/Home/Privacyҳ��
            //     });//ʹ��Cookie�ķ�ʽ
            #endregion
            
            #region ��Scheme��Ȩ
            ////Ĭ��scheme��UrlTokenScheme
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    //��ʵ�ᱣ���key-value     Ҳ����name�����ظ�  value����UrlTokenAuthenticationHandler
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    //options.ExpireTimeSpan//����ʱ��
            //    options.LoginPath = "/Home/Index";//δ��¼������ת��/Home/Indexҳ��
            //    options.AccessDeniedPath = "/Home/Privacy";//�е�½����δ��Ȩ������ת��/Home/Privacyҳ��
            //})//ʹ��Cookie�ķ�ʽ
            ////.AddJWT
            //;
            #endregion

            #region CustomAdd
            builder.Services.Replace(ServiceDescriptor.Scoped<IClaimsTransformation, CustomClaimsTransformation>());
            #endregion
            #endregion

            #region ��Ȩ
            builder.Services.AddSingleton<IAuthorizationHandler, DateOfBirthRequirementHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, CountryRequirementHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();//�ӿں͸���ֿ����͵�IOCע��
            builder.Services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            //builder.Services.AddAuthorization();//��Ȩ��ص�IOCע��--��AddMVC�Ѿ�����
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin");
                });//�ȼ���  Roles=Admin
                options.AddPolicy("MutiPolicy", policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin")//�����ڿ�ܷ�װ�õ�
                    .RequireUserName("Eleven")//Role  UserName������õ�
                    .RequireClaim(ClaimTypes.Country)//ֻҪ����Country����
                    .RequireAssertion(context =>//����������չ����--cliam֮��������ϢҲ����IP��
                    {
                        return context.User.HasClaim(c => c.Type == ClaimTypes.Email)
                        && context.User.Claims.First(c => c.Type == ClaimTypes.Email).Value.Equals("57265177@qq.com");
                    })
                    .RequireAssertion(context =>//ClaimTypes���ַ���������
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

            #region JWT��Ȩ+��Ȩ  HS��ʽ
            //JWTTokenOptions tokenOptions = new JWTTokenOptions();
            //builder.Configuration.Bind("JWTTokenOptions", tokenOptions);

            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            //      .AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
            //        ValidateIssuer = true,//�Ƿ���֤Issuer
            //        ValidateAudience = true,//�Ƿ���֤Audience
            //        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��---Ĭ�ϻ������300s��Ź���
            //        ClockSkew = TimeSpan.FromSeconds(0),//token���ں��������
            //        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey

            //        ValidAudience = tokenOptions.Audience,//Audience,��Ҫ��ǰ��ǩ��jwt������һ��
            //        ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//�õ�SecurityKey
            //    };
            //});
            #endregion

            #region JWT��Ȩ+��Ȩ  RS��ʽ
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
            //�����SecurityKey��ʵû��������,��������Ĺ�Կ
            #region ��ȡRSA��Key
            string path = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            string key = File.ReadAllText(path);
            Console.WriteLine($"KeyPath:{path}");
            var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);
            #endregion
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,//�Ƿ���֤Issuer
                  ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��

                  ValidateAudience = true,//�Ƿ���֤Audience
                  ValidAudience = tokenOptions.Audience,//Audience,��Ҫ��ǰ��ǩ��jwt������һ��

                  ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                  ClockSkew = TimeSpan.FromSeconds(0),//token���ں��������

                  ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                  IssuerSigningKey = new RsaSecurityKey(keyParams),

                  IssuerSigningKeyValidator = (m, n, z) =>
                  {
                      Console.WriteLine("This is IssuerValidator");
                      return true;
                  },//�Զ���У�����

                  IssuerValidator = (m, n, z) =>
                  {
                      Console.WriteLine("This is IssuerValidator");
                      return "http://localhost:5726";
                  },//�Զ���У�����
                  AudienceValidator = (m, n, z) =>
                  {
                      Console.WriteLine("This is AudienceValidator");
                      return true;
                      //return m != null && m.FirstOrDefault().Equals(this.Configuration["Audience"]);
                  },//�Զ���У����򣬿����µ�¼��֮ǰ����Ч
              };

              #region Events
              //���ṩ��ί����չ��Ҳ����ֱ��new�¶���override����
              options.Events = new JwtBearerEvents()
              {
                  OnAuthenticationFailed = context =>
                  {
                      Console.WriteLine($"This JWT Authentication OnAuthenticationFailed");
                      if (context.Exception.GetType().Name.Equals("SecurityTokenExpiredException"))
                      {
                          context.Response.Headers.Add("JWTAuthenticationFailed", "1");//
                      }
                      return Task.CompletedTask;
                  },
                  OnChallenge = context =>
                  {
                      Console.WriteLine($"This JWT Authentication OnChallenge");
                      context.Response.Headers.Add("JWTChallenge", "expired");//���߿ͻ����ǹ�����
                      return Task.CompletedTask;
                  },
                  OnForbidden = context =>
                  {
                      Console.WriteLine($"This JWT Authentication OnForbidden");
                      context.Response.Headers.Add("JWTForbidden", "1");//
                      return Task.CompletedTask;
                  },
                  OnMessageReceived = context =>
                  {
                      Console.WriteLine($"This JWT Authentication OnMessageReceived");
                      context.Response.Headers.Add("JWTMessageReceived", "1");//
                      return Task.CompletedTask;
                  },
                  OnTokenValidated = context =>
                  {
                      Console.WriteLine($"This JWT Authentication OnTokenValidated");
                      context.Response.Headers.Add("JWTTokenValidated", "1");//
                      return Task.CompletedTask;
                  }
              };
              #endregion
          });

            #endregion

            #endregion

            #region Build
            var app = builder.Build();
            #endregion

            #region Use�м��

            #region ��ԭ��Use�м��--�����ùܵ�ģ��
            //#region ������չ
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

            //#region ������չ
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
            //           //await next.Invoke(context);//û���¸��м��
            //           await context.Response.WriteAsync("This is The Chooen One! </br>");
            //           await context.Response.WriteAsync("This is Hello World 3 end </br>");
            //       });
            //});
            #endregion

            #region ����������չ�÷�
            #region Use
            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("use1 start");
            //    await next.Invoke();
            //    Console.WriteLine("use1 end");
            //});
            #endregion

            #region Run
            ////Run�൱��һ���ս�㣬Run֮����м�����ᱻִ�У���Ϊ������Useһ�����Ե���next.Invoke();
            //app.Run(context =>
            //{
            //    Console.WriteLine("run");
            //    return context.Response.WriteAsync("Run,Hello World!");
            //});
            #endregion

            #region Map
            //ֻ�з����ض���·���Ż�ִ��
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
            ////����������ʱ���м���Żᱻִ�У�����MapWhen������һ���µĹܵ�������������ʱ���µĹܵ���������ܵ�������ζ�����ܵ����м�����ᱻִ��
            //app.MapWhen(context =>
            //{
            //    return context.Request.Query.ContainsKey("Name");
            //    //�ܾ���chorme�����������  		
            //    //������		
            //    //��ajaxͳһ����		
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
            ////UseWhen��MapWhen���ƣ�Ҳ�ǵ���������ʱ���м���Żᱻִ�У�������UseWhen����������ܵ�
            //app.UseWhen(context =>
            //{
            //    return context.Request.Query.ContainsKey("Name");
            //    //�ܾ���chorme�����������  		
            //    //������		
            //    //��ajaxͳһ����		
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

            #region UseMiddlewareʽ--����
            //�Զ����м����-�޲���
            //app.UseMiddleware<FirstMiddleware>();
            ////ʵ��IMiddleWare�ӿ�
            //app.UseMiddleware<SecondMiddleware>();
            //ʵ��IMiddleWare�ӿ�-�в���
            //app.UseMiddleware<ThirdMiddleware>("DemoProject");
            #endregion

            #region ��׼�Զ����м��
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

            #region ���Ĭ���м��
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
            //    //����Ϣ��ExceptionHandlerMiddleware�м���ṩ�����������ExceptionHandlerMiddleware�м�����񵽵��쳣��Ϣ��
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


            #region ������
            app.UseHotlinkingPreventionMiddleware();
            #endregion

            #region ��̬�ļ�����
            app.UseStaticFiles();

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),//ָ��·��---����provider
            //    ServeUnknownFileTypes = false,//
            //    OnPrepareResponse = context =>
            //    {
            //        //��Ӧ����֮ǰ���ſ����޸�header
            //        context.Context.Response.Headers[HeaderNames.CacheControl] = "no-store";//"no-cache";//
            //    }
            //});

            ////���þ�̬�ļ��м��
            //var provider = new FileExtensionContentTypeProvider();
            ////׷��δ֪���͵�MIMEӳ��
            ////provider.Mappings.Add(".ini", "text/plain");
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticFiles")),
            //    RequestPath = new PathString("/StaticFiles"),
            //    ServeUnknownFileTypes = true,
            //    DefaultContentType = "image/jpeg", 
            //    //ContentTypeProvider = provider, //��������ø�����.ini�ļ������ܱ����ʣ���֧�ַ��ʲ������͵��ļ�������ο�FileExtensionContentTypeProvider��
            //    OnPrepareResponse = (context) =>
            //    {
            //        //��Ӧ����֮ǰ���ſ����޸�header
            //        //context.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");//���ÿɿ������
            //    }
            //});
            //app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
            //    RequestPath = "/StaticDirectory"
            //});//��ʵ�Ǹ�����
            #endregion

            #region Session
            app.UseSession();
            #endregion


            #region graph
            //http://localhost:5726/graph
            //https://graphviz.christine.website/
            //ʹ�ø��м����������ʾ��RoutePatternʸ��ͼ
            app.Map("/graph", branch => branch.UseMiddleware<GraphEndpointMiddleware>());
            #endregion

            #region UseRouting
            app.UseRouting();
            #endregion

            #region ��Ȩ��Ȩ
            //app.UseAuthentication();//���߿�ܣ��������ˣ���Ҫ����Ȩ--����µ�½��Ϣ
            app.UseAuthorization();//����ÿ������Ҫ����������ڵĴ���,����Ȩ����

            #endregion

            #region MapControllerRoute

            app.MapControllerRoute(
              name: "about-route",
              pattern: "about",
              defaults: new { controller = "Route", action = "About" }
              );

            //α��̬·��
            app.MapControllerRoute(
               name: "static",
               pattern: "static/{id:int}.html",
               defaults: new { controller = "Route", action = "StaticPage" });

            //����·�ɣ�����MVC��WebAPI������Ҫ����������---���ڲ�����

            //Լ��
            //http://localhost:5726/hello/11e  
            //http://localhost:5726/hello/11
            //http://localhost:5726/hello/eleven
            app.MapGet("/hello/{name}", async context =>
            {
                var name = context.Request.RouteValues["name"];
                await context.Response.WriteAsync($"Hello {name} no constraint !");
            });  //������   http://localhost:5726/hello/11e

            app.MapGet("/hello/{name:alpha}", async context =>
            {
                var name = context.Request.RouteValues["name"];
                await context.Response.WriteAsync($"Hello {name} alpha!");
            }); //������   http://localhost:5726/hello/eleven  

            app.MapGet("/hello/{name:int}", async context =>
            {
                var name = context.Request.RouteValues["name"];
                await context.Response.WriteAsync($"Hello {name} int!");
            }); //������   http://localhost:5726/hello/11


            //����·��---����ƥ������Լ����  ����Լ�����ߵ�Ĭ��·��
            //���ȼ�˳��range--����---Ĭ��

            // http://localhost:5726/Route/Data/2019-11  ���������㣬�������׼��rang
            // http://localhost:5726/Route/Data/2019-13  ����2�����Ҹ���׼��
            // http://localhost:5726/Route/Data/2018-09  ����2�����Ҹ���׼��
            // http://localhost:5726/Route/Data/2018-9   ֻ����Ĭ��·��
            // http://localhost:5726/Route/Data?year=2019&month=11  Ĭ��·��
            app.MapControllerRoute(
               name: "range",
               pattern: "{controller=Home}/{action=Index}/{year:range(2019,2021)}-{month:range(1,12)}");

            app.MapControllerRoute(
                name: "regular",
                pattern: "{controller}/{action}/{year}-{month}",
                constraints: new { year = "^\\d{4}$", month = "^\\d{2}$" },
                defaults: new { controller = "Home", action = "Index", });

            //�Զ���Լ��

            //��̬·��
            app.UseDynamicRouteDefault();

            //Ĭ��·��
            app.MapControllerRoute(
                name: "default",//·�ɵ�key---֧�ֶ��·�ɣ�key-value�洢�����Բ�Ҫ�ظ�key
                pattern: "{controller=Home}/{action=Index}/{id?}");//·�ɹ���
            //http://localhost:5726/home/index


            //MapGetָ������ʽ---MinimalAPI
            //http://localhost:5726/ElevenTest
            app.MapGet("/ElevenTest", async context =>
            {
                await context.Response.WriteAsync($"This is ElevenTest");
            })
            //.RequireAuthorization();//Ҫ����Ȩ
            //.WithMetadata(new AuditPolicyAttribute());//·�����еĻ������Զ�Ӹ�����
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