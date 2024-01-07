using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextTemplateDemo.Demos.PasswordReset;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TextTemplating;

namespace TextTemplateDemo.Demos.Sample
{
    public class SampleDemo:ITransientDependency
    {
        private readonly ITemplateRenderer _templateRenderer;

        public SampleDemo(ITemplateRenderer templateRenderer)
        {
            _templateRenderer = templateRenderer;
        }

        public async Task RunAsync()
        {
            var result = await _templateRenderer.RenderAsync(
                "Sample", //the template name
                new SampleMode
                {
                    Name = "姚明",
                    Description = "篮球",
                }
            );

            Console.WriteLine(result);
        }
    }
}
