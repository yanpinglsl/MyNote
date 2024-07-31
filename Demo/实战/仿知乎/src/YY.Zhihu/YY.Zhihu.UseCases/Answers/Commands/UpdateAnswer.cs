using AutoMapper;
using FluentValidation;
using YY.Zhihu.Domain;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.UseCases.Answers.Commands
{
    [Authorize]
    public record UpdateAnswerCommand(int QuestionId, int AnswerId, string Content) : ICommand<IResult>;

    public class UpdateAnswerCommandValidator : AbstractValidator<UpdateAnswerCommand>
    {
        public UpdateAnswerCommandValidator()
        {
            RuleFor(command => command.QuestionId)
                .GreaterThan(0);
            RuleFor(command => command.AnswerId)
                .GreaterThan(0);
            RuleFor(command => command.Content)
                .NotEmpty();
        }
    }
    public class UpdateQuestionHandler(
    IRepository<Question> questions,
    IUser user,
    IMapper mapper) : ICommandHandler<UpdateAnswerCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateAnswerCommand command, CancellationToken cancellationToken)
        {
            var spec = new AnswerByIdAndCreatedBySpec(user.Id!.Value, command.QuestionId, command.AnswerId);
            var question = await questions.GetSingleOrDefaultAsync(spec,cancellationToken);
            if (question == null)
               return Result.NotFound("问题不存在");
            var answer = question.Answers.FirstOrDefault(l => l.Id == command.AnswerId);
            if (answer == null )
                return Result.NotFound("回答不存在");
            mapper.Map(command, answer);
            await questions.SaveChangesAsync();
            return Result.Success();
        }
    }
}
