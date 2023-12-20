using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;

namespace GlobalizationLocalizationDemo.Extends
{
    public class CustomHeaderRequestCultureProvider : RequestCultureProvider
    {
        // Header 名称，默认为 Accept-Language
        public string HeaderName { get; set; } = HeaderNames.AcceptLanguage;

        // 当 Header 值有多个时，最多取前 n 个
        public int MaximumHeaderValuesToTry { get; set; } = 3;

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(httpContext));
            ArgumentException.ThrowIfNullOrEmpty(nameof(HeaderName));

            var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().GetList<StringWithQualityHeaderValue>(HeaderName);

            if (acceptLanguageHeader == null || acceptLanguageHeader.Count == 0)
            {
                return NullProviderCultureResult;
            }

            var languages = acceptLanguageHeader.AsEnumerable();

            if (MaximumHeaderValuesToTry > 0)
            {
                languages = languages.Take(MaximumHeaderValuesToTry);
            }

            var orderedLanguages = languages.OrderByDescending(h => h, StringWithQualityHeaderValueComparer.QualityComparer)
                .Select(x => x.Value).ToList();

            if (orderedLanguages.Count > 0)
            {
                return Task.FromResult(new ProviderCultureResult(orderedLanguages));
            }

            return NullProviderCultureResult;
        }
    }

}
