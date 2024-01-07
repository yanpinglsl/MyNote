using System.Net;
using AbpExceptionDemo.Localization.ErrorCode;
using AbpExceptionDemo.Localization.Exception;
using Microsoft.AspNetCore.Builder;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace AbpExceptionDemo;

[DependsOn(typeof(AbpAspNetCoreMvcModule))]
public class AppModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocaliztion();
        ConfigureExceptionLocalization();

        //配置是否显示详细的错误信息
        //此处配置后会影响BusinessException的显示
        //Configure<AbpExceptionHandlingOptions>(options =>
        //{
        //    options.SendExceptionsDetailsToClients = true;
        //});
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseRouting();

        app.UseConfiguredEndpoints();

        
    }

    public void ConfigureLocaliztion()
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AppModule>(
                nameof(AbpExceptionDemo),
                "Localization");
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ExceptionResource>("zh-Hans")
                .AddVirtualJson("/Localization/Exception/Resources");
        });
    }

    public void ConfigureExceptionLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ErrorCodeResource>("zh-Hans")
                .AddVirtualJson("/Localization/ErrorCode/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Sample", typeof(ErrorCodeResource));
        });


        Configure<AbpExceptionHttpStatusCodeOptions>(options =>
        {
            options.Map(SampleErrorCodes.CustomExceptionMessage, HttpStatusCode.BadRequest);
            options.Map(SampleErrorCodes.ThisIsABusinessException, HttpStatusCode.InternalServerError);
        });

    }
}