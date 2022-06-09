using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace TelnyxFunc;

public static class TextMessaging
{
    [FunctionName("ReceiveTextMessage")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequest req, ILogger log, [CosmosDB(
            "Telnyx",
            "Messages",
            ConnectionStringSetting = "CosmosDBConnectionString")]
        IAsyncCollector<string> receivedMessageOut)
    {
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            await receivedMessageOut.AddAsync(requestBody);
            return new OkResult();
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }

    }
}