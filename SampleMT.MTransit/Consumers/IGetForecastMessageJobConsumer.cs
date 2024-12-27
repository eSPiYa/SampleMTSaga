using MassTransit;
using Microsoft.Extensions.Logging;
using SampleMT.MTransit.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Consumers
{
    public class IGetForecastMessageJobConsumer : IJobConsumer<IGetForecastMessage>
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<IGetForecastMessageJobConsumer> logger;

        public IGetForecastMessageJobConsumer(ILogger<IGetForecastMessageJobConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Run(JobContext<IGetForecastMessage> context)
        {
            context.CancellationToken.Register(() =>
            {
                this.logger.LogInformation($"Cancelling Job: {context.JobId}", context.Job);
            });

            await context.Publish<IGeneratedForecastMessage>(new
            {
                CorrelationId = context.Job.CorrelationId,
                JobId = context.JobId,
                Date = context.Job.Date,
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });

            this.logger.LogInformation($"Sending '{typeof(IGeneratedForecastMessage).Name}' with JobId of '{context.JobId}' for CorrelationId of '{context.CorrelationId}'");
        }
    }
}
