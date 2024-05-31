using Microsoft.AspNetCore.Identity;


namespace YY.Zhihu.Infrastructure.Identity
{
    public class JwtSettings
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Secret { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
    }
}