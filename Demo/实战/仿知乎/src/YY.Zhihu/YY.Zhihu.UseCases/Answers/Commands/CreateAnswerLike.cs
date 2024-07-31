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
using YY.Zhihu.Domain.QuestionAggerate.Event;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Dto;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Interfaces;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.UseCases.Answers.Commands
{

    [Authorize]
    public record CreateAnswerLikeCommand(int AnswerId, bool IsLike) : ICommand<IResult>;

    public class CreateAnswerLikeCommandValidator : AbstractValidator<CreateAnswerLikeCommand>
    {
        public CreateAnswerLikeCommandValidator()
        {
            RuleFor(command => command.AnswerId)
                .GreaterThan(0);
        }
    }
    public class CreateAnswerLikeHandler(
        IAnswerRepository answers,
        IUser user) : ICommandHandler<CreateAnswerLikeCommand, IResult>
    {
        public async Task<IResult> Handle(CreateAnswerLikeCommand command, CancellationToken cancellationToken)
        {
            AnswerByIdWithLikeByUserIdSpec spec = new AnswerByIdWithLikeByUserIdSpec(command.AnswerId, user.Id!.Value);
            var answer = await answers.GetAnswerByIdWithLikeByUserIdAsync(spec, cancellationToken);
            if (answer == null)
                return Result.NotFound("回答不存在");
            var result = answer.AddLike(user.Id!.Value, command.IsLike);
            if (!result.IsSuccess)
                return result;
            answer.AddDomainEvent(new AnswerLikeEvent(answer.QuestionId));
            await answers.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
