using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleMT.Common.Enumerators;
using SampleMT.Common.Extensions;
using SampleMT.MTransit.Enumerators;
using SampleMT.MTransit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void UseMassTransit(this (IServiceCollection services, IConfiguration configuration) extend)
        {
            var mtConfig = extend.configuration.GetModuleSection(ModulesEnumerator.MassTransit);

            extend.services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

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

                        cfg.ConfigureEndpoints(context);
                    });
                }
            });
        }
    }
}
