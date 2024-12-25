using MassTransit;
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
        private readonly IRequestClient<IRequestForecasts> weatherForecastClient;

        public MtWeatherForecastService(IRequestClient<IRequestForecasts> weatherForecastClient)
        {
            this.weatherForecastClient = weatherForecastClient;
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync(int days, CancellationToken ct)
        {
            var correlationId = NewId.NextGuid();

            var response = await weatherForecastClient.GetResponse<IRequestForecastsResponse>(new
            {
                CorrelationId = correlationId,
                Days = days
            }, ct, RequestTimeout.After(m: 5));

            return response.Message.Forecasts;
        }
    }
}
