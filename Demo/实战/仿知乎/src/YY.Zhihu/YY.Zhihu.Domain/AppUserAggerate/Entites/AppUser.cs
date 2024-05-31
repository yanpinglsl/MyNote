using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;
using YY.Zhihu.SharedLibraries.Domain;

namespace YY.Zhihu.Domain.AppUserAggerate.Entites
{
    /// <summary>
    /// 用户类
    /// </summary>
    public class AppUser : AuditBaseEntity,IAggregateRoot
    {
        //EFCore框架必须保证每个类中含有无参构造函数，否则会报错
        //所以此处追加了该无参构造函数
        protected AppUser() { }

        public AppUser(int userId)
        {
            Id = userId;
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nickname { get; set; }

        public string? Avatar { get; set; }

        public string? Bio { get; set; }

        /// <summary>
        /// 关注列表
        /// </summary>
        public ICollection<FollowUser> Followees { get; set; } = new List<FollowUser>();

        /// <summary>
        /// 粉丝列表
        /// </summary>
        public ICollection<FollowUser> Followers { get; set; } = new List<FollowUser>();

        /// <summary>
        /// 关注问题列表
        /// </summary>
        public ICollection<FollowQuestion> FollowQuestions { get; set; } = new List<FollowQuestion>();
    }
}
