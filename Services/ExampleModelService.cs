using AzureSQLWebAPIMicroservice.Data;
using AzureSQLWebAPIMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AzureSQLWebAPIMicroservice.Kafka;
using AzureSQLWebAPIMicroservice.Config;

namespace AzureSQLWebAPIMicroservice.Services
{
    public class ExampleModelService
    {
        private readonly ExampleDbContext _context;
        private readonly KafkaProducer _kafkaProducer;

        public ExampleModelService(ExampleDbContext context)
        {
            _context = context;
            // Initialize the KafkaProducer with your Kafka configuration
            _kafkaProducer = new KafkaProducer(AppConfig.BootstrapServers, AppConfig.Topic);
        }

        // Modify each CRUD method to send a Kafka message after the operation

        // Create
        public async Task<ExampleModel> AddExampleModel(ExampleModel model)
        {
            _context.ExampleModels.Add(model);
            await _context.SaveChangesAsync();

            // Send Kafka message
            await _kafkaProducer.SendMessageAsync("create", JsonConvert.SerializeObject(model));

            return model;
        }

        // Read all
        public async Task<List<ExampleModel>> GetAllExampleModels()
        {
            return await _context.ExampleModels.ToListAsync();
        }

        // Read by ID
        public async Task<ExampleModel> GetExampleModelById(int id)
        {
            return await _context.ExampleModels.FirstOrDefaultAsync(e => e.Id == id);
        }

        // Update
        public async Task<ExampleModel> UpdateExampleModel(int id, ExampleModel model)
        {
            var existingModel = await _context.ExampleModels.FirstOrDefaultAsync(e => e.Id == id);
            if (existingModel == null)
            {
                return null;
            }

            existingModel.Name = model.Name;
            // Update other properties as necessary

            _context.Entry(existingModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Send Kafka message
            await _kafkaProducer.SendMessageAsync("update", JsonConvert.SerializeObject(existingModel));

            return existingModel;
        }

        // Delete
        public async Task<bool> DeleteExampleModel(int id)
        {
            var model = await _context.ExampleModels.FindAsync(id);
            if (model == null)
            {
                return false;
            }

            _context.ExampleModels.Remove(model);
            await _context.SaveChangesAsync();

            // Send Kafka message
            await _kafkaProducer.SendMessageAsync("delete", JsonConvert.SerializeObject(new { Id = id }));

            return true;
        }
    }
}
