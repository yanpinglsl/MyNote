
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


//CacheManager������
//builder.Services
//    .AddOcelot()
//    .AddConsul()    
//    .AddCacheManager(x =>
//    {
//        //ʹ��Dictionary��ʽ����
//        x.WithDictionaryHandle();
//    });


//CacheManager���Redis���ֲ�ʽ����
//builder.Services
//    .AddOcelot()
//    .AddConsul()
//    //ע����ط���
//    .AddCacheManager(x =>
//    {
//        //����Redis�����Ϣ
//        x.WithRedisConfiguration("redis", cofig =>
//        {
//            cofig.WithAllowAdmin()//���й���Ա��ز���
//            //.WithPassword("")//���redis��Ҫ����������
//            .WithDatabase(10)//ָ�����ݿ�������RedisĬ����16����
//            .WithEndpoint("127.0.0.1", 6379);//ָ��IP+�˿�
//        }).WithRedisCacheHandle("redis",true);//ָ������
//        x.WithJsonSerializer();//ָ���������л���ʽ
//    });
builder.Services
    .AddOcelot()
    .AddConsul()
    .AddPolly()
    ////����Ĭ��Ϊfalse��������ʾ�Ƿ�������ȫ�֡�
    ////���ʹ�õ���Ĭ�ϵ�false�Ļ�����ô���DelegatingHandler��ר�����ocelot.json����ָ����һ��ReRoute��    
    //.AddDelegatingHandler<AddHeaderHandler>(true);
    //�Զ���ۺϷ�ʽ
    .AddSingletonDefinedAggregator<SampleAggregator>()
     //Administrationģ��
     .AddAdministration("/admin", options =>
      {
          options.Authority = "https://localhost:7010";
          options.Audience = "Ocelot";
          options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
      });

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//ע���Լ��Ļ�����
builder.Services.AddSingleton<IOcelotCache<CachedResponse>, CustomCache>();


//Ids����
// �������֤������ӵ�DI��������Bearer������ΪĬ�Ϸ���
// AddJwtBearer �� JWT ��֤���������ӵ�DI���Թ������֤����ʹ��
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("ApiGateway", options =>
    {
        options.Authority = "http://localhost:7010";//����IdentityServer����Ȩ��ַ
        options.RequireHttpsMetadata = false;//����Ҫhttps
        options.Audience = "YY";//api��name,��Ҫ��config��������ͬ
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
