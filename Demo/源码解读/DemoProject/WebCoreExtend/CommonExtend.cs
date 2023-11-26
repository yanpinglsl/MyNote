using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.WebCore
{
    public static class CommonExtend
    {
        //Microsoft.AspNetCore.Mvc.Routing.DynamicRouteValueTransformer
        //Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }

        /// <summary>
        /// 基于HttpContext,当前鉴权方式解析，获取用户信息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static UserInfo GetCurrentUserInfo(this HttpContext httpContext)
        {
            IEnumerable<Claim> claimlist = httpContext.AuthenticateAsync().Result.Principal.Claims;
            return new UserInfo()
            {
                id = long.Parse(claimlist.FirstOrDefault(u => u.Type == "id").Value),
                username = claimlist.FirstOrDefault(u => u.Type == "username").Value ?? "匿名"
            };
        }
    }
    public class UserInfo
    {
        public long id { get; set; }
        public string username { get; set; }
    }
    public class CurrentUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        public DateTime LoginTime { get; set; }

        public List<Data> Datas { get; set; }
    }
    public class Data
    { }
}
