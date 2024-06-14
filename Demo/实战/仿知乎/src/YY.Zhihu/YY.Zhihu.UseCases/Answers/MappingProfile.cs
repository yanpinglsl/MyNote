using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.Common;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Domain;
using YY.Zhihu.UseCases.Answers.Commands;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.Questions;

namespace YY.Zhihu.UseCases.Answers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAnswerCommand, Answer>();
            CreateMap<UpdateAnswerCommand, Answer>();
        }
    }
}
