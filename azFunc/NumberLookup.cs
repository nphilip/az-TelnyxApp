using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Telnyx.net.Services;

namespace TelnyxFunc;

public static class NumberLookup
{
    [FunctionName("NumberLookup")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req, ILogger log, [CosmosDB(
            "NumberLookup",
            "LookupRecord",
            ConnectionStringSetting = "CosmosDBConnectionString")]
        IAsyncCollector<string> lookupRecordOut)
    {
        try
        {
            var service = new NumberLookupService();
            var updateOptions = new NumberLookupRecordOptions
            {
                Type = null
            };
            var lookup = await service.GetAsync(req.Query["number"], updateOptions);
            await lookupRecordOut.AddAsync(lookup.TelnyxResponse.ObjectJson);
            return new OkObjectResult(lookup.TelnyxResponse.ResponseJson);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }
}