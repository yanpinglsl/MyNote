using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Events;
using YY.Zhihu.Domain.Common;
using YY.Zhihu.SharedLibraries.Domain;
using YY.Zhihu.SharedLibraries.Result;

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
        public List<FollowUser> _followees = new List<FollowUser>();
        public IReadOnlyCollection<FollowUser> Followees  => _followees.AsReadOnly();

        /// <summary>
        /// 粉丝列表
        /// </summary>
        private List<FollowUser> _followers= new List<FollowUser>();
        public IReadOnlyCollection<FollowUser> Followers => _followers.AsReadOnly();

        /// <summary>
        /// 关注问题列表
        /// </summary>
        private List<FollowQuestion> _followQuestions = new List<FollowQuestion>();
        public IReadOnlyCollection<FollowQuestion> FollowQuestions => _followQuestions.AsReadOnly();

        public IResult AddFollowQuestion(int questionId)
        {
            if(_followQuestions.Any(l=>l.QuestionId == questionId))
            {
                return Result.Invalid("问题已关注");
            }
            FollowQuestion question = new FollowQuestion()
            {
                QuestionId = questionId,
                FollowDate = DateTime.Now
            };

            _followQuestions.Add(question);
            AddDomainEvent(new FollowQuestionAddedEvent(question));
            return Result.Success();
        }
        public void RemoveFollowQuestion(int questionId)
        {
           var question = _followQuestions.FirstOrDefault(l=>l.QuestionId == questionId);
            if(question != null)
            {
                _followQuestions.Remove(question);
                AddDomainEvent(new FollowQuestionRemovedEvent(question));
            }
        }
        public IResult AddFolloweeUser(int followeeId)
        {
            if(followeeId == Id)
            {
                return Result.Invalid("不能关注自己");
            }
            var followee = _followees.FirstOrDefault(l => l.FolloweeId == followeeId);
            if (followee != null)
            {
                return Result.Invalid("该用户已关注");
            }

            _followees.Add(new FollowUser()
            {
                FollowerId = Id,
                FolloweeId = followeeId,
                FollowDate = DateTime.Now
            });
            return Result.Success();
        }
        public void RemoveFolloweeUser(int followeeId)
        {
            var followee = _followees.FirstOrDefault(l => l.FolloweeId == followeeId);
            if (followee != null)
            {
                _followees.Remove(followee);
            }
        }
    }
}
