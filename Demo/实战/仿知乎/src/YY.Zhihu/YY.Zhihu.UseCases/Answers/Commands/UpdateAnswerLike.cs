using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Dto;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.UseCases.Answers.Commands
{
    public record UpdateAnswerLikeCommand(int AnswerId, bool IsLike) : ICommand<IResult>;

    public class UpdateAnswerLikeCommandValidator : AbstractValidator<UpdateAnswerLikeCommand>
    {
        public UpdateAnswerLikeCommandValidator()
        {
            RuleFor(command => command.AnswerId)
                .GreaterThan(0);
        }
    }
    public class UpdateAnswerLikeHandler(
        IAnswerRepository answers,
        IUser user,
        IMapper mapper) : ICommandHandler<UpdateAnswerLikeCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateAnswerLikeCommand command, CancellationToken cancellationToken)
        {
            AnswerByIdWithLikeByUserIdSpec spec = new AnswerByIdWithLikeByUserIdSpec(command.AnswerId, user.Id!.Value);
            var answer = await answers.GetAnswerByIdWithLikeByUserIdAsync(spec, cancellationToken);
            if (answer == null)
                return Result.NotFound("回答不存在");
            var result = answer.UpdateLike(user.Id!.Value, command.IsLike);
            if (!result.IsSuccess)
                return result;
            await answers.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
