using AutoMapper;
using FluentValidation;
using MediatR;
using YY.Zhihu.Domain;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.UseCases.AppUsers.Commands
{
    [Authorize]
    public record DeleteQuestionCommand(int Id) : ICommand<IResult>;

    public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
    {
        public DeleteQuestionCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0);
        }
    }
    public class DeleteQuestionCommandHandler(
        IRepository<Question> questions,
        IUser user) : ICommandHandler<DeleteQuestionCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
        {
            var spec = new QuestionWithAnswerByCreatedBySpec(user.Id!.Value, command.Id);
            var question = await questions.GetSingleOrDefaultAsync(spec, cancellationToken);
            if (question == null)
                return Result.NotFound("问题不存在");
            if (question.Answers.Count != 0)
                return Result.Failure("问题下有回答，不能删除");
            questions.Delete(question);
            await questions.SaveChangesAsync();
            return Result.Success();
        }
    }
}
