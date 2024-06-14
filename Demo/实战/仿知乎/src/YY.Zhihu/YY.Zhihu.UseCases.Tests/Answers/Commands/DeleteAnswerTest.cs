using FluentAssertions;
using Xunit;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Answers.Commands;
using YY.Zhihu.UseCases.AppUsers.Commands;
using YY.Zhihu.UseCases.Tests;

namespace YY.Zhihu.UseCases.Tests.Answers.Commands
{
    public class DeleteAnswerTest(IServiceProvider serviceProvider) : TestBase(serviceProvider)
    {
        [Fact]
        public async Task ShouldSuccess()
        {
            var result = await Sender.Send(new DeleteAnswerCommand(1, 1));
            result.Status.Should().Be(ResultStatus.Ok);
        }

        [Fact]
        public async Task ShouldQuestionNoExists()
        {
            var result = await Sender.Send(new DeleteAnswerCommand(9999, 1));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("问题不存在");
        }

        [Fact]
        public async Task ShouldAnswerNoExists()
        {
            var result = await Sender.Send(new DeleteAnswerCommand(1, 9999));
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain("回答不存在");
        }
    }
}
