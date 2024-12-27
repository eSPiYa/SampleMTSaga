using MassTransit;
using MassTransit.Contracts.JobService;
using Microsoft.Extensions.Logging;
using SampleMT.Common.Models;
using SampleMT.MTransit.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.StateMachines.WeatherForecast
{
    public class WeatherForecastStateMachine : MassTransitStateMachine<WeatherForecastState>
    {
        private readonly ILogger<WeatherForecastStateMachine> logger;

        public WeatherForecastStateMachine(ILogger<WeatherForecastStateMachine> logger)
        {
            this.logger = logger;

            InstanceState(x => x.CurrentState);

            #region Flow

            Initially(
                When(this.IRequestForecastsEvent)
                    .Then(x =>
                    {
                        this.logger.LogInformation($"Initialize for '{x.CorrelationId}' && '{x.Message.CorrelationId}'");

                        x.Saga.RequestId = x.RequestId;
                        x.Saga.ResponseAddress = x.ResponseAddress;
                        x.Saga.Days = x.Message.Days;
                        x.Saga.StartDate = DateOnly.FromDateTime(DateTime.Now);
                    })
                    .TransitionTo(this.GettingForecasts)
                    .ThenAsync(this.GetForecasts)
                );

            During(this.GettingForecasts,
                When(this.IGeneratedForecastMessageEvent)
                .Then(x =>
                {
                    this.logger.LogInformation($"Received '{typeof(IGeneratedForecastMessage).Name}' with JobId of '{x.Message.JobId}' for CorrelationId of '{x.CorrelationId}'");

                    var msg = x.Message;
                    var forecast = new Common.Models.WeatherForecast(msg.Date, msg.TemperatureC, msg.Summary);

                    x.Saga.JobsList[msg.JobId] = true;

                    x.Saga.WeatherForecasts.Add(forecast);
                })
                .If(context => context.Saga.WeatherForecasts.Count == context.Saga.Days, thenBinder => thenBinder
                    .ThenAsync(async x =>
                    {
                        var endpoint = await x.GetSendEndpoint(x.Saga.ResponseAddress);

                        await endpoint.Send<IRequestForecastsResponse>(new
                        {
                            CorrelationId = x.Message.CorrelationId,
                            Forecasts = x.Saga.WeatherForecasts.OrderBy(o => o.Date),
                        }, r => r.RequestId = x.Saga.RequestId);
                    })
                    .Then(x =>
                    {
                        this.logger.LogInformation($"Responded to request for CorrelationId of '{x.CorrelationId}'");
                    })
                    .TransitionTo(this.Final)
                )
                );
            #endregion

            SetCompletedWhenFinalized();
        }

        #region Events
        public Event<IRequestForecasts> IRequestForecastsEvent { get; private set; } = null!;
        public Event<IGeneratedForecastMessage> IGeneratedForecastMessageEvent { get; private set; } = null!;
        #endregion

        #region States
        public State GettingForecasts { get; private set; } = null!;
        #endregion

        #region Private Methods

        private async Task GetForecasts(BehaviorContext<WeatherForecastState, IRequestForecasts> context)
        {
            var startDate = context.Saga.StartDate;

            var tasks = Enumerable.Range(1, context.Saga.Days).Select(async idx =>
            {
                var jobId = NewId.NextGuid();

                context.Saga.JobsList.Add(jobId, false);

                this.logger.LogInformation($"Sending '{typeof(IGetForecastMessage).Name}' with JobId of '{jobId}' for CorrelationId of '{context.CorrelationId}'");

                await context.Publish<SubmitJob<IGetForecastMessage>>(new
                {
                    JobId = jobId,
                    Job = new
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Date = startDate.AddDays(idx)
                    }
                });
            });

            await Task.WhenAll(tasks);
        }
        #endregion
    }
}
