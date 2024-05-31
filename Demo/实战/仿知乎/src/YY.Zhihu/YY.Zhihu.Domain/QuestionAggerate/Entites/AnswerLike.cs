using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;

namespace YY.Zhihu.Domain.QuestionAggerate.Entites
{
    public class AnswerLike : AuditBaseEntity
    {
        public int UserId { get; set; }
        public int AnswerId { get; set; }
        public Answer Answer { get; set; } = null!;

        public bool isLike { get; set; }
        public bool isDisLike { get; set; }

    }
}
