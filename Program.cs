using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((context, loggingBuilder) =>
            {
                loggingBuilder.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                });
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOpenTelemetryTracing(builder =>
                {
                    var tracingOtlpEndpoint = hostContext.Configuration["OTLP_ENDPOINT_URL"];

                    builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(hostContext.HostingEnvironment.ApplicationName))
                           .AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddSource("OtPrGrJa.Example");

                    if (tracingOtlpEndpoint != null)
                    {
                        builder.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new System.Uri(tracingOtlpEndpoint);
                        });
                    }
                    else
                    {
                        builder.AddConsoleExporter();
                    }
                });
            });
}
