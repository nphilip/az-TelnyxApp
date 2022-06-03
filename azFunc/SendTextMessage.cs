using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Telnyx;

namespace TelnyxFunc;

public static class SendTextMessage
{
    [FunctionName("SendTextMessage")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequest req, ILogger log,
        [CosmosDB(
            "TextMessage",
            "SentMessage",
            ConnectionStringSetting = "CosmosDBConnectionString")]
        IAsyncCollector<string> sentMessageOut)
    {
        // Create a messaging service object
        var service = new MessagingSenderIdService();
        // Add in the options for the message to be sent
        var options = new NewMessagingSenderId
        {
            From = "+" + req.Query["From"],
            To = "+" + req.Query["To"],
            Text = req.Query["Text"]
        };
        
        try
        {
            var message = await service.CreateAsync(options);
            // Write to Cosmos DB
            await sentMessageOut.AddAsync(message.TelnyxResponse.ObjectJson);
            return new OkObjectResult(message.TelnyxResponse.ObjectJson);
        }
        catch (TelnyxException ex)
        {
            log.LogError(ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }

    }
}