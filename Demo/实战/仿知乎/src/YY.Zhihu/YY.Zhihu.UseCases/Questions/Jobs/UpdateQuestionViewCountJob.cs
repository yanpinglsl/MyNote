using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Repositoy;

namespace YY.Zhihu.UseCases.Questions.Jobs
{
    public class UpdateQuestionViewCountJob(
        IRepository<Question> questions,
        QuestionViewCountService questionViewCount) : IJob
    {
        public static JobKey Key = new JobKey(nameof(UpdateQuestionViewCountJob), nameof(Question));
        private const int BatchSize = 20;
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var questionViewDict = questionViewCount.GetAndReset();
                if (questionViewDict == null) return;
                var totalBatch = (int)Math.Ceiling((double)questionViewDict.Count / BatchSize);
                for (int i = 0; i < totalBatch; i++)
                {
                    var batchData = questionViewDict.Skip((i) * BatchSize).Take(BatchSize).ToDictionary();
                    var ids = batchData.Keys.ToArray();
                    var questionList = await questions.GetListAsync(new QuestionsByIdsSpec(ids));

                    if (questionList.Count == 0) return;
                    foreach (var item in questionList)
                    {
                        item.AddViewCount(batchData[item.Id]);
                    }
                }
                await questions.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }
    }
}
