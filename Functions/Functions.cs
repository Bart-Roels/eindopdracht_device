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
using System.Collections.Generic;
using System.Linq;

namespace MCT.Function
{
    public static class Functions
    {
        [FunctionName("AddLog")]
        public static async Task<IActionResult> AddLog([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/addlog")] HttpRequest req, ILogger log)
        {

            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                // Get request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                // Deserialize JSON to Log object
                Log logItem = JsonConvert.DeserializeObject<Log>(requestBody);

                // Cosmos DB connection string
                string connectionString = Environment.GetEnvironmentVariable("CosmosDB");

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
            catch (System.Exception ex)
            {
                // Log exception
                log.LogError(ex.Message);
                return new BadRequestObjectResult("something went wrong");
            }

        }

        [FunctionName("ReadLog")]
        public static async Task<IActionResult> ReadLog([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/logs")] HttpRequest req, ILogger log)
        {
            try
            {

                // Cosmos DB connection string
                string connectionString = Environment.GetEnvironmentVariable("CosmosDB");

                // Connect to Cosmos DB
                CosmosClientOptions options = new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Gateway
                };
                var cosmosClient = new CosmosClient(connectionString, options);

                // Get all logs
                var container = cosmosClient.GetContainer("hota", "logs");
                var sqlQueryText = "SELECT * FROM c";
                var queryDefinition = new QueryDefinition(sqlQueryText);
                var iterator = container.GetItemQueryIterator<Log>(queryDefinition);
                List<Log> logs = new List<Log>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    logs.AddRange(response.ToList());
                }

                return new OkObjectResult(logs);

            }
            catch (System.Exception ex)
            {
                // Log exception
                log.LogError(ex.Message);
                // Return error
                return new BadRequestObjectResult("Something went wrong");
            }


        }
    }
}
