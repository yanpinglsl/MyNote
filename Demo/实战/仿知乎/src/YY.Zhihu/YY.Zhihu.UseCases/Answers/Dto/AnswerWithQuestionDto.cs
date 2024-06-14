using YY.Zhihu.UseCases.Questions.Dto;

namespace YY.Zhihu.UseCases.Answers.Dto
{
    public record AnswerWithQuestionDto
    {
        public AnswerDto Answer { get; init; } = null!;

        public QuestionDto Question { get; init; } = null!;
    }
}
