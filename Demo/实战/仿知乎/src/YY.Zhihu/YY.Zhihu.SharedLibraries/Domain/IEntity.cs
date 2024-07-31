using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.SharedLibraries.Domain
{
    public interface IEntity;
    public class IEntity<TId>: IEntity
    {
        TId? Id { get; set; }
    }
}
