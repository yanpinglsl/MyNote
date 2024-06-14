using AutoMapper;
using FluentValidation;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.UseCases.Answers.Commands
{
    public record DeleteAnswerCommand(int questionId, int answerId) : ICommand<IResult>;

    public class DeleteAnswerCommandValidator : AbstractValidator<DeleteAnswerCommand>
    {
        public DeleteAnswerCommandValidator()
        {
            RuleFor(command => command.questionId)
                .GreaterThan(0);
            RuleFor(command => command.answerId)
                .GreaterThan(0);
        }
    }
    public class DeleteAnswerCommandHandler(
        IRepository<Question> questions,
        IUser user) : ICommandHandler<DeleteAnswerCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteAnswerCommand command, CancellationToken cancellationToken)
        {
            var spec = new AnswerByIdAndCreatedBySpec(user.Id!.Value, command.questionId, command.answerId);
            var question = await questions.GetSingleOrDefaultAsync(spec, cancellationToken);
            if (question == null)
                return Result.NotFound("问题不存在");
            var answer = question.Answers.FirstOrDefault(l => l.Id == command.answerId);
            if (answer == null)
                return Result.NotFound("回答不存在");
            question.Answers.Remove(answer);
            await questions.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
