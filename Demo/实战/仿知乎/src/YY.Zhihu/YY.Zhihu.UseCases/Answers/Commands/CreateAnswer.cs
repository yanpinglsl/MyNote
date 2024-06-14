using AutoMapper;
using FluentValidation;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Dto;

namespace YY.Zhihu.UseCases.Answers.Commands
{
    public record CreateAnswerCommand(int QuestionId,string? Content) : ICommand<Result<CreatedAnswerDto>>;

    public class CreateAnswerCommandValidator : AbstractValidator<CreateAnswerCommand>
    {
        public CreateAnswerCommandValidator()
        {
            RuleFor(command => command.QuestionId)
                .GreaterThan(0);
            RuleFor(command => command.Content)
                .NotEmpty();
        }
    }
    public class CreateAnswerHandler(
    IRepository<Question> questions,
    IMapper mapper) : ICommandHandler<CreateAnswerCommand, Result<CreatedAnswerDto>>
    {
        public async Task<Result<CreatedAnswerDto>> Handle(CreateAnswerCommand command, CancellationToken cancellationToken)
        {
            var question = await questions.GetByIdAsync(command.QuestionId, cancellationToken);
            if (question == null) 
                return Result.NotFound("问题不存在");

            var answer = mapper.Map<Answer>(command);
            question.Answers.Add(answer);
            var answerId = await questions.SaveChangesAsync();
            return Result.Success(new CreatedAnswerDto(command.QuestionId, answerId));
        }
    }
}
