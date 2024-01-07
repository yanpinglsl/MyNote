using EmailSendDemo.AbpEmailingServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Test
{
    public class EmailHelperTest
    {
        public void EmailSend()
        {
            var services = new ServiceCollection();

            services.Configure<MailConfigOptions>("Smtp", options =>
            {
                options.Host = "smtp.sendgrid.net";
                options.Port = 587;
                options.UserName = "your username";
                options.Password = "your password";
                options.Domain = "";
                options.EnableSsl = false;
                options.UseDefaultCredentials = false;
                options.DefaultFromAddress = "noreply@abp.io";
                options.DefaultFromDisplayName = "ABP application";
            });

            services.Configure<MailConfigOptions>("MailKit", options =>
            {
                options.Host = "smtp.sendgrid.net";
                options.Port = 465;
                options.UserName = "your username";
                options.Password = "your password";
                options.Domain = "";
                options.EnableSsl = true;
                options.UseDefaultCredentials = false;
                options.DefaultFromAddress = "noreply@abp.io";
                options.DefaultFromDisplayName = "ABP application";
            });

            services.AddTransient<SmtpEmailSender>();
            services.AddTransient<MailKitSmtpEmailSender>();
            var serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetRequiredService<MailTestService>().RunAsync();
        }
    }
}
