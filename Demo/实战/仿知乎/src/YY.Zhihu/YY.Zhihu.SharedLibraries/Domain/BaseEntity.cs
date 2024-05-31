using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.SharedLibraries.Domain
{
    public abstract class BaseEntity<TId> : IEntity<TId>
    {
        public TId Id { get; set; } = default!;
    }
}
