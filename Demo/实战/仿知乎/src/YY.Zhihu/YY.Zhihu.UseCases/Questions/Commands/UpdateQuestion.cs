using AutoMapper;
using FluentValidation;
using YY.Zhihu.Domain;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.AppUserAggerate.Specifications;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.UseCases.AppUsers.Commands
{
    [Authorize]
    public record UpdateQuestionCommand(int Id,string Title, string? Description) : ICommand<IResult>;

    public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
    {
        public UpdateQuestionCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0);

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
    public class UpdateQuestionHandler(
    IRepository<Question> questions,
    IUser user,
    IMapper mapper) : ICommandHandler<UpdateQuestionCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateQuestionCommand command, CancellationToken cancellationToken)
        {
            var spec = new QuestionByCreatedBySpec(user.Id!.Value, command.Id);
            var question = await questions.GetSingleOrDefaultAsync(spec,cancellationToken);
            if (question == null)
               return Result.NotFound("问题不存在");
            mapper.Map(command, question);
            await questions.SaveChangesAsync();
            return Result.Success();
        }
    }
}
