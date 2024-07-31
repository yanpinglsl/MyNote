using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.SharedLibraries.Domain;

namespace YY.Zhihu.Domain.QuestionAggerate.Event
{
    public class AnswerLikeEvent(int questionId) : BaseEvent
    {
        public int QuestionId { get; set; } = questionId;
    }
}
