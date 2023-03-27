using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.RouteExtend
{
    /// <summary>
    /// 约束
    /// </summary>
    public class CustomGenderRouteConstraint : IRouteConstraint
    {
        /// <summary>
        /// 校验数据的---返回true就通过，false就失败
        /// 只支持0或者1
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="route"></param>
        /// <param name="routeKey"></param>
        /// <param name="values"></param>
        /// <param name="routeDirection"></param>
        /// <returns></returns>
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            Console.WriteLine($"This is {nameof(CustomGenderRouteConstraint)}.Match...");
            if (values.TryGetValue(routeKey, out object value))//拿到值
            {
                var parameterValueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (parameterValueString == null)
                {
                    return false;
                }
                else
                {
                    return parameterValueString.Equals("0") || parameterValueString.Equals("1");
                }
            }

            return false;
        }
    }
}
