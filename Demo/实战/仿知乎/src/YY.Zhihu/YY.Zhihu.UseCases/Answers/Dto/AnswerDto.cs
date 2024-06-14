using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.UseCases.Answers.Dto
{
    public record AnswerDto
    {
        public int Id { get; init; }

        public string? Content { get; init; }

        public int LikeCount { get; init; }

        public DateTimeOffset? LastModifiedAt { get; init; }

        public int? CreatedBy { get; init; }

        public string? CreatedByNickName { get; init; }

        public string? CreatedByBio { get; init; }
    }
}
