using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NAutowired;
using NAutowired.AspNetCore6.WebAPI.Sample.Config;
using NAutowired.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add Controllers As Services
builder.Services.AddControllers().AddControllersAsServices();

//Replace `IControllerActivator` implement.
builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, NAutowiredControllerActivator>());
//Add config to ioc container
builder.Services.Configure<SnowflakeConfig>(builder.Configuration.GetSection("Snowflake"));
//Use auto dependency injection
builder.Services.AutoRegisterDependency(new List<string> { "NAutowired.AspNetCore6.WebAPI.Sample" });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();

