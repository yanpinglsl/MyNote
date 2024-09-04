using Consul.AspNetCore;
using Winton.Extensions.Configuration.Consul;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// 加载consul配置中心配置
//多服务多个配置文件
string consul_url = "http://localhost:8500";
builder.Configuration.AddConsul(
    $"{builder.Environment.ApplicationName}/appsettings.json",
            options =>
            {
                //options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1、consul地址
                options.Optional = true; // 2、配置选项
                options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
            }
  ).AddConsul(
    $"{builder.Environment.ApplicationName}/customsettings.json",
            options =>
            {
                //options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1、consul地址
                options.Optional = true; // 2、配置选项
                options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
            });
builder.Services.AddConsul();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
