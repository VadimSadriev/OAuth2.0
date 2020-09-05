using Microsoft.Extensions.Configuration;
using System;

namespace OAuth.Common.Configuration
{
    public static class Extensions
    {
        public static IConfigurationSection CheckExistence(this IConfigurationSection section)
        {
            if (!section.Exists())
                throw new ArgumentNullException($"Configuration {section.Path} not found");

            return section;
        }
    }
}
