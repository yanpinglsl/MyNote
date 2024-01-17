using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Id4.BookStore.Web;

[Dependency(ReplaceServices = true)]
public class BookStoreBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "BookStore";
}
