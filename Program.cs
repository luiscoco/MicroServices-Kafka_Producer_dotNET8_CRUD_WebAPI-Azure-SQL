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
