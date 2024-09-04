
using CacheManager.Core;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Administration;
using Ocelot.Cache;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System.IdentityModel.Tokens.Jwt;
using YY.Ocelot.HttpApi;
using YY.Ocelot.HttpApi.Aggregator;
using YY.Ocelot.HttpApi.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("ocelot.json");


//CacheManager做缓存
//builder.Services
//    .AddOcelot()
//    .AddConsul()    
//    .AddCacheManager(x =>
//    {
//        //使用Dictionary方式处理
//        x.WithDictionaryHandle();
//    });


//CacheManager配合Redis做分布式缓存
//builder.Services
//    .AddOcelot()
//    .AddConsul()
//    //注册相关服务
//    .AddCacheManager(x =>
//    {
//        //配置Redis相关信息
//        x.WithRedisConfiguration("redis", cofig =>
//        {
//            cofig.WithAllowAdmin()//运行管理员相关操作
//            //.WithPassword("")//如果redis需要密码则配置
//            .WithDatabase(10)//指定数据库索引（Redis默认有16个）
//            .WithEndpoint("127.0.0.1", 6379);//指定IP+端口
//        }).WithRedisCacheHandle("redis",true);//指定配置
//        x.WithJsonSerializer();//指定数据序列化形式
//    });
builder.Services
    .AddOcelot()
    .AddConsul()
    .AddPolly()
    ////参数默认为false，用来表示是否作用于全局。
    ////如果使用的是默认的false的话，那么这个DelegatingHandler是专门针对ocelot.json里面指定的一个ReRoute的    
    //.AddDelegatingHandler<AddHeaderHandler>(true);
    //自定义聚合方式
    .AddSingletonDefinedAggregator<SampleAggregator>()
     //Administration模块
     .AddAdministration("/admin", options =>
      {
          options.Authority = "https://localhost:7010";
          options.Audience = "Ocelot";
          options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
      });

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//注册自己的缓存类
builder.Services.AddSingleton<IOcelotCache<CachedResponse>, CustomCache>();


//Ids配置
// 将身份认证服务添加到DI，并将“Bearer”配置为默认方案
// AddJwtBearer 将 JWT 认证处理程序添加到DI中以供身份认证服务使用
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("ApiGateway", options =>
    {
        options.Authority = "http://localhost:7010";//配置IdentityServer的授权地址
        options.RequireHttpsMetadata = false;//不需要https
        options.Audience = "YY";//api的name,需要与config的名称相同
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
app.UseOcelot().Wait();
app.Run();
