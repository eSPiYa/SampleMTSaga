using SampleMT.Common.Enumerators;
using SampleMT.Common.Extensions;
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

                app.MapGet("/weatherforecast", () =>
                {
                    var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                        .ToArray();
                    return forecast;
                })
                .WithName("GetWeatherForecast")
                .WithOpenApi();
            }
                
        }
    }
}
