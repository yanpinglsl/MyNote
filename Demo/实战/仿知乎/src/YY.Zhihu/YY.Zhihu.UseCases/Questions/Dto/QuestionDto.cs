using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.UseCases.Questions.Dto
{
    public record QuestionDto
    {
        public int Id { get; init; }

        public string Title { get; init; } = null!;

        public string? Description { get; init; }

        public int AnswerCount { get; init; }

        public int FollowerCount { get; init; }

        public int ViewCount { get; init; }
    }
}
