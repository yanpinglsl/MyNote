using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.StandardMiddlewareExtend
{
    public class BrowserFilterOptions
    {
        public bool EnableIE { get; set; } = false;
        public bool EnableEdge { get; set; } = false;
        public bool EnableChorme { get; set; } = false;
        public bool EnableFirefox { get; set; } = false;


        internal List<Func<HttpContext, Tuple<bool, string>>> DisableList = new List<Func<HttpContext, Tuple<bool, string>>>();

        public void InitDisableList(Func<HttpContext, Tuple<bool, string>> func)
        {
            this.DisableList.Add(func);
        }

    }
}
