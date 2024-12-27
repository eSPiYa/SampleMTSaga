using Marten;
using MassTransit;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleMT.Common.Enumerators;
using SampleMT.Common.Extensions;
using SampleMT.Common.Interfaces;
using SampleMT.MTransit.Consumers;
using SampleMT.MTransit.Enumerators;
using SampleMT.MTransit.Messages;
using SampleMT.MTransit.Models;
using SampleMT.MTransit.Services;
using SampleMT.MTransit.StateMachines.WeatherForecast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SampleMT.MTransit.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void UseMassTransit(this (IServiceCollection services, IConfiguration configuration) extend)
        {
            var mtConfig = extend.configuration.GetModuleSection(ModulesEnumerator.MassTransit)!;

            if (mtConfig.IsSubModuleEnabled(SubModulesEnumerator.Processor))
            {
                extend.services.AddScoped<IWeatherForecastService, MtWeatherForecastService>();
            }

            extend.services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConfigureEndpointsCallback((context, name, cfg) =>
                {
                    cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30)));
                    cfg.UseMessageRetry(r => r.Immediate(5));
                    cfg.UseInMemoryOutbox(context);
                });

                if (mtConfig!.IsUsedPersistenceProvider(PersistenceProviderEnumerator.InMemory))
                    x.SetInMemorySagaRepositoryProvider();
                else if(mtConfig!.IsUsedPersistenceProvider(PersistenceProviderEnumerator.Marten))
                {
                    var provider = mtConfig!.GetPersistenceProvider<MartenPersistenceProvider>();

                    extend.services.AddMarten(options =>
                    {
                        options.Connection(provider.ConnectionString);
                    });

                    x.SetMartenSagaRepositoryProvider(true);
                }

                if (mtConfig.IsSubModuleEnabled(SubModulesEnumerator.GetForecastMessageJobConsumer))
                    x.AddConsumer<IGetForecastMessageJobConsumer>(cfg =>
                    {
                        cfg.Options<JobOptions<IGetForecastMessage>>(options => options
                            .SetJobTimeout(TimeSpan.FromMinutes(15))
                            .SetConcurrentJobLimit(10));
                    });

                if (mtConfig.IsSubModuleEnabled(SubModulesEnumerator.RequestForecastsConsumer))
                    x.AddConsumer(typeof(IRequestForecastsConsumer));

                if (mtConfig.IsSubModuleEnabled(SubModulesEnumerator.SagaStateMachine))
                {
                    x.AddSagaStateMachine<WeatherForecastStateMachine, WeatherForecastState>();

                    x.AddJobSagaStateMachines();
                }

                if (mtConfig!.IsUsedProvider(ProvidersEnumerator.RabbitMQ))
                {
                    var provider = mtConfig!.GetProvider<RabbitMqProvider>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.UseDelayedMessageScheduler();

                        cfg.Host(provider.Host, provider.VirtualHost, hostCfg =>
                        {
                            hostCfg.Username(provider.User);
                            hostCfg.Password(provider.Password);
                        });

                        cfg.UseMessageRetry(r => r.Intervals(100, 500, 1000, 5000));

                        cfg.ConfigureEndpoints(context);
                    });
                }
            });
        }
    }
}
