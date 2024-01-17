using Id4.BookStore.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Id4.BookStore.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class BookStoreController : AbpControllerBase
{
    protected BookStoreController()
    {
        LocalizationResource = typeof(BookStoreResource);
    }
}
