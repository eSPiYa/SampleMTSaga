using SampleMT.Common.Enumerators;
using SampleMT.Common.Extensions;
using SampleMT.Common.Interfaces;
using SampleMT.Common.Services;
using SampleMT.MTransit.Extensions;

namespace SampleMT.Service.Extensions
{
    internal static class IServiceCollectionExtensions
    {
        public static void AddModulesByConfig(this (IServiceCollection services, IConfiguration configuration) extend)
        {
            extend.services.AddScoped<IWeatherForecastService, DefaultWeatherForecastService>();

            if (extend.configuration.IsModuleEnabled(ModulesEnumerator.MassTransit))
            {
                extend.UseMassTransit();
            }
        }
    }
}
