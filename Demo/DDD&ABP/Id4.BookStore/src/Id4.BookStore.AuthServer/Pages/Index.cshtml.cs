using IdentityServer4.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.Localization;
using Client = Volo.Abp.IdentityServer.Clients.Client;

namespace Id4.BookStore.Pages;

public class IndexModel : AbpPageModel
{
    public List<Client>? Clients { get; protected set; }

    public IReadOnlyList<LanguageInfo>? Languages { get; protected set; }

    public string? CurrentLanguage { get; protected set; }

    protected IClientRepository ClientRepository { get; }

    protected ILanguageProvider LanguageProvider { get; }

    public IndexModel(IClientRepository openIdApplicationRepository, ILanguageProvider languageProvider)
    {
        ClientRepository = openIdApplicationRepository;
        LanguageProvider = languageProvider;
    }

    public async Task OnGetAsync()
    {
        Clients = await ClientRepository.GetListAsync();

        Languages = await LanguageProvider.GetLanguagesAsync();
        CurrentLanguage = CultureInfo.CurrentCulture.DisplayName;
    }
}
