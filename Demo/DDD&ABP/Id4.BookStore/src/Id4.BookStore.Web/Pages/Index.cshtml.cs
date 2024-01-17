using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Id4.BookStore.Web.Pages;

public class IndexModel : BookStorePageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
