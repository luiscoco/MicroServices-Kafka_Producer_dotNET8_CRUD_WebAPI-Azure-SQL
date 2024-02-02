# How to integrate Kafka in a .NET8 Azure-SQL Microservice

## 1. Prerequisite

### 1.1. Kafka installation

Download Kafka (**kafka_2.13-3.6.1.tgz**) from Apache web page: https://kafka.apache.org/downloads

![image](https://github.com/luiscoco/MicroServices-Kafka_dotNET8_CRUD_WebAPI-Azure-SQL/assets/32194879/3a9121f2-9fe7-4a1f-a386-e91d288dfb94)

We uncompress the **kafka_2.13-3.6.1.tgz** and copy the folder in the C:/

![image](https://github.com/luiscoco/MicroServices-Kafka_dotNET8_CRUD_WebAPI-Azure-SQL/assets/32194879/79cf7efe-6c7f-4be9-925e-02bbad5c3ee1)

![image](https://github.com/luiscoco/MicroServices-Kafka_dotNET8_CRUD_WebAPI-Azure-SQL/assets/32194879/c71fde3b-fa3f-4118-9378-a1f0b36f4fbd)

We create add the **C:\kafka_2.13-3.6.1\bin\windows** in the **PATH** environmental variable

![image](https://github.com/luiscoco/MicroServices-Kafka_dotNET8_CRUD_WebAPI-Azure-SQL/assets/32194879/e2bd5826-a890-451c-a4ae-aaa89fb2dc4c)

![image](https://github.com/luiscoco/MicroServices-Kafka_dotNET8_CRUD_WebAPI-Azure-SQL/assets/32194879/e072ba06-e055-406b-abf4-1d5e0062b0bb)

**VERY IMPORTANT:** Set the **bootstrap_server** in the **server.properties** file

```
advertised.listeners=PLAINTEXT://localhost:9092
```

Run and Test Kafka

In a command prompt window we first **run Zookeper** 

```
zookeeper-server-start C:\kafka_2.13-3.6.1\config\zookeeper.properties
```

In another command prompt window we **run Kafka server**

```
kafka-server-start C:\kafka_2.13-3.6.1\config\server.properties
```

We creata producer

We create

### 1.2. Create Azure SQL database

See section 1 in this repo: https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-SQL

## 2. Create .NET8 CRUD WebAPI Azure-SQL Microservice

See this repo: https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-SQL

## 3. Load the project dependencies

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

Add/create first migration with this command:

```
dotnet ef migrations add InitialCreate
```

Also update the database with this command

```
dotnet ef database update
```

## 10. Review the project file and set InvariantGlobalization to false

In the AzureSQLWebAPIMicroservice.csproj set InvariantGlobalization to false

```
<InvariantGlobalization>false</InvariantGlobalization>
```

## 11. Run and test the application


