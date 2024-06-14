using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.Zhihu.UseCases.Answers.Dto
{
    public record CreatedAnswerDto(int questionId, int answerId);
}
