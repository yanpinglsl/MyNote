using FluentAssertions;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Questions.Queries;

namespace YY.Zhihu.UseCases.Tests.Answers.Queries
{
    public class GetAnswerWithQuestionTest(IServiceProvider serviceProvider):TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new GetQuestionQuery(1));
            result.Value.Should().NotBeNull();
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new GetQuestionQuery(99));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("问题不存在");
        }
    }
}
