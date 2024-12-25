using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMT.Common.Extensions
{
    public static class IConfigurationSectionExtensions
    {
        public static IConfigurationSection GetSubModules(this IConfigurationSection configSection)
        {
            var subModules = configSection.GetSection("subModules");

            return subModules;
        }

        public static IConfigurationSection? GetSubModule(this IConfigurationSection configSection, string subModuleName)
        {
            var subModuleSection = configSection.GetSubModules().GetChildren().Where(m => string.Equals(m.Value, subModuleName, StringComparison.InvariantCultureIgnoreCase) || string.Equals(m.GetValue<string>("name"), subModuleName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            return subModuleSection;
        }

        public static bool IsSubModuleEnabled(this IConfigurationSection configSection, string subModuleName)
        {
            bool hasSubModule = configSection.GetSubModule(subModuleName) != null;

            return hasSubModule;
        }
    }
}
