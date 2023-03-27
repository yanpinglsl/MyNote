using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.AuthenticationExtend
{
    /// <summary>
    /// 需要注册到AddAuthentication之后
    /// </summary>
    public class CustomClaimsTransformation : IClaimsTransformation
    {
        /// <summary>
        /// 如果是中国，添加个language为cn
        /// </summary>
        /// <param name="principal">The user.</param>
        /// <returns>The principal unchanged.</returns>
        public virtual Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            Console.WriteLine(principal.Claims.FirstOrDefault().Value);

            if (principal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Country))?.Value.Equals("China") ?? false)
            {
                //principal.Claims.Append(new Claim("language", "cn"));//这种方式无效
                ClaimsIdentity? identity = principal.Identity as ClaimsIdentity;
                identity?.AddClaim(new Claim("language", "cn"));
            }

            return Task.FromResult(principal);
        }
    }
}
