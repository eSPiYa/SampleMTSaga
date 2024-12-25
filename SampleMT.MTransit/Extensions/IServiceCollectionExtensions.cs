using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleMT.Common.Enumerators;
using SampleMT.Common.Extensions;
using SampleMT.Common.Interfaces;
using SampleMT.MTransit.Consumers;
using SampleMT.MTransit.Enumerators;
using SampleMT.MTransit.Models;
using SampleMT.MTransit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

                x.AddConsumer(typeof(IRequestForecastsConsumer));

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
