using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;

namespace YY.Zhihu.Domain.AppUserAggerate.Entites
{
    /// <summary>
    /// 关注列表/粉丝列表类
    /// </summary>
    public class FollowUser : BaseEntity
    {
        /// <summary>
        /// 关注者
        /// </summary>
        public int FollowerId { get; set; }
        public AppUser Follower { get; set; } = null!;

        /// <summary>
        /// 被关注者
        /// </summary>
        public int FolloweeId { get; set; }
        public AppUser Followee { get; set; } = null!;

        /// <summary>
        /// 关注时间
        /// </summary>
        public DateTimeOffset FollowDate { get; set; }
    }
}
