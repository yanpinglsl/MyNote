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
    /// 直接规则---跟RolesAuthorizationRequirement是一样的
    /// </summary>
    public class SingleEmailRequirement : AuthorizationHandler<SingleEmailRequirement>, IAuthorizationRequirement
    {
        public SingleEmailRequirement(string requiredName)
        {
            if (requiredName == null)
            {
                throw new ArgumentNullException(nameof(requiredName));
            }

            RequiredName = requiredName ?? "@qq.com";
        }
        /// <summary>
        /// 邮件域名，默认@qq.com
        /// </summary>
        public string RequiredName { get; }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SingleEmailRequirement requirement)
        {
            

            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var emailCliamList = context.User.FindAll(c => c.Type == ClaimTypes.Email);//支持多Scheme
                if (emailCliamList.Any(c => c.Value.EndsWith(RequiredName, StringComparison.OrdinalIgnoreCase)))//数据库校验--Redis校验
                {
                    context.Succeed(requirement);
                }
                else
                {
                    //context.Fail();//失败不管
                }
            }
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"{nameof(SingleEmailRequirement)}:Requires a user email end with {RequiredName}";
        }
    }
}
