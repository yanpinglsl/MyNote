  using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebCoreExtend.AuthorizationExtend.Requirement
{
    /// <summary>
    /// Country---支持传入
    /// </summary>
    public class CountryRequirement : IAuthorizationRequirement
    {
        public string Country = null;
        public CountryRequirement(string country)
        {
            this.Country = country;
        }
    }

    public class CountryRequirementHandler : AuthorizationHandler<CountryRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CountryRequirement requirement)
        {
            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.Country))
            {
                var emailCliamList = context.User.FindAll(c => c.Type == ClaimTypes.Country);//支持多Scheme
                if (emailCliamList.Any(c => c.Value.Contains(requirement.Country, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    //context.Fail();//没成功就留给别人处理
                }
            }
            return Task.CompletedTask;
        }
    }
}
