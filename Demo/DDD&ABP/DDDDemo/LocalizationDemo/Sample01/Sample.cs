﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace LocalizationDemo.Sample01
{
    public static class Sample
    {
        public static void Run()
        {
            var application = AbpApplicationFactory.Create<SampleAbpModule>();

            application.Initialize();

            var service = 
                application.ServiceProvider.GetService<SampleService>();

            service.Test();
        }
    }
}
