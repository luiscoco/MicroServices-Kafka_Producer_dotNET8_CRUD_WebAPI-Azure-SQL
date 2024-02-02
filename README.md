# How to integrate Kafka in a .NET8 Azure-SQL Microservice

## 1. Prerequisite

### 1.1. Kafka installation

Install Kafka in you window laptop

Run and Test Kafka

### 1.2. Create Azure SQL database

See section 1 in this repo: https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-SQL

## 2. Create .NET8 CRUD WebAPI Azure-SQL Microservice

See this repo: https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-SQL

## 3. Load the 

![image](https://github.com/luiscoco/MicroServices-Kafka_dotNET8_CRUD_WebAPI-Azure-SQL/assets/32194879/25d953aa-c8ee-4b48-8525-efd257568f04)


## 4. Create the project folders structure

We create two new folders: **Config** and **Kafka**

![image](https://github.com/luiscoco/MicroServices-Kafka_dotNET8_CRUD_WebAPI-Azure-SQL/assets/32194879/9ad5e672-b90f-423a-b4e3-56f3a396a870)

We create two new files: **AppConfig.cs** and **KafkaProducer.cs**

## 5. Create the KafkaProducer.cs

```csharp
using Confluent.Kafka;

namespace AzureSQLWebAPIMicroservice.Kafka
{
    public class KafkaProducer
    {
        private readonly ProducerConfig _config;
        private readonly string _topic;

        public KafkaProducer(string bootstrapServers, string topic)
        {
            _config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _topic = topic;
        }

        public async Task SendMessageAsync(string key, string message)
        {
            using (var producer = new ProducerBuilder<string, string>(_config).Build())
            {
                try
                {
                    var result = await producer.ProduceAsync(_topic, new Message<string, string> { Key = key, Value = message });
                    Console.WriteLine($"Message sent to partition {result.Partition} with offset {result.Offset}");
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"Error producing message: {e.Error.Reason}");
                }
            }
        }
    }
}
```

## 6. Create the AppConfig.cs

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

## 7. Modify the appsettings.json

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

## 8. Modify the program.cs (middleware)

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

## 9. Initialize the database


## 10. 
