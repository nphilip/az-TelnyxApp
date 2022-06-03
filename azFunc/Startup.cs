using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Telnyx;
using TelnyxFunc;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TelnyxFunc;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        TelnyxConfiguration.SetApiKey(Environment.GetEnvironmentVariable("TelnyxApiKey"));
        if (Environment.GetEnvironmentVariable("TelnyxApiKey") is null)
            throw new NullReferenceException("TelnyxApiKey not set - configure in Application Settings");
        TelnyxConfiguration.SetApiKey(Environment.GetEnvironmentVariable("TelnyxApiKey"));
    }
}