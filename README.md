# How to integrate Kafka in a .NET8 Azure-SQL Microservice

## 1. Prerequisite

Install Kafka in you window laptop

Run and Test Kafka

## 2. Create .NET8 CRUD WebAPI Azure-SQL Microservice

See this repo: https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-SQL

## 3. Create the project folders structure


## 4. Create the KafkaProducer.cs


## 5. Create the AppConfig.cs

```csharp
namespace AzureSQLWebAPIMicroservice.Config
{
    public static class AppConfig
    {
        public static string BootstrapServers { get; set; } = string.Empty;
        public static string Topic { get; set; } = string.Empty;
    }
}
```

## 6. Modify the appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:mysqlserver1974luiscoco.database.windows.net,1433;Initial Catalog=mysqldatabasename;Persist Security Info=False;User ID=myadminlogin;Password=Luiscoco123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topic": "test"
  }
}
```

## 7. Modify the program.cs (middleware)

```csharp
using Microsoft.EntityFrameworkCore;
using AzureSQLWebAPIMicroservice.Data;
using AzureSQLWebAPIMicroservice.Services;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using AzureSQLWebAPIMicroservice.Kafka;
using AzureSQLWebAPIMicroservice.Config;

var builder = WebApplication.CreateBuilder(args);

// Initialize AppConfig with Kafka settings
AppConfig.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
AppConfig.Topic = builder.Configuration["Kafka:Topic"];

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ExampleModelService>();

// Add Kafka Producer Service using AppConfig
builder.Services.AddSingleton(new KafkaProducer(AppConfig.BootstrapServers, AppConfig.Topic));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
```


