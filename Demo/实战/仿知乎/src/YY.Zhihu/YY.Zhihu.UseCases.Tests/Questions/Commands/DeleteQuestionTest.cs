using FluentAssertions;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.Tests;

namespace YY.Zhihu.UseCases.Tests.Questions.Commands
{
    public class DeleteAnswerTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var created = await Sender.Send(new CreateQuestionCommand("这是一个问题吗？", "这是一个问题的描述"));
            var result = await Sender.Send(new DeleteQuestionCommand(created.Value!.Id));
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new DeleteQuestionCommand(99));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("问题不存在");
        }

        [Fact]
        public async Task ShouldHaveAnswer()
        {
            var result = await Sender.Send(new DeleteQuestionCommand(1));
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain("问题下有回答，不能删除");
        }
    }
}
