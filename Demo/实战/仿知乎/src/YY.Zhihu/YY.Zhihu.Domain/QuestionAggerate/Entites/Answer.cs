using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;

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
        public ICollection<AnswerLike> AnswerLikes { get; set; } = new List<AnswerLike>();

        /// <summary>
        /// 点踩数
        /// </summary>
        public int DisLikeCount { get; set; }
        public ICollection<AnswerLike> AnswerDisLikes { get; set; } = new List<AnswerLike>();

        /// <summary>
        /// 更新点赞
        /// 问题1：为什么不直接使用属性进行更新呢？
        /// 因为如果通过属性更新，则可能导致数据不一致的问题；比如更新了点赞数，却忘记更新点赞列表
        /// 问题2：为什么点赞数不直接使用list.Count
        /// 点赞/点踩数会频繁使用，当频繁获取列表的元素数量时，直接调用List的Count属性可能会导致性能下降，特别是当列表包含大量元素时
        /// </summary>
        /// <param name="answerLike"></param>
        public void UpdateLikeList(AnswerLike answerLike)
        {
            bool isExist = AnswerDisLikes.Contains(answerLike);
            if (answerLike.isLike && !isExist)
            {
                LikeCount++;
                AnswerLikes.Add(answerLike);
            }
            else if (!answerLike.isLike && isExist)
            {
                LikeCount--;
                AnswerLikes.Remove(answerLike);
            }
        }
        /// <summary>
        /// 更新点踩
        /// </summary>
        /// <param name="answerLike"></param>
        public void UpdateDisLikeList(AnswerLike answerLike)
        {
            bool isExist = AnswerDisLikes.Contains(answerLike);
            if (answerLike.isLike && !isExist)
            {
                LikeCount++;
                AnswerDisLikes.Add(answerLike);
            }
            else if (!answerLike.isLike && isExist)
            {
                LikeCount--;
                AnswerDisLikes.Remove(answerLike);
            }
        }
    }
}
