using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using eindopdracht_device.Models;
using Microsoft.Azure.Cosmos;

namespace MCT.Function
{
    public static class Functions
    {
        [FunctionName("AddLog")]
        public static async Task<IActionResult> AddLog([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/addlog")] HttpRequest req,ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Get request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Deserialize JSON to Log object
            Log logItem = JsonConvert.DeserializeObject<Log>(requestBody);

            // Cosmos DB connection string
            string connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");

            // Add to Cosmos DB
            CosmosClientOptions options = new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            };

            var cosmosClient = new CosmosClient(connectionString, options);

            // Create new guid for the id
            logItem.Id = Guid.NewGuid();
            var container = cosmosClient.GetContainer("hota", "logs");
            await container.CreateItemAsync(logItem, new PartitionKey(logItem.HotAirBalloon));
            return new OkObjectResult(logItem);
            
        }
    }
}
