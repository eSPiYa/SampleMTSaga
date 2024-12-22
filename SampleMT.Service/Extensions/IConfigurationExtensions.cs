using SampleMT.Service.Enumerators;

namespace SampleMT.Service.Extensions
{
    public static class IConfigurationExtensions
    {
        public static IConfigurationSection? GetModulesSection(this IConfiguration configuration) => configuration.GetSection("Modules");

        public static bool IsModuleEnabled(this IConfiguration configuration, ModulesEnumerator module)
        {
            var moduleName = module.ToString();
            bool? hasModule = configuration?.GetModulesSection()?.GetChildren().Any(m => string.Equals(m.Value, moduleName, StringComparison.InvariantCultureIgnoreCase) || string.Equals(m.GetValue<string>("name"), moduleName, StringComparison.InvariantCultureIgnoreCase));

            return hasModule.GetValueOrDefault(false);
        }
    }
}
