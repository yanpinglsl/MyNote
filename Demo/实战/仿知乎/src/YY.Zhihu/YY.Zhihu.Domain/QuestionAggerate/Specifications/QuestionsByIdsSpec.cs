using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.Domain.QuestionAggerate.Specifications
{
    public class QuestionsByIdsSpec:Specification<Question>
    {

        public QuestionsByIdsSpec(int[] ids)
        {
            FilterCondition = question => ids.Contains(question.Id);
        }
    }
}
