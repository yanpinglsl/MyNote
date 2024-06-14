using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.SharedLibraries.Domain;

namespace YY.Zhihu.Domain.AppUserAggerate.Events
{
    public class FollowQuestionRemovedEvent(FollowQuestion followQuestion) : BaseEvent
    {
        public FollowQuestion FollowQuestion { get; } = followQuestion;
    }
}
