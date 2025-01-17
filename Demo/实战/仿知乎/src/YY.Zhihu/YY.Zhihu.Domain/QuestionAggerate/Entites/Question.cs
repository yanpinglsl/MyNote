﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Common;
using YY.Zhihu.SharedLibraries.Domain;

namespace YY.Zhihu.Domain.QuestionAggerate.Entites
{
    public class Question : AuditUserEntity,IAggregateRoot
    {

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public string? Summary { get; set; }
        public int ViewCount { get; set; }
        public int FollowerCount { get; set; }
        public ICollection<Answer>  Answers { get; set; } = new List<Answer>();
        public void AddViewCount(int count)
        {
            ViewCount += count;
        }
        public void GenerateSummary()
        {
            Summary = Description?.Substring(0, DataSchemaConstants.DefaultQuestionSummaryLength);
        }
    }
}
