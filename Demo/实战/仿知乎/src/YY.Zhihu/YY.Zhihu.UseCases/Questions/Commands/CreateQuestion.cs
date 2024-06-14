using AutoMapper;
using FluentValidation;
using YY.Zhihu.Domain;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Questions.Dto;

namespace YY.Zhihu.UseCases.AppUsers.Commands
{
    public record CreateQuestionCommand(string Title, string? Description) : ICommand<Result<CreatedQuestionDto>>;

    public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
    {
        public CreateQuestionCommandValidator()
        {
            RuleFor(command => command.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(6, DataSchemaConstants.DefaultQuestionTitleLength)
                .Must(l => l.TrimEnd().EndsWith("?") || l.TrimEnd().EndsWith("？"))
                .WithMessage("问题标题必须以问号结尾");

            RuleFor(command => command.Description)
                .MaximumLength(DataSchemaConstants.DefaultDescriptionTitleLength);
        }
    }
    public class CreateQuestionHandler(
    IRepository<Question> questions,
    IMapper mapper) : ICommandHandler<CreateQuestionCommand, Result<CreatedQuestionDto>>
    {
        public async Task<Result<CreatedQuestionDto>> Handle(CreateQuestionCommand command, CancellationToken cancellationToken)
        {
            var question = mapper.Map<Question>(command);
            questions.Add(question);
            await questions.SaveChangesAsync();
            return Result.Success(new CreatedQuestionDto(question.Id));
        }
    }
}
