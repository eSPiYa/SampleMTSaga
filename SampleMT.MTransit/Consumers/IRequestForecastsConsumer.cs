using MassTransit;
using Microsoft.Extensions.Logging;
using SampleMT.Common.Models;
using SampleMT.MTransit.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Consumers
{
    internal class IRequestForecastsConsumer : IConsumer<IRequestForecasts>
    {
        private readonly ILogger<IRequestForecastsConsumer> logger;

        public IRequestForecastsConsumer(ILogger<IRequestForecastsConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IRequestForecasts> context)
        {
            this.logger.LogInformation($"Consuming request with CorrelationId of '{context.CorrelationId}'", context.Message);

            var days = context.Message.Days;

            var summaries = new[]
                {
                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                };

            var forecast = Enumerable.Range(1, days).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ));

            await context.RespondAsync<IRequestForecastsResponse>(new
            {
                CorrelationId = context.Message.CorrelationId,
                Forecasts = forecast
            });

            this.logger.LogInformation($"Responded to request with CorrelationId of '{context.CorrelationId}'");
        }
    }
}
