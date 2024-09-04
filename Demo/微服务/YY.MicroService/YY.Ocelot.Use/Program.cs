using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("ocelot.json");

builder.Services
    .AddOcelot();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseOcelot().Wait();

app.Run();