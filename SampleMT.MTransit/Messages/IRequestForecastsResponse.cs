using MassTransit;
using SampleMT.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Messages
{
    public interface IRequestForecastsResponse : CorrelatedBy<Guid>
    {
        IEnumerable<WeatherForecast> Forecasts { get; set; }
    }
}
