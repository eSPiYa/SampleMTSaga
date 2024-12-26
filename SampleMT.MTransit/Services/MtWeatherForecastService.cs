using MassTransit;
using Microsoft.Extensions.Logging;
using SampleMT.Common.Interfaces;
using SampleMT.Common.Models;
using SampleMT.MTransit.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Services
{
    internal class MtWeatherForecastService : IWeatherForecastService
    {
        private readonly ILogger<MtWeatherForecastService> logger;
        private readonly IRequestClient<IRequestForecasts> weatherForecastClient;

        public MtWeatherForecastService(ILogger<MtWeatherForecastService> logger, 
                                        IRequestClient<IRequestForecasts> weatherForecastClient)
        {
            this.logger = logger;
            this.weatherForecastClient = weatherForecastClient;
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync(int days, CancellationToken ct)
        {   
            var correlationId = NewId.NextGuid();

            this.logger.LogInformation($"Sending IRequestForecasts request with CorrelationId of '{correlationId}' and number of days '{days}'");

            var response = await weatherForecastClient.GetResponse<IRequestForecastsResponse>(new
            {
                CorrelationId = correlationId,
                Days = days
            }, ct, RequestTimeout.After(m: 5));

            this.logger.LogInformation($"Received IRequestForecastsResponse response with CorrelationId of '{correlationId}'");

            return response.Message.Forecasts;
        }
    }
}
