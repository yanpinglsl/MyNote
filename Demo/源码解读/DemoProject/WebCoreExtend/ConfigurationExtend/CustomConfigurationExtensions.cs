using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.ConfigurationExtend
{
    public static class CustomConfigurationExtensions
    {
        public static void AddCustomConfiguration(
          this IConfigurationBuilder builder, Action<CustomConfigurationOptions> optionsAction)
        {
            builder.Add(new CustomConfigurationSource(optionsAction));
        }

        public static void AddCustomConfiguration(
         this IConfigurationBuilder builder)
        {
            builder.AddCustomConfiguration(options => { });
        }
    }
}
