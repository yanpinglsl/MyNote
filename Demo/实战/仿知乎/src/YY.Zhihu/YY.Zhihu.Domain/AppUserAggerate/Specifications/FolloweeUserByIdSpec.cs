using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;

namespace YY.Zhihu.Domain.AppUserAggerate.Specifications
{
    public class FolloweeUserByIdSpec : Specification<AppUser>
    {
        public FolloweeUserByIdSpec(int userId, int followeeId)
        {
            FilterCondition = user => user.Id.Equals(userId);

            AddInclude(user => user.Followees.Where(fq => fq.FolloweeId == followeeId));
        }
    }
}
