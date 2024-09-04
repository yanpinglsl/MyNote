using YY.IdentityServer;

var builder = WebApplication.CreateBuilder(args);


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
    .AddInMemoryClients(Config.GetClients())//客户端配置添加到内存
    .AddTestUsers(Config.GetUsers()); 

var app = builder.Build();

app.UseIdentityServer();

app.Run();

