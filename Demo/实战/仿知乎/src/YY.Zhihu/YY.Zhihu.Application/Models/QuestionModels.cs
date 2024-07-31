namespace YY.Zhihu.Application.Models
{
    public record CreateQuestionRequest(string Title, string? Description);
    public record UpdateQuestionRequest(string Title, string? Description);
}
