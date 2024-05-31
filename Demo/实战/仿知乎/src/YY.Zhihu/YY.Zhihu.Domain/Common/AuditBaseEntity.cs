using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.Domain.Common
{
    public class AuditBaseEntity : BaseEntity
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
    }
}
