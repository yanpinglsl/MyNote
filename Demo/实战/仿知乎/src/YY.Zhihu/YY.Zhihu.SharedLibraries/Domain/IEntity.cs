using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.SharedLibraries.Domain
{
    public class IEntity<TId>
    {
        TId Id { get; set; }
    }
}
