using Microsoft.Extensions.Configuration;
using SampleMT.Common.Enumerators;

namespace SampleMT.Common.Extensions
{
    public static class IConfigurationExtensions
    {
        public static IConfigurationSection? GetModulesSection(this IConfiguration configuration) => configuration.GetSection("Modules");

        public static IConfigurationSection? GetModuleSection(this IConfiguration configuration, ModulesEnumerator module)
        {
            var moduleName = module.ToString();
            var moduleSection = configuration.GetModulesSection()?.GetChildren().Where(m => string.Equals(m.Value, moduleName, StringComparison.InvariantCultureIgnoreCase) || string.Equals(m.GetValue<string>("name"), moduleName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            
            return moduleSection;
        }

        public static bool IsModuleEnabled(this IConfiguration configuration, ModulesEnumerator module)
        {
            var moduleName = module.ToString();
            bool hasModule = configuration?.GetModuleSection(module) != null;

            return hasModule;
        }
    }
}
