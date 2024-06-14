using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;

namespace YY.Zhihu.Domain.AppUserAggerate.Specifications
{
    public class AppUserByIdSpec : Specification<AppUser>
    {
        public AppUserByIdSpec(int userId)
        {
            FilterCondition = user => user.Id.Equals(userId);
        }
    }
}
