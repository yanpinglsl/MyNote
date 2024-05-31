using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.Domain.Common
{
    public class AuditUserEntity:AuditBaseEntity
    {
        public int? CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
