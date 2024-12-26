using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Messages
{
    public interface IGeneratedForecastMessage : CorrelatedBy<Guid>
    {
        Guid JobId { get; set; }
        DateOnly Date { get; set; }
        int TemperatureC { get; set; }
        string? Summary { get; set; }
    }
}
