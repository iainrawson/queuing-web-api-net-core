using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Rewrite;
using DataSources.TidalData;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueueService;

// TODO: Can we get rid of Startup and use the more modern way of configuring a .NET CORE Web API, 
// or is this approach necessary as we're hosting two services within one app?
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            //options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        services.AddSwaggerGen();
        services.AddHttpClient<AdmiraltyTidalApiClient>(client => {

            var admiraltyTidalApiBaseUrl = (Configuration["AdmiraltyTidalApi:BaseUrl"] ?? "").Trim();
            if (string.IsNullOrWhiteSpace(admiraltyTidalApiBaseUrl)) {
                throw new ArgumentException(nameof(admiraltyTidalApiBaseUrl));
            }
            client.BaseAddress = new Uri(admiraltyTidalApiBaseUrl);

            var admiraltyTidalApiApiKey = (Configuration["AdmiraltyTidalApi:ApiKey"] ?? "").Trim();
            if (string.IsNullOrWhiteSpace(admiraltyTidalApiApiKey)) {
                throw new ArgumentException(nameof(admiraltyTidalApiApiKey));
            }
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", admiraltyTidalApiApiKey);
            
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        app.UseStaticFiles();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
        app.UseRouting();

        var option = new RewriteOptions();
        option.AddRedirect("^$", "swagger");
        app.UseRewriter(option);

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}