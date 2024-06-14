using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.Common;
using YY.Zhihu.SharedLibraries.Result;

namespace YY.Zhihu.Domain.QuestionAggerate.Entites
{
    /// <summary>
    /// 回答类
    /// </summary>
    public class Answer : AuditUserEntity
    {
        /// <summary>
        /// 问题
        /// </summary>
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 点踩数
        /// </summary>
        public int DislikeCount { get; private set; }

        public List<AnswerLike> _answerLikes  = new List<AnswerLike>();
        public IReadOnlyCollection<AnswerLike> AnswerLikes  => _answerLikes.AsReadOnly();

        public IResult AddLike(int userId,bool isLike)
        {
            if (_answerLikes.Any(l => l.UserId == userId))
            {
                return Result.Failure("已赞或已踩");
            }
            if (isLike)
            {
                LikeCount++;
            }
            else
            {
                DislikeCount++;

            }
            _answerLikes.Add(new AnswerLike()
            {
                AnswerId = Id,
                UserId = userId,
                IsLike = isLike
            });
            return Result.Success();
        }

        /// <summary>
        /// 更新点赞
        /// 问题1：为什么不直接使用属性进行更新呢？
        /// 因为如果通过属性更新，则可能导致数据不一致的问题；比如更新了点赞数，却忘记更新点赞列表
        /// 问题2：为什么点赞数不直接使用list.Count
        /// 点赞/点踩数会频繁使用，当频繁获取列表的元素数量时，直接调用List的Count属性可能会导致性能下降，特别是当列表包含大量元素时
        /// </summary>
        /// <param name="answerLike"></param>
        public IResult UpdateLike(int userId, bool isLike)
        {
            var answerLike = _answerLikes.FirstOrDefault(like => like.UserId == userId);
            if (answerLike == null) 
                return Result.NotFound("未找到点赞记录");

            if (answerLike.IsLike == isLike) 
                return Result.Failure("已赞或已踩");

            answerLike.IsLike = isLike;

            if (isLike)
            {
                LikeCount++;
                DislikeCount--;
            }
            else
            {
                LikeCount--;
                DislikeCount++;
            }
            return Result.Success();
        }

        /// <summary>
        ///     移除点赞/点踩记录
        /// </summary>
        /// <param name="userId"></param>
        public void RemoveLike(int userId)
        {
            var answerLike = _answerLikes.FirstOrDefault(like => like.UserId == userId);

            if (answerLike == null) return;

            _answerLikes.Remove(answerLike);

            if (answerLike.IsLike) LikeCount -= 1;
            else DislikeCount -= 1;
        }
    }
}
