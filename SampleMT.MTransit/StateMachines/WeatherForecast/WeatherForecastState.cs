using MassTransit;
using SampleMT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.StateMachines.WeatherForecast
{
    public class WeatherForecastState : SagaStateMachineInstance, ISagaVersion
    {
        public required string CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public int Days { get; set; }
        public DateOnly StartDate { get; set; }
        public IDictionary<Guid, bool> JobsList { get; private set; } = new Dictionary<Guid, bool>();
        public IList<Common.Models.WeatherForecast> WeatherForecasts { get; private set; } = new List<Common.Models.WeatherForecast>();
        public Guid? RequestId { get; set; }
        public Uri ResponseAddress { get; set; } = null!;
    }
}
