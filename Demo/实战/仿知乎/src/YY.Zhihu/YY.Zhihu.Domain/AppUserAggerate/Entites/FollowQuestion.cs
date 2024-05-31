using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;

namespace YY.Zhihu.Domain.AppUserAggerate.Entites
{
    /// <summary>
    /// 关注问题类
    /// </summary>
    public class FollowQuestion : BaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户类
        /// </summary>
        public AppUser AppUser { get; set; } = null!;

        /// <summary>
        /// 关注问题
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 关注问题的时间
        /// </summary>
        public DateTimeOffset FollowDate { get; set; }
    }
}
