using SampleMT.Common.Interfaces;
using SampleMT.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.Common.Services
{
    public class DefaultWeatherForecastService : IWeatherForecastService
    {
        public Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
        {
            var summaries = new[]
                {
                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                };

            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ));

            return Task.FromResult(forecast);
        }
    }
}
