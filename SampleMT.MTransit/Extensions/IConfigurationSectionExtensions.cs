using Microsoft.Extensions.Configuration;
using SampleMT.Common.Extensions;
using SampleMT.MTransit.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.MTransit.Extensions
{
    internal static class IConfigurationSectionExtensions
    {
        public static IConfigurationSection GetPersistenceProvider(this IConfigurationSection configSection)
        {
            var providerConfigSection = configSection.GetSection("persistence");

            return providerConfigSection;
        }

        public static T GetPersistenceProvider<T>(this IConfigurationSection configSection) where T : class => configSection!.GetPersistenceProvider()!.Get<T>()!;

        public static IConfigurationSection GetProvider(this IConfigurationSection configSection)
        {
            var providerConfigSection = configSection.GetSection("provider");

            return providerConfigSection;
        }

        public static T GetProvider<T>(this IConfigurationSection configSection) where T : class => configSection!.GetProvider()!.Get<T>()!;

        public static bool IsUsedPersistenceProvider(this IConfigurationSection configSection, PersistenceProviderEnumerator provider)
        {
            var providerName = provider.ToString();
            var providerConfigSection = configSection.GetPersistenceProvider();
            bool result = string.Equals(providerConfigSection.GetValue<string>("name"), providerName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        public static bool IsUsedProvider(this IConfigurationSection configSection, ProvidersEnumerator provider)
        {
            var providerName = provider.ToString();
            var providerConfigSection = configSection.GetProvider();
            bool result = string.Equals(providerConfigSection.GetValue<string>("name"), providerName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        public static bool IsSubModuleEnabled(this IConfigurationSection configSection, SubModulesEnumerator subModule)
        {
            var subModuleName = subModule.ToString();

            return configSection.IsSubModuleEnabled(subModuleName);
        }
    }
}
