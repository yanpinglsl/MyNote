using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.ConfigurationExtend
{
    public class CustomConfigurationSource : IConfigurationSource
    {
        private readonly Action<CustomConfigurationOptions> _optionsAction;
        public CustomConfigurationSource(Action<CustomConfigurationOptions> optionsAction)
        {
            _optionsAction = optionsAction;
        }


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            CustomConfigurationOptions customConfigurationOptions = new CustomConfigurationOptions();
            this._optionsAction.Invoke(customConfigurationOptions);

            return new CustomConfigurationProvider(customConfigurationOptions);
        }
    }
}
