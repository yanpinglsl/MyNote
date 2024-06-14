using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.UseCases.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]

    public class AuthorizeAttribute: Attribute
    {
    }
}
