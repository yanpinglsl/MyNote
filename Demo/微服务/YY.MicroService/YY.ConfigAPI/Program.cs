using Consul.AspNetCore;
using Winton.Extensions.Configuration.Consul;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// ����consul������������
//������������ļ�
string consul_url = "http://localhost:8500";
builder.Configuration.AddConsul(
    $"{builder.Environment.ApplicationName}/appsettings.json",
            options =>
            {
                //options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1��consul��ַ
                options.Optional = true; // 2������ѡ��
                options.ReloadOnChange = true; // 3�������ļ����º����¼���
                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4�������쳣
            }
  ).AddConsul(
    $"{builder.Environment.ApplicationName}/customsettings.json",
            options =>
            {
                //options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1��consul��ַ
                options.Optional = true; // 2������ѡ��
                options.ReloadOnChange = true; // 3�������ļ����º����¼���
                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4�������쳣
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
