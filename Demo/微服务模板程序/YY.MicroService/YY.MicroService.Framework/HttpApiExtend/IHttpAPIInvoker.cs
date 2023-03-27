using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.MicroService.Framework.HttpApiExtend
{
    public interface IHttpAPIInvoker
    {
        string InvokeApi(string url);
    }
}
