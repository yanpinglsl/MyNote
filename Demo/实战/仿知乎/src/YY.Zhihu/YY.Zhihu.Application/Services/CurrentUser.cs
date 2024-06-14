using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.Application.Services
{
    public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
    {
        public readonly ClaimsPrincipal? User = httpContextAccessor.HttpContext?.User;
        public int? Id
        {
            get
            {
                if (User == null)
                {
                    return null;
                }
                else
                {
                    string? userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    return userid == null ? null : Convert.ToInt32(userid);
                }
            }
        }
    }
}
