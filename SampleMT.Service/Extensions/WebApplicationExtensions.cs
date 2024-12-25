using SampleMT.Common.Enumerators;
using SampleMT.Common.Extensions;
using SampleMT.Common.Interfaces;
using SampleMT.Common.Models;

namespace SampleMT.Service.Extensions
{
    internal static class WebApplicationExtensions
    {
        public static void AddModulesByConfig(this WebApplication app)
        {
            if (app.Configuration.IsModuleEnabled(ModulesEnumerator.WebApi))
            {
                var summaries = new[]
                {
                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                };

                app.MapGet("/weatherforecast", async (CancellationToken cancellationToken, int days = 10) =>
                {
                    IEnumerable<WeatherForecast> weatherForecast;
                    using (var scope = app.Services.CreateScope())
                    {
                        var weatherForecastService = scope.ServiceProvider.GetRequiredService<IWeatherForecastService>();
                        weatherForecast = await weatherForecastService.GetWeatherForecastsAsync(days, cancellationToken);
                    }
                                        
                    return weatherForecast;
                })
                .WithName("GetWeatherForecast")
                .WithOpenApi();
            }
                
        }
    }
}
